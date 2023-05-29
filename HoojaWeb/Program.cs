using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HoojaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Sätter upp sökvägen till loggfilen
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "applog.txt");

            // Konfigurerar Serilog för att skriva till loggfilen
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Lägger till tjänster i behållaren
                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                // Konfigurerar HTTP-anropspipelinen
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // Standardvärdet för HSTS är 30 dagar. Du kan ändra detta för produktionsscenarier, se https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                // Loggar allvarliga fel när värdet avslutas oväntat
                Log.Fatal(ex, "Värd avslutades oväntat");
            }
            finally
            {
                // Stänger loggen och skriver eventuella kvarvarande loggevent
                Log.CloseAndFlush();
            }
        }
    }
}
