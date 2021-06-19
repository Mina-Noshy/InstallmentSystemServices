using Microsoft.AspNetCore.Identity;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebModels.Models.Account
{
    public class AppUser : IdentityUser
    {
        [MaxLength(20)]
        [RegularExpression(@"[a-zA-Z0-9\u0600-\u06FF ]+")]
        public string FirstName { get; set; }

        [MaxLength(20)]
        [RegularExpression(@"[a-zA-Z0-9\u0600-\u06FF ]+")]
        public string LastName { get; set; }

        public string PictureURL { get; set; }

        public List<RefreshTokenVM> RefreshTokens { get; set; }

        public virtual ICollection<Client> GetClients { get; set; }
    }

}
