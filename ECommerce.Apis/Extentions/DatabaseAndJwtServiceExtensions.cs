using ECommerce.Core.Models.Auth;
using ECommerce.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json;

namespace ECommerce.Apis.Extentions
{
    public static class DatabaseAndJwtServiceExtensions
    {
        public static void ConfigureDatabaseAndJwt(this IServiceCollection services, IConfiguration configuration)
        {
            // Database configuration
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // JWT configuration
            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero
            };

            // Add JWT authentication
            services.AddSingleton(tokenValidationParameters);
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            // This For JWT Authentication to make sure that the user is authorized to access the resource
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(jwt =>
           {
               jwt.SaveToken = true;
               jwt.TokenValidationParameters = tokenValidationParameters;
               jwt.Events = new JwtBearerEvents
               {
                   OnChallenge = context =>
                   {
                       context.HandleResponse();
                       context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                       context.Response.ContentType = "application/json";
                       var result = JsonSerializer.Serialize(new
                       {
                           StatusCode = StatusCodes.Status401Unauthorized,
                           Message = "You are not authorized to access this resource."
                       });
                       return context.Response.WriteAsync(result);
                   },
                   OnForbidden = context =>
                   {
                       context.Response.StatusCode = StatusCodes.Status403Forbidden;
                       context.Response.ContentType = "application/json";
                       var result = JsonSerializer.Serialize(new
                       {
                           StatusCode = StatusCodes.Status403Forbidden,
                           Message = "You do not have permission to access this resource."
                       });
                       return context.Response.WriteAsync(result);
                   }
               };
           });
        }
    }
}