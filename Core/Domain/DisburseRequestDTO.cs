
using System;

namespace Finacle.Core.Domain
{
    public class DisburseRequestDTO
    {
        public string CreditAccountIdentification { get; set; }
        public DateTime TransactionValueDate { get; set; }
        public decimal DisburseAmount { get; set; }
        public string ModeOfDisbursement { get; set; }
        public string DeductOverdueDemands { get; set; }
        public string TransactionType { get; set; }
        public string DisbursementSegmentNumber { get; set; }
    }

}
