using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public class Manager : User
    {
        private Manager(Guid id, string login, string password, string firstName, string lastName)
       : base(id, login, password, firstName, lastName)
        { }

        protected Manager() { }
        public static Manager CreateNew(string login, string password, string firstName, string lastName)
        {
            return new Manager(Guid.NewGuid(), login, password, firstName, lastName);
        }
        public static Manager Restore(Guid id, string login, string password, string firstName, string lastName)
        {
            return new Manager(id, login, password, firstName, lastName);
        }
    }
}
