namespace Finacle.Core.Domain
{
    public class PayoffRequestDTO
    {
        public string TransactionType { get; set; } // e.g., "C"
        public string PayoffValueDate { get; set; } // Date when the payoff should be applied
        public string CollectionAccountIdentification { get; set; } // Identifier for the collection account
    }

}
