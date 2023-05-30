using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace HoojaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // Set up the path to the log file
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "applog.txt");

            // Configure Serilog to write to the log file
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                // Configure HTTP request pipeline
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default value for HSTS is 30 days. You can change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();
                app.UseSession();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                // Log serious errors when the host is unexpectedly terminated
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Close the log and flush any remaining log events
                Log.CloseAndFlush();
            }
        }
    }
}
