using System;

namespace Finacle.Core.Domain
{
    public class DisburseResponseDto
    {
        public string AccountIdentification { get; set; }
        public string TransactionReference { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}