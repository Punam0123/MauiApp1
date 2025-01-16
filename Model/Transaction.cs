using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    public int Id { get; set; }
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Tags { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Debt { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string TransactionType { get; set; } // e.g., "Cash Inflow", "Expense", etc.
}
