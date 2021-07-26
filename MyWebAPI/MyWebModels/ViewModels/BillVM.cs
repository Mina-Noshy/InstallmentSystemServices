using MyWebModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class BillVM
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public string clientName { get; set; }
        public double originalAmount { get; set; }
        public double percentage { get; set; }
        public double totalAmount { get; set; }
        public double amountPaid { get; set; }
        public double restAmount { get; set; }
        public short installmentCount { get; set; } // عدد الاقساط
        public double delayFine { get; set; } = 0; // غرامه تاخير
        public InstallmentTypes delayFineType { get; set; }
        public InstallmentTypes installmentType { get; set; }
        public DateTime billDate { get; set; }
        public string description { get; set; }
    }
}
