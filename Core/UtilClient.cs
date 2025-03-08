using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public bool IsValidPhoneNumber(string phone, out string localNumber)
        {
            localNumber = null;

            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return false;

                // Remove all non-digit characters
                string cleanPhoneNumber = Regex.Replace(phone, @"[^\d]", "");

                // Ensure it is numeric
                if (!long.TryParse(cleanPhoneNumber, out _))
                    return false;

                // Ghana local number format (Example: +233XXXXXXXXX -> 0XXXXXXXXX)
                if (cleanPhoneNumber.StartsWith("233") && cleanPhoneNumber.Length == 12)
                {
                    localNumber = "0" + cleanPhoneNumber.Substring(3);
                    return true;
                }

                // Already in local format (e.g., 024XXXXXXX)
                if (cleanPhoneNumber.Length == 10 && cleanPhoneNumber.StartsWith("0"))
                {
                    localNumber = cleanPhoneNumber;
                    return true;
                }

                return false; // Invalid format
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
