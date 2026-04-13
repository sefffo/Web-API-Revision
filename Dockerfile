# ─────────────────────────────────────────────
# STAGE 1: BUILD
# We use the full .NET 10 SDK image here because we need
# the compiler, NuGet, and all build tools.
# "AS build" gives this stage a name so stage 2 can reference it.
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Set the working directory inside the container.
# All subsequent COPY and RUN commands operate relative to this path.
WORKDIR /src

# ── Copy project files BEFORE source code ──
# This is a Docker layer caching trick. Docker builds in layers —
# if these .csproj files haven't changed, Docker reuses the cached
# restore layer and skips re-downloading all NuGet packages.
# This saves minutes on every rebuild when you only changed C# code.
COPY ECommerceSolution.slnx .
COPY ECommerce.Web/ECommerce.Web.csproj                                    ECommerce.Web/
COPY ECommerce.Domain/ECommerce.Domain.csproj                              ECommerce.Domain/
COPY ECommerce.Persistence/ECommerce.Persistence.csproj                    ECommerce.Persistence/
COPY ECommerce.Service/ECommerce.Services.csproj                           ECommerce.Service/
COPY ECommerce.Services.Abstraction/ECommerce.Services.Abstraction.csproj  ECommerce.Services.Abstraction/
COPY ECommerce.SharedLibirary/ECommerce.SharedLibirary.csproj              ECommerce.SharedLibirary/
COPY ECommerce.Presentation/ECommerce.Presentation.csproj                  ECommerce.Presentation/

# Restore all NuGet packages defined across the solution.
# Uses the .slnx file to discover all projects and their dependencies.
# This runs BEFORE copying source code — cache trick explained above.
RUN dotnet restore ECommerceSolution.slnx

# Now copy ALL remaining source files into the container.
# The dot "." means "copy everything from the build context (your repo root)
# into the current WORKDIR (/src)".
# .dockerignore filters out /bin, /obj, .git, images, etc. before this runs.
COPY . .

# Publish the API project in Release mode into /app/publish.
# -c Release    → optimized build, no debug symbols
# -o /app/publish → output folder
# --no-restore  → skip restore since we already did it above (faster)
RUN dotnet publish ECommerce.Web/ECommerce.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─────────────────────────────────────────────
# STAGE 2: RUNTIME
# We switch to the much smaller ASP.NET runtime image.
# It has NO SDK, NO compiler, NO NuGet — just what's needed to RUN the app.
# This is why multi-stage builds exist: the final image is ~200MB
# instead of ~800MB if we used the SDK image for everything.
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Working directory inside the runtime container.
WORKDIR /app

# Copy ONLY the published output from stage 1 into this stage.
# "--from=build" references the named stage above.
# We don't copy source code — just the compiled DLLs and assets.
COPY --from=build /app/publish .

# Tell Docker this container listens on port 8080.
# ASP.NET Core 8+ defaults to port 8080 (not 5000/5001) in containers.
# This is documentation only — the actual port binding happens in docker-compose.
EXPOSE 8080

# The command that runs when the container starts.
# "dotnet ECommerce.Web.dll" launches your compiled API.
ENTRYPOINT ["dotnet", "ECommerce.Web.dll"]
