using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyWebModels.Models
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public double OriginalAmount { get; set; }

        [Range(1, 100)]
        [Required]
        public int Percentage { get; set; }

        [Required]
        public double TotalAmount { get; set; }

        [Required]
        public double AmountPaid { get; set; }

        [Required]
        public double RestAmount { get; set; }

        [Range(1, 100)]
        [Required]
        public int InstallmentCount { get; set; } // عدد الاقساط

        [Required]
        public double DelayFine { get; set; } = 0; // غرامه تاخير

        [Required]
        public InstallmentTypes DelayFineType { get; set; }

        [Required]
        public InstallmentTypes InstallmentType { get; set; }

        [Required]
        public DateTime BillDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Description { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual Client GetClient { get; set; }

        public virtual ICollection<Installment> GetInstallments { get; set; }
    }

    public enum InstallmentTypes
    {
        سنوى,
        شهرى,
        يومى
    }

}
