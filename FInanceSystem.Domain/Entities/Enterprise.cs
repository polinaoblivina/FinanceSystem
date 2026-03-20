using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Enterprise
    {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }

        private Enterprise() { }
        private Enterprise(Guid id, string name) 
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid enterprise id", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid enterprise name", nameof(name));

            Id = id;
            Name = name;
        }
        public static Enterprise CreateNew(string name)
        {
            return new Enterprise(Guid.NewGuid(), name);
        }

        public static Enterprise Restore(Guid id, string name)
        {
            return new Enterprise(id, name);
        }
    } 
}
