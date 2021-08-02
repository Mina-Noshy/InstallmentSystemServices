using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class UpdateUserPasswordVM
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
