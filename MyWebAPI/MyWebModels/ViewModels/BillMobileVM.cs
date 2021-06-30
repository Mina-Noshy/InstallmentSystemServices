using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class BillMobileVM
    {
        public int id { get; set; }
        public string client { get; set; }
        public double originalAmount { get; set; }
        public int percentage { get; set; }
        public double totalAmount { get; set; }
        public double amountPaid { get; set; }
        public double restAmount { get; set; }
    }
}
