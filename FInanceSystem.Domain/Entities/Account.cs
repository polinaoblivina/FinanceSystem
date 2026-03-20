using FInanceSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid BankId { get; private set; }
        public decimal Balance { get; private set; }
        public AccountStatus Status { get; private set; }

        private Account(Guid id, Guid clientId, Guid bankId, decimal balance, AccountStatus status)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid account id", nameof(id));
            if (clientId == Guid.Empty)
                throw new ArgumentException("Invalid client id", nameof(clientId));
            if (bankId == Guid.Empty)
                throw new ArgumentException("Invalid bank id", nameof(bankId));
            if (balance < 0)
                throw new ArgumentException("Invalid balance",nameof(balance));

            Id = id;
            ClientId = clientId;
            BankId = bankId;
            Balance = balance;
            Status = status;
        }
        public static Account CreateNew(Guid clientId, Guid bankId)
        {
            return new Account(Guid.NewGuid(), clientId, bankId, 0, AccountStatus.Active);
        }

        public static Account Restore(Guid id, Guid clientId, Guid bankId, decimal balance, AccountStatus status)
        {
            return new Account(id, clientId, bankId, balance, status);
        }
        public void Close()
        {
            if(Status != AccountStatus.Active)
                throw new InvalidOperationException("Cannot close a non-active account");
            if (Balance > 0)
                throw new InvalidOperationException("Cannot close a non-empty account");
            Status = AccountStatus.Closed;
        }
        public void Block()
        {
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Cannot block this account");
            Status = AccountStatus.Blocked;
        }
        public void Unblock()
        {
            if (Status != AccountStatus.Blocked)
                throw new InvalidOperationException("Only blocked accounts can be unblocked");

            Status = AccountStatus.Active;
        }

        public void IncreaseBalance(decimal amount)
        {
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Account is not active");
            if (amount <= 0)
                throw new ArgumentException("Invalid amount", nameof(amount));

            Balance += amount;
        }

        public void DecreaseBalance(decimal amount)
        {
            if (Status != AccountStatus.Active)
                throw new InvalidOperationException("Account is not active");
            if (amount <= 0)
                throw new ArgumentException("Invalid amount", nameof(amount));
            if (Balance < amount)
                throw new InvalidOperationException("Balance is less than the amount");

            Balance -= amount;
        }
    }
}
