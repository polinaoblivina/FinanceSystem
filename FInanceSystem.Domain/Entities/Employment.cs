using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Employment
    {
        public Guid Id { get; private set; }
        public Guid ClientId { get; private set; }
        public Guid EnterpriseId { get; private set; }

        private Employment() { }
        private Employment(Guid id, Guid clientId, Guid enterpriseId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid employment id", nameof(id));
            if (clientId == Guid.Empty)
                throw new ArgumentException("Invalid client id", nameof(clientId));
            if (enterpriseId == Guid.Empty)
                throw new ArgumentException("Invalid enterprise id", nameof(enterpriseId));

            Id = id;
            ClientId = clientId;
            EnterpriseId = enterpriseId;
        }
        public static Employment CreateNew(Guid clientId, Guid enterpriseId)
        {
            return new Employment(Guid.NewGuid(), clientId, enterpriseId);
        }

        public static Employment Restore(Guid id, Guid clientId, Guid enterpriseId)
        {
            return new Employment(id, clientId, enterpriseId);
        }
    }
}
