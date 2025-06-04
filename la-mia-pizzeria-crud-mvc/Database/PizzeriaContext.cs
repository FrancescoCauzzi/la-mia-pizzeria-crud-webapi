using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace la_mia_pizzeria_crud_mvc.Database
{
    public class PizzeriaContext : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration _configuration;

        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<PizzaCategory> PizzaCategories { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public PizzeriaContext(DbContextOptions<PizzeriaContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("PizzeriaContextConnection"));
            }
        }

    }
}
