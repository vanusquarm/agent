using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core
{
    public class GetAcctBalancesResponse
    {
        [JsonPropertyName("customerAccountBalances")]
        public List<CustomerAccountBalance> CustomerAccountBalances { get; set; }

        [JsonPropertyName("isLastSet")]
        public string IsLastSet { get; set; }

        [JsonPropertyName("setNumber")]
        public int? SetNumber { get; set; }
    }
    public class Amount
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("value")]
        public double? Value { get; set; }
    }

    public class Balance
    {
        [JsonPropertyName("amount")]
        public Amount Amount { get; set; }

        [JsonPropertyName("creditDebitIndicator")]
        public string CreditDebitIndicator { get; set; }

        [JsonPropertyName("balanceType")]
        public string BalanceType { get; set; }
    }

    public class CustomerAccountBalance
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("accountNickName")]
        public string AccountNickName { get; set; }

        [JsonPropertyName("productType")]
        public string ProductType { get; set; }
        public string ProductDescription { get; set; }

        [JsonPropertyName("IBAN")]
        public string IBAN { get; set; }

        [JsonPropertyName("applicationReferenceIdentification")]
        public string ApplicationReferenceIdentification { get; set; }

        [JsonPropertyName("balances")]
        public List<Balance> Balances { get; set; }
    }

}