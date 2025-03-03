using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class IntraBankTransferRequest
    {
        public string SourceAccountId { get; set; }
        public string DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string ChannelReference { get; set; }
        public string ChannelId { get; set; }
        public string DocNum { get; set; }
        public string UserPartTransactionCode { get; set; }
        public string ChargeEventId { get; set; }
    }
}