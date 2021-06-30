using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class ClientVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone_1 { get; set; }
        public string phone_2 { get; set; }
        public string phone_3 { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
    }
}
