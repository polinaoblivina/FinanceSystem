using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FInanceSystem.Domain.Entities
{
    public abstract class User
    {
        public Guid Id { get; protected set; }
        public string Login { get; protected set; }
        public string Password { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        protected User(Guid id, string login, string password, string firstName, string lastName)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user id", nameof(id));

            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Invalid login", nameof(login));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Invalid password", nameof(password));

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Invalid first name", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Invalid last name", nameof(lastName));

            Id = id;
            Login = login;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
