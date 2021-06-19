using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Services.Helper
{
    public static class EastariaHelper
    {
        // values
        public static string BaseEmail = "mina-noshy@outlook.com";





        // functions
        public static string GetResponceStatus(string message)
        {
            if (message.ToUpper().Contains("SUCCESS"))
                return "Success";
            else if (message.ToUpper().Contains("ERROR"))
                return "Error";
            else
                return "Warning";
        }
    }
}
