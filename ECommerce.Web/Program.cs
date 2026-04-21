using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DataSeed;
using ECommerce.Persistence.Data.DbContexts;
using ECommerce.Persistence.Data.IdentityData;
using ECommerce.Persistence.IdentityData.DataSeeding;
using ECommerce.Persistence.Repositories;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using ECommerce.Services.Services;
using ECommerce.Services.Servicies;
using ECommerce.Services.Servicies.Payment_Service;
using ECommerce.SharedLibirary.Settings;
using ECommerce.Web.CustomMiddleWares;
using ECommerce.Web.Extensions;
using ECommerce.Web.Factories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace ECommerce.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            #region Auth

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DashboardPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173",   // Vite dev server
                            "http://localhost:3000",   // fallback
                            "https://ecommerce-dashboard-one-tawny.vercel.app",
                            "https://ecommerce-dashboard-sefffo.vercel.app",
                            "https://ecommerce-dashboard-git-main-saifs-projects-6c87b55a.vercel.app"
                        )
                        .SetIsOriginMatching(origin =>
                            origin.StartsWith("http://localhost") ||
                            origin.EndsWith(".vercel.app")
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });



            //that's handle the request to match this headers 

            builder.Services.AddAuthentication(

                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;


                }




                ).AddCookie() //for the google login handshake
                .AddJwtBearer(options =>


                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                        ValidAudience = builder.Configuration["JwtOptions:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:securityKey"]!))
                    };



                }
                ).AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"]!;
                    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"]!;
                    options.CallbackPath = "/signin-google"; // must match Google Console exactly
                    options.SaveTokens = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });




            #endregion

            #region Registers



            //upload Service Registeration
            builder.Services.AddScoped<IUploadService, UploadService>();




            // Bind FawaterakSettings
            builder.Services.Configure<FawaterakSettings>(
                builder.Configuration.GetSection("FawaterakSettings"));

            // Register FawaterakService with managed HttpClient
            builder.Services.AddHttpClient<IFawaterakService, FawaterakService>();


            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContexts>()
            .AddSignInManager()           // required for external login (Google OAuth) to complete the handshake
            .AddDefaultTokenProviders();  // required for email confirmation / password reset tokens




            builder.Services.AddDbContext<ApplicationIdentityDbContexts>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("IdentityConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;

            });


            builder.Services.AddScoped<ICacheService, CacheService>();

            builder.Services.AddScoped<ICacheRepository, CacheRepository>();

            builder.Services.AddScoped<IBasketService, BasketService>();

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();


            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {

                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);

            });




            builder.Services.AddScoped<IProductService, ProductService>();


            //builder.Services.AddAutoMapper(x=>x.LicenseKey="",typeof(ProductPictureUrlResolver).Assembly);//registering the assembly where the resolver is located
            builder.Services.AddAutoMapper(typeof(ProductPictureUrlResolver).Assembly);
            builder.Services.AddTransient<ProductPictureUrlResolver>();

            //registering the resolver as transient because it does not maintain any state and is lightweight
            //whats the use of transient?
            //it creates a new instance of the service each time it is requested
            //this is useful for lightweight, stateless services
            builder.Services.AddAutoMapper(mp => mp
            .AddProfile<ProductProfile>()
            );




            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));


            #endregion
            // Add services to the container.




            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Ensure all API responses use camelCase (e.g. subTotal, userEmail, orderDate)
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            app.UseMiddleware<ExceptionHandlerMiddleWare>();

            app.UseCors("DashboardPolicy");

            #region Data Seeding

            //created a seperation of concerns by moving the migration and seeding logic to extension methods
            //open closed principle: we can add new functionality without modifying the existing code, we just need to create new extension methods for the new functionality and call them in the program.cs file
            //by adding any extention method we can enhance the WebApplication class without modifying its source code 

            await app.MigrateDatabaseAsync();

            await app.MigrateIdentityDatabaseAsync();


            await app.SeedIdentityDataAsync();

            await app.SeedDataAsync();

            #endregion



            app.UseHttpsRedirection();


            app.UseSwagger();
            app.UseSwaggerUI();



            app.MapStaticAssets();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
