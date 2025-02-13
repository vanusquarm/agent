using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class AccountTransferRequest
    {
        public string SourceAccountId { get; set; }
        public string DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string ChannelReference { get; set; }
        public string ChannelId { get; set; }

        public static string MapService(string key)
        {
            // List of services and their corresponding channel IDs
            Dictionary<string, string> serviceMap = new Dictionary<string, string>
            {
                { "GAPS", "GTBGH1" },
                { "737", "GTBGH2" },
                { "GTWORLD", "GTBGH3" },
                { "MYGHPAY", "GTBGH4" },
                { "GTCOLLECT", "GTBGH5" }
            };
            return serviceMap.TryGetValue(key.ToUpper(), out string value) ? value : "GTBGH100";
        }

    }
}