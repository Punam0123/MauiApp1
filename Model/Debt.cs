using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class Debt
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
