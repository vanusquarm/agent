using System;

namespace Finacle.Core.Domain
{
    public class IntraBankTransferResponse
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionReference { get; set; }
    }
}