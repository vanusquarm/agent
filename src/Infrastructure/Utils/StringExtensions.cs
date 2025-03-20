using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
