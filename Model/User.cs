using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string ContactNumber { get; set; }
        public string Currency { get; set; } = "NRP";
        public Guid Id { get; internal set; }
    }
}
