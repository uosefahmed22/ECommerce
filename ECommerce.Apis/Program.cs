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
            app.UseSwagger();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }
            app.ConfigureMiddleware();

            app.MapControllers();
            app.Run();
        }
    }
}
