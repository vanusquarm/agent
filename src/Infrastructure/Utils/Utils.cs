using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                "1" => "CURRENT",
                "2" or "3" => "SAVINGS",
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

    }
}
