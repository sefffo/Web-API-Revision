# =============================================================
# MULTI-STAGE BUILD
# A Dockerfile can have multiple FROM instructions — each one
# starts a brand new stage with its own filesystem.
# The key benefit: only the LAST stage becomes the final image.
# Stage 1 compiles the code using the heavy SDK (~800MB).
# Stage 2 runs the app using only the lightweight runtime (~220MB).
# We throw away everything from Stage 1 — source code, compiler,
# NuGet cache — nothing leaks into the production image.
# =============================================================


# =============================================================
# STAGE 1 — BUILD
# Use the full .NET 10 SDK image. "AS build" names this stage
# so Stage 2 can reference it later with --from=build.
# The SDK includes: C# compiler, MSBuild, NuGet client, dotnet CLI.
# =============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Set the working directory inside the container.
# Every subsequent COPY and RUN command runs relative to /src.
# If /src doesn't exist, Docker creates it automatically.
WORKDIR /src


# ── LAYER CACHING TRICK ──────────────────────────────────────
# Docker builds images in layers — one layer per instruction.
# Each layer is cached based on its inputs. If inputs haven't
# changed, Docker reuses the cached layer and skips that step.
#
# The expensive step here is "dotnet restore" — it downloads
# ALL NuGet packages for the entire solution. This can take
# 30-60 seconds on a cold run.
#
# Strategy:
#   1. Copy ONLY the .csproj and .slnx files first
#   2. Run dotnet restore immediately
#   3. Copy the actual source code AFTER
#
# Result: if you only change a .cs file (which is 99% of the time),
# Docker sees the .csproj files haven't changed → reuses the cached
# restore layer → skips package downloads entirely → much faster rebuild.
#
# If you add/remove a NuGet package (changes .csproj), the cache
# is invalidated only from that COPY line down — which is correct.
# ─────────────────────────────────────────────────────────────
COPY ECommerceSolution.slnx .
COPY ECommerce.Web/ECommerce.Web.csproj                                    ECommerce.Web/
COPY ECommerce.Domain/ECommerce.Domain.csproj                              ECommerce.Domain/
COPY ECommerce.Persistence/ECommerce.Persistence.csproj                    ECommerce.Persistence/
COPY ECommerce.Service/ECommerce.Services.csproj                           ECommerce.Service/
COPY ECommerce.Services.Abstraction/ECommerce.Services.Abstraction.csproj  ECommerce.Services.Abstraction/
COPY ECommerce.SharedLibirary/ECommerce.SharedLibirary.csproj              ECommerce.SharedLibirary/
COPY ECommerce.Presentation/ECommerce.Presentation.csproj                  ECommerce.Presentation/

# Restore all NuGet packages for the entire solution.
# Uses ECommerceSolution.slnx to discover all projects.
# This is the slow step — cached aggressively by the trick above.
RUN dotnet restore ECommerceSolution.slnx


# ── COPY SOURCE CODE ─────────────────────────────────────────
# Now copy ALL remaining files (controllers, services, configs, etc.)
# "." means: copy everything from the Docker build context (repo root)
# into the current WORKDIR (/src).
#
# The .dockerignore file runs first and strips out:
#   /bin, /obj  → compiled output (rebuilt fresh inside container)
#   .git        → version history (not needed to run the app)
#   *.md, images, PDFs → documentation (not needed at runtime)
# This keeps the build context small and the COPY fast.
# ─────────────────────────────────────────────────────────────
COPY . .


# ── PUBLISH ──────────────────────────────────────────────────
# Compile and publish the API project in Release mode.
#
# -c Release      → Production build: optimized IL, no debug symbols,
#                   smaller and faster than Debug mode.
# -o /app/publish → Output all compiled DLLs, assets, and config files
#                   into /app/publish inside the container.
# --no-restore    → Skip restore — we already ran it above.
#                   Prevents redundant package downloads.
#
# After this step, /app/publish contains everything needed to RUN
# the app: ECommerce.Web.dll, all dependency DLLs, appsettings.json,
# wwwroot assets, etc.
# ─────────────────────────────────────────────────────────────
RUN dotnet publish ECommerce.Web/ECommerce.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# =============================================================
# STAGE 2 — RUNTIME
# Switch to the ASP.NET Core runtime-only image (~220MB).
# This image has NO SDK, NO compiler, NO NuGet client.
# It only contains the .NET runtime needed to EXECUTE the app.
#
# Why not just use the SDK image for everything?
# - SDK image: ~800MB (includes build tools you don't need at runtime)
# - Runtime image: ~220MB (lean, minimal attack surface, faster pulls)
# - Multi-stage = you get the best of both worlds
# =============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Working directory inside the runtime container.
WORKDIR /app

# Copy ONLY the published output from Stage 1.
# "--from=build" references the named stage above.
# We grab /app/publish (compiled app) — NOT the source code.
# The source code, SDK, and NuGet cache are all discarded here.
COPY --from=build /app/publish .

# Declare that this container listens on port 8080.
# ASP.NET Core 8+ defaults to port 8080 inside containers
# (not 5000/5001 like older versions).
# EXPOSE is documentation only — the actual host port mapping
# is configured in docker-compose.yml under "ports".
EXPOSE 8080

# The command executed when the container starts.
# Equivalent to running: dotnet ECommerce.Web.dll
# Uses exec form (JSON array) — more reliable than shell form
# because it runs dotnet directly as PID 1 (no shell wrapper),
# which means OS signals (SIGTERM for graceful shutdown) are
# received directly by the .NET process.
ENTRYPOINT ["dotnet", "ECommerce.Web.dll"]
