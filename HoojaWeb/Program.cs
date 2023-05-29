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
            // S�tter upp s�kv�gen till loggfilen
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "applog.txt");

            // Konfigurerar Serilog f�r att skriva till loggfilen
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // L�gger till tj�nster i beh�llaren
                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                // Konfigurerar HTTP-anropspipelinen
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // Standardv�rdet f�r HSTS �r 30 dagar. Du kan �ndra detta f�r produktionsscenarier, se https://aka.ms/aspnetcore-hsts.
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
                // Loggar allvarliga fel n�r v�rdet avslutas ov�ntat
                Log.Fatal(ex, "V�rd avslutades ov�ntat");
            }
            finally
            {
                // St�nger loggen och skriver eventuella kvarvarande loggevent
                Log.CloseAndFlush();
            }
        }
    }
}
