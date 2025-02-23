using System;

namespace Loans.API.Entities.DTOs
{
    public class DisburseResponseDto
    {
        public string AccountIdentification { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionDate { get; set; }
    }
}