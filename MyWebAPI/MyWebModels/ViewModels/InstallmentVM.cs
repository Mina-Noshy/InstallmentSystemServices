using MyWebModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebModels.ViewModels
{
    public class InstallmentVM
    {
        public int id { get; set; }
        public int billId { get; set; }
        public string clientName { get; set; }
        public double amountValue { get; set; }
        public DateTime dueDate { get; set; }
        public DateTime? receivedDate { get; set; }
        public string billDescription { get; set; }
        public double delayFine { get; set; }

        public InstallmentTypes delayFineType { get; set; }

        public InstallmentTypes installmentType { get; set; }
    }
}
