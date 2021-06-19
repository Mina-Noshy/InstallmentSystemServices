using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebModels.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebModels.Seeding
{
    public static class UserSeed
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasData(

                new AppUser()
                {
                    Id = "6510262c-bbcb-4629-b1e7-20de05ef7ae6",
                    FirstName = "Mina",
                    LastName = "Noshy",
                    UserName = "mina-noshy@outlook.com",
                    Email = "mina-noshy@outlook.com",
                    PhoneNumber = "01111257052",
                    NormalizedEmail = "MINA-NOSHY@OUTLOOK.COM",
                    NormalizedUserName = "MINA-NOSHY@OUTLOOK.COM",
                    LockoutEnabled = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    PasswordHash = Hasher("666666")
                },
                new AppUser()
                {
                    Id = "efcf6d9d-0550-49e4-9dcb-51d232223053",
                    FirstName = "Mina",
                    LastName = "Noshy",
                    UserName = "eastaria@outlook.com",
                    Email = "eastaria@outlook.com",
                    PhoneNumber = "01210165412",
                    NormalizedEmail = "EASTARIA@OUTLOOK.COM",
                    NormalizedUserName = "EASTARIA@OUTLOOK.COM",
                    LockoutEnabled = true,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    PasswordHash = Hasher("666666")
                });
        }


        private static string Hasher(string password)
        {
            var passwordHash = new PasswordHasher<AppUser>();
            return passwordHash.HashPassword(null, password);
        }


        //private static string PasswordGenerator(AppUser user, string password)
        //{
        //    var passwordHash = new PasswordHasher<AppUser>();
        //    return passwordHash.HashPassword(user, password);
        //}
    }
}
