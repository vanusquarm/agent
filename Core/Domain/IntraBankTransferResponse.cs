using System;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class IntraBankTransferResponse
    {
        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("tranStatus")]
        public string TranStatus { get; set; }
    }
}