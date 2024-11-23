using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Apis.Extentions;

namespace ECommerce.Apis
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.ConfigureApplicationServices(builder.Configuration);
            builder.Services.AddSwaggerDocumentation();
            builder.Services.ConfigureCors();
            builder.Services.ConfigureDatabaseAndJwt(builder.Configuration);

            var app = builder.Build();
            app.ConfigureMiddleware();

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();

        }
    }
}
