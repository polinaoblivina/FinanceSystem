using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Client : User
    {
        public bool IsApprovedByManager { get; private set; }
        protected Client() { }
        private Client(Guid id, string login, string password, string firstName, string lastName, bool approved)
            : base(id, login, password, firstName, lastName)
        {
            IsApprovedByManager = approved;
        }
        public static Client CreateNew(string login, string password, string firstName, string lastName)
        {
            return new Client(Guid.NewGuid(), login, password, firstName, lastName, false);
        }
        public static Client Restore(Guid id, string login, string password, string firstName, string lastName, bool approved)
        {
            return new Client(id, login, password, firstName, lastName, approved);
        }
        public void Approve()
        {
            if (IsApprovedByManager)
                throw new InvalidOperationException("Client already approved");

            IsApprovedByManager = true;
        }

        public void RevokeApproval()
        {
            if (!IsApprovedByManager)
                throw new InvalidOperationException("Client is not approved");

            IsApprovedByManager = false;
        }
    }
}
