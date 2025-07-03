using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Utils
{
    public static class Utils
    {
        public static (string AccountType, string BranchCode, string Currency, string Sequence)
        ParseAccountNumber(string accountNumber)
        {
            if (accountNumber.Length < 13)
                throw new ArgumentException("Account number must be greater than 13 digits");

            var typeCode = accountNumber.Substring(0, 1);
            var branch = accountNumber.Substring(1, 3);
            var currencyCode = accountNumber.Substring(4, 3);
            var sequence = accountNumber.Substring(7, 6);

            return (
                AccountType: GetAccountTypeName(typeCode),
                BranchCode: branch,
                Currency: GetCurrencyName(currencyCode),
                Sequence: sequence
            );
        }

        private static string GetAccountTypeName(string code)
        {
            return code switch
            {
                "0" => "OFFICE",
                "1" or "2" => "SAVINGS",
                "3" => "CURRENT",
                _ => "UNKNOWN"
            };
        }

        private static string GetCurrencyName(string code)
        {
            return code switch
            {
                "001" => "GHS",
                "002" => "USD",
                "003" => "GBP",
                "004" => "EUR",
                _ => "UNKNOWN"
            };
        }

        public static bool IsValidPhoneNumber(string phone, out string localNumber)
        {
            localNumber = null;

            try
            {
                if (string.IsNullOrWhiteSpace(phone)) { return false; }

                // Remove all non-digit characters
                Regex regex = new Regex(@"[^\d]", RegexOptions.None, TimeSpan.FromSeconds(1));
                string cleanPhoneNumber = regex.Replace(phone, "");

                // Ensure it is numeric
                if (!long.TryParse(cleanPhoneNumber, out _)) { return false; }

                // Ghana local number format (Example: +233XXXXXXXXX -> 0XXXXXXXXX)
                if (cleanPhoneNumber.StartsWith("233") && cleanPhoneNumber.Length == 12) { localNumber = "0" + cleanPhoneNumber.Substring(3); return true; }

                // Already in local format (e.g., 024XXXXXXX)
                if (cleanPhoneNumber.Length == 10 && cleanPhoneNumber.StartsWith("0")) { localNumber = cleanPhoneNumber; return true; }

                return false; // Invalid format
            }
            catch (Exception) { return false; }
        }
    }
}
