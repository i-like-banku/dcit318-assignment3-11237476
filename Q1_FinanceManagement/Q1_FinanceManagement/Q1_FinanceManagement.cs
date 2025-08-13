using System;
using System.Collections.Generic;

namespace Q1_FinanceManagement
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction) =>
            Console.WriteLine($"[Bank Transfer] {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction) =>
            Console.WriteLine($"[Mobile Money] {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction) =>
            Console.WriteLine($"[Crypto Wallet] {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Balance: {Balance:C}");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
                Console.WriteLine("Insufficient funds!");
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"New balance: {Balance:C}");
            }
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.Write("Enter Account Number: ");
            string acctNum = Console.ReadLine() ?? string.Empty;

            decimal initBal;
            while (true)
            {
                Console.Write("Enter Initial Balance: ");
                if (decimal.TryParse(Console.ReadLine(), out initBal)) break;
                Console.WriteLine("Invalid input. Please enter a valid decimal number.");
            }

            var account = new SavingsAccount(acctNum, initBal);

            string[] categories = { "Groceries", "Utilities", "Entertainment" };

            for (int i = 0; i < categories.Length; i++)
            {
                decimal amount;
                while (true)
                {
                    Console.Write($"Enter amount for {categories[i]}: ");
                    if (decimal.TryParse(Console.ReadLine(), out amount)) break;
                    Console.WriteLine("Invalid input. Please enter a valid decimal number.");
                }

                var transaction = new Transaction(i + 1, DateTime.Now, amount, categories[i]);

                ITransactionProcessor processor = i switch
                {
                    0 => new MobileMoneyProcessor(),
                    1 => new BankTransferProcessor(),
                    _ => new CryptoWalletProcessor()
                };

                processor.Process(transaction);
                account.ApplyTransaction(transaction);
                _transactions.Add(transaction);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
