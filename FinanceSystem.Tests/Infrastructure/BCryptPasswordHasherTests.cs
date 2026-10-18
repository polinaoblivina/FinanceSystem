using FinanceSystem.Infrastructure.Security;

namespace FinanceSystem.Tests.Infrastructure
{
    public class BCryptPasswordHasherTests
    {
        private readonly BCryptPasswordHasher _hasher = new();

        [Fact]
        public void Hash_DoesNotReturnPlainPassword()
        {
            var hash = _hasher.Hash("secret");

            Assert.NotEqual("secret", hash);
            Assert.StartsWith("$2", hash);
        }

        [Fact]
        public void Hash_SamePasswordTwice_ProducesDifferentHashes()
        {
            Assert.NotEqual(_hasher.Hash("secret"), _hasher.Hash("secret"));
        }

        [Fact]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            var hash = _hasher.Hash("secret");

            Assert.True(_hasher.Verify("secret", hash));
        }

        [Fact]
        public void Verify_WrongPassword_ReturnsFalse()
        {
            var hash = _hasher.Hash("secret");

            Assert.False(_hasher.Verify("wrong", hash));
        }

        [Fact]
        public void Verify_NotABcryptHash_ReturnsFalse()
        {
            Assert.False(_hasher.Verify("secret", "secret"));
        }
    }
}
