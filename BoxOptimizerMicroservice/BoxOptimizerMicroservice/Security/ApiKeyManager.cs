using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace BoxOptimizerMicroservice.Security
{
    public static class ApiKeyManager
    {
        public static string GenerateNewApiKey(int keyByteLength = 32)
        {
            byte[] randomNumber = new byte[keyByteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return WebEncoders.Base64UrlEncode(randomNumber);
            }
        }

        public static string HashApiKey(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
                byte[] hashedBytes = sha256.ComputeHash(apiKeyBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}