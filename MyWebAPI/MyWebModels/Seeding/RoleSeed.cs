using Microsoft.EntityFrameworkCore;
using MyWebModels.Models.Account;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebModels.Seeding
{
    public static class RoleSeed
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<AppRole>().HasData(
                new AppRole() { Id = "57784dee-54ff-4115-9835-da06239d6117", Name = RoleVM.Admin, NormalizedName = RoleVM.Admin.ToUpper() },
                new AppRole() { Id = "93c4a412-3af5-49f8-9b27-cecc7b6f6e79", Name = RoleVM.Moderator, NormalizedName = RoleVM.Moderator.ToUpper() },
                new AppRole() { Id = "33c4a411-4af6-49f7-9b44-eac6ae423e23", Name = RoleVM.User, NormalizedName = RoleVM.User.ToUpper() }
                );
        }
    }
}
