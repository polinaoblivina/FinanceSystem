using FInanceSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid FromId { get; private set; }
        public Guid ToId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime Timestamp { get; private set; }

        private Transaction() { }
        private Transaction(Guid id, Guid fromId, Guid toId, decimal amount, TransactionType type, DateTime timestamp)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid transaction id", nameof(id));

            if (toId == Guid.Empty)
                throw new ArgumentException("Invalid receiver id", nameof(toId));

            if (amount <= 0)
                throw new ArgumentException("Invalid amount", nameof(amount));

            if (fromId == Guid.Empty)
                throw new ArgumentException("Invalid sender id", nameof(fromId));


            Id = id;
            FromId = fromId;
            ToId = toId;
            Amount = amount;
            Type = type;
            Timestamp = timestamp;
        }

        public static Transaction CreateNew(Guid fromId, Guid toId, decimal amount, TransactionType type)
        {
            return new Transaction(Guid.NewGuid(), fromId,  toId, amount, type, DateTime.UtcNow);
        }

        public static Transaction Restore(Guid id, Guid fromId, Guid toId, decimal amount,  TransactionType type, DateTime timestamp)
        {
            return new Transaction(id, fromId,toId, amount, type, timestamp);
        }
    }
}
