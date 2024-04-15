using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using ShoppingWebsite.Hubs;
using System.Security.Claims;
using TimeTrackingSystem.Data;
using TimeTrackingSystem.Hubs;
using TimeTrackingSystem.Models;

namespace TimeTrackingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
            });


            //Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Authen/LoginGoogle";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/Clock/Index";
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Premium", policy => policy.RequireAssertion(context =>
                {
                    var role = context.User.FindFirstValue(ClaimTypes.Role);
                    return role == "2";
                }));
            });


            services.AddDbContext<SystemContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"))
                        .UseLazyLoadingProxies();
            });

            builder.Services.AddSignalR(o => o.MaximumReceiveMessageSize = 102400000);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
      
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<MeetingHubServer>("/server");

            app.MapHub<PaymentHubServer>("/paymentserver");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}