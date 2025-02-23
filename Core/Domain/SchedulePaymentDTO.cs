namespace Finacle.Core.Domain
{
    public class SchedulePaymentDTO
    {
        public string TransactionType { get; set; } // e.g., "C"
        public string PaymentValueDate { get; set; } // Date when the payment is scheduled
        public decimal PaymentAmount { get; set; } // Total payment amount
        public string TransferAccountIdentification { get; set; } // Identifier for the transfer account
        public decimal AdjustedPrincipal { get; set; } // Amount adjusted towards principal
        public decimal AdjustedInterest { get; set; } // Amount adjusted towards interest
        public decimal AdjustedFee { get; set; } // Amount adjusted towards fees
    }

}
