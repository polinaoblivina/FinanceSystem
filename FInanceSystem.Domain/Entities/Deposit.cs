using FInanceSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using FInanceSystem.Domain.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FInanceSystem.Domain.Entities
{
    public class Deposit
    {
        public Guid Id { get; private set; }
        public string? Number { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid BankId { get; private set; }
        public decimal Balance { get; private set; }
        public double InterestRate { get; private set; }
        public DepositType Type { get; private set; }
        public DepositStatus Status { get; private set; }
        public DateTime LastInterestDate { get; private set; }

        private Deposit() { }
        private Deposit(Guid id, string number, Guid clientId, Guid bankId, decimal balance, double interestRate, DepositType depositType, DepositStatus status, DateTime lastInterestDate)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid deposite id", nameof(id));
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Invalid deposit number", nameof(number));
            if (clientId == Guid.Empty)
                throw new ArgumentException("Invalid client id", nameof(clientId));
            if (bankId == Guid.Empty)
                throw new ArgumentException("Invalid bank id", nameof(bankId));
            if (balance < 0)
                throw new ArgumentException("Invalid amount", nameof(balance));
            if (interestRate < 0)
                throw new ArgumentException("Invalid interest rate", nameof(interestRate));

            Id = id;
            Number = number;
            ClientId = clientId;
            BankId = bankId;
            Balance = balance;
            InterestRate = interestRate;
            Type = depositType;
            Status = status;
            LastInterestDate = lastInterestDate;
        }
        public static Deposit CreateNew(Guid clientId, Guid bankId, decimal balance, double interestRate, DepositType type)
        {
            return new Deposit(Guid.NewGuid(), FInanceSystem.Domain.Utils.Utils.GenerateDepositNumber(), clientId, bankId, balance, interestRate, type, DepositStatus.Active, DateTime.UtcNow);
        }

        public static Deposit Restore(Guid id, string number, Guid clientId, Guid bankId, decimal balance, double interestRate, DepositType depositType, DepositStatus status, DateTime lastInterestDate)
        {
            return new Deposit(id, number, clientId, bankId, balance, interestRate, depositType, status, lastInterestDate);
        }

        public void Close()
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Only active deposits can be closed");
            AccrueInterest();

            Status = DepositStatus.Closed;
        }
        public void Block()
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Only active deposits can be blocked.");
            Status = DepositStatus.Blocked;
        }
        public void Unblock()
        {
            if (Status != DepositStatus.Blocked)
                throw new InvalidOperationException("Only blocked deposits can be unblocked.");
            Status = DepositStatus.Active;
        }
        public void IncreaseBalance(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Invalid amount", nameof(amount));
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Deposit not active");
            if (Type != DepositType.OpenDeposit)
                throw new InvalidOperationException("This deposit does not allow replenishment");

            Balance += amount;
        }
        public void DecreaseBalance(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Invalid amount", nameof(amount));
            if (Balance < amount)
                throw new InvalidOperationException("Balance is less than the amount");
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Deposit not active");
            if (Type != DepositType.OpenDeposit)
                throw new InvalidOperationException("Withdrawal not allowed");

            Balance -= amount;
        }

        public void AccrueInterest()
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Cannot accrue interest on non-active deposit");
            if (Balance <= 0)
                throw new InvalidOperationException("Cannot accrue interest on zero or negative balance");

            var days = (DateTime.UtcNow - LastInterestDate).Days;

            if (days <= 0)
                throw new InvalidOperationException("Interest already accrued");

            decimal dailyRate = (decimal)InterestRate / 365m;
            decimal interest = Balance * dailyRate * days;

            Balance += interest;
            LastInterestDate = DateTime.UtcNow;
        }
    }
}
