using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebModels.Sittings
{
    public class MailConfiguration
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
