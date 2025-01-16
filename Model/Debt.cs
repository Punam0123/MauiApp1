using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiApp1.Models
{
    public class Debt
    {
        public int Id { get; set; }  // Debt ID (auto-incremented)

        [ForeignKey("UserId")]
        public int UserId { get; set; }  // User ID associated with the debt
        public decimal Amount { get; set; }  // Amount of the debt
        public decimal PaidAmount { get; set; }  // Amount already paid
        public DateTime Date { get; set; }  // Date when the debt was created
        public string Description { get; set; }  // Description of the debt
    }
}
