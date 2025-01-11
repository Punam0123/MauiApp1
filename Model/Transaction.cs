

using System;
using System.ComponentModel.DataAnnotations;

namespace MauiApp1.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; }
        public string Tags { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public decimal Debt { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}

