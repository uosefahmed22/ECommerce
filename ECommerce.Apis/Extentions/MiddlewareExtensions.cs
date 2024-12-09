namespace ECommerce.Apis.Extentions
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce.Apis v1"));
            }

            // Enable middleware to serve the Swagger-UI (HTML, JS, CSS, etc.),
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("Open");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}