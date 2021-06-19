using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class SendEmailVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email Content")]
        public string EmailContent { get; set; }
    }
}
