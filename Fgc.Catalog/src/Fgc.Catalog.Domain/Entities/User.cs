using Fgc.Catalog.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Fgc.Catalog.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public DateTime RegisteredAt { get; set; }

        private User() { }

        [SetsRequiredMembers]
        private User(string name, string email, DateTime registeredAt)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            RegisteredAt = registeredAt;
        }

        public static User Create(string name, string email, DateTime registeredAt)
        {
            Validate(name, email);
            return new User(name, email, registeredAt);
        }

        public void Update(string name, string email)
        {
            Validate(name, email);
            Name = name;
            Email = email;
        }

        private static void Validate(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new CatalogDomainException("Name is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new CatalogDomainException("Email is required.");
        }
    }
}
