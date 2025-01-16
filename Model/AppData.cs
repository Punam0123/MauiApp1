namespace MauiApp1.Models
{
    public class AppData
    {
        // The next available transaction ID
        public List<User> Users { get; set; } = new List<User>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Debt> Debts { get; set; } = new List<Debt>();
    }
}
