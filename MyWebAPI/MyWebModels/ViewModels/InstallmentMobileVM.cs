using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class InstallmentMobileVM
    {
        public int id { get; set; }
        public string client { get; set; }
        public double amountValue { get; set; }
        public DateTime dueDate { get; set; }
        public DateTime? receivedDate { get; set; }
    }
}
