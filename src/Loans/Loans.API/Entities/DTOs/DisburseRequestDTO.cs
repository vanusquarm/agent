
using System;

namespace Loans.API.Entities.DTOs
{
    public class DisburseRequestDTO
    {
        public string CreditAccountIdentification { get; set; }
        public string TransactionValueDate { get; set; }
        public decimal DisburseAmount { get; set; }
        public string ModeOfDisbursement { get; set; }
        public string DeductOverdueDemands { get; set; }
        public string TransactionType { get; set; }
        public string DisbursementSegmentNumber { get; set; }
    }

}
