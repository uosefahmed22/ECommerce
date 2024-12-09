using ECommerce.Apis.Helpers;
using ECommerce.Core.IRepositories;
using ECommerce.Core.IServices;
using ECommerce.Core.Models.Auth;
using ECommerce.Repository.Data;
using ECommerce.Repository.Repositories;
using ECommerce.Repository.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Stripe;
using CheckoutService = ECommerce.Repository.Services.CheckoutService;

namespace ECommerce.Apis.Extentions
{
    public static class ApplicationServiceExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity configuration
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>();

            // Dependency injection
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IFavoriteWishlistRepository, FavoriteWishlistRepository>();

            // Cloudinary configuration
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySetting"));
            services.AddSingleton(cloudinary =>
            {
                var config = configuration.GetSection("CloudinarySetting").Get<CloudinarySettings>();
                var account = new CloudinaryDotNet.Account(config.CloudName, config.ApiKey, config.ApiSecret);
                return new CloudinaryDotNet.Cloudinary(account);
            });
            
            //SMTPEmail configuration
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            // Additional services
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(MappingProfile));


            //Stripe Configuration
            services.AddControllersWithViews();
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            StripeConfiguration.ApiKey = configuration["StripeSettings:Secretkey"];
            services.AddLogging();

            // Redis Configuration
            var redisConfiguration = configuration.GetValue<string>("Redis:ConnectionString");

            if (string.IsNullOrEmpty(redisConfiguration))
            {
                throw new ArgumentNullException("Redis:ConnectionString", "Redis connection string is not configured.");
            }
            var redis = ConnectionMultiplexer.Connect(redisConfiguration);
            services.AddSingleton<IConnectionMultiplexer>(redis);

            // API behavior customization
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            ErrorMessages = e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                        }).ToArray();

                    var result = new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(result);
                };
            });
        }
    }
}