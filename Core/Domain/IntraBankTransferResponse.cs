using System;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class IntraBankTransferResponse
    {
        public string TransactionDate { get; set; }
        public string TransactionReference { get; set; }
        public string TranStatus { get; set; }
    }
}