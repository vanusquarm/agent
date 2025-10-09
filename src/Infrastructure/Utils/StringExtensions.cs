using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Utils
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Split(this string input, int chunkSize)
        {
            if (string.IsNullOrEmpty(input) || chunkSize <= 0)
                return Enumerable.Empty<string>();

            return input.Chunk(chunkSize).Select(chunk => new string(chunk));
        }
        public static string Split(this string input, int chunkSize, int chunkIndex)
        {
            return input.Split(chunkSize).Skip(chunkIndex - 1).FirstOrDefault() ?? string.Empty;
        }

        public static string ToHash(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var doc = JsonDocument.Parse(input);
            string normalized = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(normalized.ToLower());
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public static string ToHash<T>(this T input)
        {
            string normalized = JsonSerializer.Serialize(input, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            if (string.IsNullOrEmpty(normalized))
                return string.Empty;

            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(normalized.ToLower());
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
