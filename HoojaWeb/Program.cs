
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
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
            var builder = WebApplication.CreateBuilder(args);

            DotNetEnv.Env.Load();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "AuthSession";
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "Authenticated";
                    options.LoginPath = "/login/index";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
                    options.Cookie.IsEssential = true;
                });
            var app = builder.Build();
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
                app.UseSession();
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
