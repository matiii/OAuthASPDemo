using System;
using System.Security.Cryptography;

namespace OAuth.WebApi.Extensions
{
    public static class HashExtensions
    {
        public static string ToHash(this string value)
        {
            var hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(value);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}