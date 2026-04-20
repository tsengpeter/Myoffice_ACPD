using Microsoft.EntityFrameworkCore;
using myofficeacpd.Data;
using myofficeacpd.Interfaces;
using myofficeacpd.Services;

namespace myofficeacpd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MyofficeAcpdDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Myoffice_ACPD")));

            builder.Services.AddScoped<IMyofficeacpdService, MyofficeacpdService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Myoffice ACPD API", Version = "v1" });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Myoffice ACPD API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
