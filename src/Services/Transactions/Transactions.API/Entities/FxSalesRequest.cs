namespace Transactions.API.Entities
{
    public class FxSalesRequest
    {

        public string DebitAccount { get; set; }


        public string CreditAccount { get; set; }


        public string Amount { get; set; }


        public string? Remark { get; set; }


        public string Purpose { get; set; }


        public string ReferenceNumber { get; set; }


        public string? DebitChargeEventId { get; set; }


        public string? CreditChargeEventId { get; set; }


        public string? UserType { get; set; } // Admin or User


        public string? SecretAnswer { get; set; }

        public LienDetailsList[]? lienDetailsList { get; set; }


        public string? TellerId { get; set; }


        public string? SupervisorId { get; set; }


        public string? RateCode { get; set; }

        public float? Rate { get; set; }


        public string Currency { get; set; }

        public int? SubsidiaryPosting { get; set; }


        public string? SolId { get; set; }



        public string? DebitDate { get; set; }


        public string? CreditDate { get; set; }


        public string? TransactionType { get; set; }


        public string? TransactionCaseId { get; set; }


        public string? Form_M { get; set; }


        public string? LcBankNumber { get; set; }


        public string? MidCurrency { get; set; }

    }
}
