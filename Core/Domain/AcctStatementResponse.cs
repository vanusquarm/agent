using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class AcctStatementResponse
    {
        [JsonPropertyName("accountDetails")]
        public AccountDetails AccountDetails { get; set; }

        [JsonPropertyName("openingBalance")]
        public int OpeningBalance { get; set; }

        [JsonPropertyName("closingBalance")]
        public int ClosingBalance { get; set; }

        [JsonPropertyName("statementRecords")]
        public List<StatementRecord> StatementRecords { get; set; }

        [JsonPropertyName("isLastSet")]
        public string IsLastSet { get; set; }

        [JsonPropertyName("setNumber")]
        public int SetNumber { get; set; }
    }

    public class AccountDetails
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }

    public class StatementRecord
    {
        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transactionValueDate")]
        public string TransactionValueDate { get; set; }

        [JsonPropertyName("transactionAmount")]
        public string TransactionAmount { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }

        [JsonPropertyName("tranParticulars")]
        public string TranParticulars { get; set; }
    }
}
