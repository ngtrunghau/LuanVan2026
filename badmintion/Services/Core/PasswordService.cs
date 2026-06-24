using badmintion.Interface.Core;
using System.Security.Cryptography;

namespace badmintion.Services.Core
{
    public class PasswordService : IPasswordService
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 210_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public string Hash(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return string.Join(
                '$',
                Prefix,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        public bool Verify(string password, string? storedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            if (!storedPassword.StartsWith($"{Prefix}$", StringComparison.Ordinal))
            {
                return CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(password),
                    System.Text.Encoding.UTF8.GetBytes(storedPassword));
            }

            var parts = storedPassword.Split('$');
            if (parts.Length != 4 ||
                !int.TryParse(parts[1], out var iterations) ||
                iterations <= 0)
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expectedHash = Convert.FromBase64String(parts[3]);
                var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool NeedsRehash(string? storedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword) ||
                !storedPassword.StartsWith($"{Prefix}$", StringComparison.Ordinal))
            {
                return true;
            }

            var parts = storedPassword.Split('$');
            return parts.Length != 4 ||
                   !int.TryParse(parts[1], out var iterations) ||
                   iterations < Iterations;
        }
    }
}
