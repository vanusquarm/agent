using Statement.Web.Models;
using System.Data;

namespace Statement.Web.Helpers
{
    public static class Utility
    {
        public static (string AccountNumber, string AccountName, string ProductDescription) GetPrioritizedAccountWithDescription(List<AccountInfo> accounts)
        {
            // Define product code mappings with their descriptions

            var productDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"CURNT", "Current Account"},
                {"MCDCC", "MasterCard Credit Card"},
                {"SASAV", "Savings Account"},
                {"FCAAC", "Foreign Currency Account"},
                {"FEAAC", "Foreign Exchange Account"},
                {"ESSAV", "Easy Savers"},
                {"SENAC", "Seniors Account"},
                {"GTMAX", "GTMAX"},
                {"CDNOE", "Non Oil Export Domiciliary A\\C - Customer"},
                {"CALLA", "Call Accounts - Clients"},
                {"CDEXT", "External Domiciliary Accounts"},
                {"SADSA", "Domiciliary Savings Accounts"},
                {"SASKS", "Smart Kids Save"},
                {"SASTS", "Smart Teens Save"},
                {"SATAS", "Target Save"},
                {"SAGTC", "GTCrea8"},
                {"CDFPA", "FX Purchase Account"},
                {"SAINA", "Instant Account"},
                {"CDDOM", "Domiciliary Deposit"},
                {"CDEXP", "Export Dom Deposit"},
                {"SAGAB", "GTExpress - Agency Banking"},
                {"VCDPC", "Visa Prepaid Ledger"},
                {"MCDPC", "MasterCard Prepaid Ledger"},
                {"SDACT", "Security Deposits - Facilities"},
                {"FEMAC", "FEM A/C"},
                {"MCDOM", "MasterCard DOM Account"},
                {"PLGCA", "Current Account - Pledged"},
                {"SAVPL", "Savings A/C Pledged"},
                {"CAPLG", "Call A/C - Pledged"},
                {"CDPLG", "Domiciliary Deposit - Pledge"},
                {"POSMA", "POS Merchant Account"},
                {"PLGCP", "Pledged Current Account - Project"},
                {"PLGOD", "Other Pledged Deposits"},
                {"CMALC", "Cash Margins - LCs"},
                {"CMALG", "Cash Margins / LGs"},
                {"CMLCD", "Cash Margins - Accepted Guarantee Time LC Draft"},
                {"CCDIE", "Cash Collateral / Issue & Endorse Delivery Order"},
                {"CMFWD", "Cash Margins - Forward Deals"},
                {"CCLCS", "Cash Collateral Received - LCs"},
                {"CDODA", "Ordinary Domiciliary Account - Customer"},
                {"POMAC", "POS Merchant Account"},
                {"CRDAC", "Credit Account"},
                {"CHQAC", "Cheque Account"},
                {"DOMAC", "Domiciliary Account"},
                {"INVAC", "Investment Account"},
                {"SAVINGS ACCOUNT", "SAVINGS ACCOUNT"},
                {"CURRENT ACCOUNT", "CURRENT ACCOUNT"},
            };
            var prioritizedAccount = accounts.FirstOrDefault(a => a.ProductCode.Equals("CURNT", StringComparison.OrdinalIgnoreCase));

            if (prioritizedAccount == null)
            {
                prioritizedAccount = accounts.FirstOrDefault();
            }

            if (prioritizedAccount != null)
            {
                string description = productDescriptions.TryGetValue(prioritizedAccount.ProductCode, out var desc)
                    ? desc
                    : "Bank Account";

                return (
                    prioritizedAccount.AccountNumber,
                    prioritizedAccount.AccountName,
                    description
                );
            }

            return (string.Empty, string.Empty, "No Account Found");
        }
    }
}
