using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWebModels.Seeding;
using MyWebModels.Models;
using MyWebModels.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWebModels.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace MyWebModels.Database
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            RoleSeed.Seed(builder);
            UserSeed.Seed(builder);
            UserRoleSeed.Seed(builder);
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Installment> Installments { get; set; }

    }
}
