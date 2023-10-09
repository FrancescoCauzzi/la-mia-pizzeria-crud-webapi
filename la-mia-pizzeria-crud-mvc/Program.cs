using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace la_mia_pizzeria_crud_mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("PizzeriaContextConnection") ?? throw new InvalidOperationException("Connection string 'PizzeriaContextConnection' not found.");

            builder.Services.AddDbContext<PizzeriaContext>();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<PizzeriaContext>();



            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // this setting helps in avoiding infinite loops during JSON serialization when there are circular references in your data structures, which is a common scenario when you have related entities in your models. (N:N, 1:N relationships)
            builder.Services.AddControllers().AddJsonOptions(x =>
                            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Dependency Injection File Logger, when a generic controller expects an object that implement an ICustomLogger interface I give it my CustomFileLogger
            builder.Services.AddScoped<ICustomLogger, CustomFileLogger>();
            // Dependency Injection DatabaseContext, whenever a generic controller expects an objects called PizzeriaContext I give it my PizzeriaContext, so that the object controller will be initialized with the PizzeriaContext (and all its related properties and methods) automatically
            builder.Services.AddScoped<PizzeriaContext, PizzeriaContext>();

            var app = builder.Build();

            var defaultDateCulture = "en-US";
            var ci = new CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo>
            {
                ci,
            },
                SupportedUICultures = new List<CultureInfo>
            {
                ci,
            }
            });
            
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

            // authentication line in our setup
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "PizzasIndex",
                pattern: "Our-Pizzas",
                defaults: new { controller = "Pizza", action = "Index" });

            app.MapControllerRoute(
                name: "PizzaDetails",
                pattern: "Our-Pizzas/{*name}",
                defaults: new { controller = "Pizza", action = "Details" });

            
            app.MapControllerRoute(
                name: "PizzaEdit",
                pattern: "Edit/{*name}",
                defaults: new { controller = "Pizza", action = "Update" });
            

            /*
            app.MapControllerRoute(
                name: "PizzaCreate",
                pattern: "Our-Pizzas/Create",
                defaults: new { controller = "Pizza", action = "Create" });
            */

            // Down here remember to change for the final version
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Pizza}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}