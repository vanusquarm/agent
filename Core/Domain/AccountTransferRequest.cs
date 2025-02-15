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
    }
}