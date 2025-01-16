using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MauiApp1.Models;

public class UserService
{
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
    private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");

    // Load AppData (Users, Transactions, Debts) from JSON file
    private int? _currentUserId;
    public Task SetCurrentUserIdAsync(int userId)
    {
        _currentUserId = userId;
        return Task.CompletedTask;
    }

    public Task<int> GetCurrentUserIdAsync()
    {
        if (_currentUserId.HasValue)
        {
            return Task.FromResult(_currentUserId.Value);
        }
        return Task.FromResult(0); // Return 0 if no user is logged in
    }

    public AppData LoadData()
    {
        if (!File.Exists(FilePath))
        {
            SaveData(new AppData());  // Create an empty file with default data
            return new AppData();
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
        }
        catch
        {
            return new AppData(); // Handle corrupted JSON or other errors gracefully
        }
    }

    // Save AppData (Users, Transactions, Debts) to JSON file
    public void SaveData(AppData data)
    {
        EnsureFolderExists();
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    // User Management
    public List<User> LoadUsers()
    {
        var appData = LoadData();
        return appData.Users;
    }

    public void SaveUsers(List<User> users)
    {
        var appData = LoadData();
        appData.Users = users;
        SaveData(appData);
    }

    public void AddUser(User user)
    {
        var appData = LoadData();
        appData.Users.Add(user);
        SaveData(appData);
    }

    public void RemoveUser(int userId)
    {
        var appData = LoadData();
        var user = appData.Users.FirstOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            appData.Users.Remove(user);
            SaveData(appData);
        }
    }

    public int RegisterUser(string username, string password, string email, string fullName, string contactNumber)
    {
        var appData = LoadData();

        // Check if the user already exists
        if (appData.Users.Any(u => u.Username == username))
        {
            throw new Exception("User already exists.");
        }

        // Create a new user with a unique UserId
        var newUserId = appData.Users.Count > 0 ? appData.Users.Max(u => u.UserId) + 1 : 1;

        var newUser = new User
        {
            UserId = newUserId,
            Username = username,
            Password = HashPassword(password),
            Email = email,
            FullName = fullName,  // Set the FullName property
            ContactNumber = contactNumber  // Set the ContactNumber property
        };

        // Add the new user to the list
        appData.Users.Add(newUser);
        SaveData(appData);

        return newUserId;
    }



    // Debt Management
    public List<Debt> LoadDebts()
    {
        var appData = LoadData();
        return appData.Debts;
    }

    public void SaveDebts(List<Debt> debts)
    {
        var appData = LoadData();
        appData.Debts = debts;
        SaveData(appData);
    }

    public void AddDebt(Debt debt)
    {
        var appData = LoadData();
        appData.Debts.Add(debt);
        SaveData(appData);
    }

    public void RemoveDebt(int debtId)
    {
        var appData = LoadData();
        var debt = appData.Debts.FirstOrDefault(d => d.Id == debtId);
        if (debt != null)
        {
            appData.Debts.Remove(debt);
            SaveData(appData);
        }
    }

    public List<Debt> GetDebtsByUserId(int userId)
    {
        var appData = LoadData();
        return appData.Debts.Where(d => d.UserId == userId).ToList();
    }

    // Transaction Management
    public void AddTransaction(Transaction transaction)
    {
        var appData = LoadData();
        appData.Transactions.Add(transaction);
        SaveData(appData);
    }

    public void RemoveTransaction(int transactionId)
    {
        var appData = LoadData();
        var transaction = appData.Transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction != null)
        {
            appData.Transactions.Remove(transaction);
            SaveData(appData);
        }
    }

    public decimal GetTotalBalanceByUserId(int userId)
{
    var appData = LoadData();
    var transactions = appData.Transactions.Where(t => t.UserId == userId);
    var totalCredits = transactions.Sum(t => t.Credit);
    var totalDebits = transactions.Sum(t => t.Debit);
    return totalCredits - totalDebits;
}


    public void UpdateTransaction(Transaction updatedTransaction)
    {
        var appData = LoadData();
        var transaction = appData.Transactions.FirstOrDefault(t => t.Id == updatedTransaction.Id);
        if (transaction != null)
        {
            transaction.Title = updatedTransaction.Title;
            transaction.Tags = updatedTransaction.Tags;
            transaction.Debit = updatedTransaction.Debit;
            transaction.Credit = updatedTransaction.Credit;
            transaction.Date = updatedTransaction.Date;
            transaction.Description = updatedTransaction.Description;
            transaction.UserId = updatedTransaction.UserId;
            SaveData(appData);
        }
    }

    public void AddCashInflow(int userId, decimal amount, string title, string description, string tags)
    {
        var appData = LoadData();

        // Create a new transaction
        var transaction = new Transaction
        {
            UserId = userId,
            Title = title,
            Credit = amount,
            Date = DateTime.Now,
            Description = description,
            Tags = tags,
            TransactionType = "Cash Inflow"
        };

        // Add transaction and save
        appData.Transactions.Add(transaction);
        SaveData(appData);
    }

    public async Task<bool> HasOutstandingDebtAsync(int userId)
    {
        var appData = LoadData();
        var userDebts = appData.Debts.Where(d => d.UserId == userId).ToList();

        // Check if there is any debt where the paid amount is less than the total amount
        return userDebts.Any(d => d.PaidAmount < d.Amount);
    }

    public async Task<decimal> GetTotalDebtAmountAsync(int userId)
    {
        var appData = LoadData();
        var userDebts = appData.Debts.Where(d => d.UserId == userId);

        // Sum the total amount (not just the unpaid portion)
        var totalDebt = userDebts.Sum(d => d.Amount);

        return await Task.FromResult(totalDebt);
    }


    public async Task AddDebtAsync(Debt debt)
    {
        var appData = LoadData();
        appData.Debts.Add(debt);
        SaveData(appData);
        await Task.CompletedTask; // Simulate async behavior for consistency
    }


    private void EnsureFolderExists()
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
    }

    // Password hashing and validation
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedPassword;
    }
}
