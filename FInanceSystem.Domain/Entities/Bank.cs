using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Bank
    {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }

        private Bank() { }
        private Bank(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid bank id", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name", nameof(name));

            Id = id;
            Name = name;
        }
        public static Bank CreateNew(string name)
        {
            return new Bank(Guid.NewGuid(), name);
        }

        public static Bank Restore(Guid id, string name)
        {
            return new Bank(id, name);
        }
    }
}
