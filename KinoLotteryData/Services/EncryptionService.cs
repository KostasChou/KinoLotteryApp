using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KinoLotteryData.Services
{
    public class EncryptionService
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA256(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return storedHash.SequenceEqual(computedHash);
        }
    }
}
