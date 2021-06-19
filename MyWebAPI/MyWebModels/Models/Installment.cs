using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyWebModels.Models
{
    public class Installment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BillId { get; set; }

        [Required]
        public double AmountValue { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReceivedDate { get; set; }

        [ForeignKey(nameof(BillId))]
        public virtual Bill GetBill { get; set; }
    }
}
