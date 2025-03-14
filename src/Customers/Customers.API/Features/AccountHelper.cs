using System.Text.RegularExpressions;

namespace Customers.API.Features
{
    public static class AccountHelper
    {
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
                if (cleanPhoneNumber.StartsWith("233") && cleanPhoneNumber.Length == 12) { localNumber = "0" + cleanPhoneNumber.Substring(3);  return true; }

                // Already in local format (e.g., 024XXXXXXX)
                if (cleanPhoneNumber.Length == 10 && cleanPhoneNumber.StartsWith("0"))  { localNumber = cleanPhoneNumber; return true; }

                return false; // Invalid format
            }
            catch (Exception) { return false; }
        }
    }
}
