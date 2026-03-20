using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Admin : User
    {
        private Admin(Guid id, string login, string password, string firstName, string lastName)
        : base(id, login, password, firstName, lastName)
        {}

        public static Admin CreateNew(string login, string password, string firstName, string lastName)
        {
            return new Admin(Guid.NewGuid(), login, password, firstName, lastName);
        }
        public static Admin Restore(Guid id, string login, string password, string firstName, string lastName)
        {
            return new Admin(id, login, password, firstName, lastName);
        }
    }
}
