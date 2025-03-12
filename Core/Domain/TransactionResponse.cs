using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class TransactionResponse
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; }

        [JsonPropertyName("fromDate")]
        public string FromDate { get; set; }

        [JsonPropertyName("toDate")]
        public string ToDate { get; set; }

        [JsonPropertyName("tranDetails")]
        public List<TranDetail> TranDetails { get; set; }
    }

    public class TranDetail
    {
        [JsonPropertyName("branchCode")]
        public string BranchCode { get; set; }

        [JsonPropertyName("transactionCurrency")]
        public string TransactionCurrency { get; set; }

        [JsonPropertyName("transactionIdentification")]
        public string TransactionIdentification { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("transactionType")]
        public string TransactionType { get; set; }

        [JsonPropertyName("transactionSubType")]
        public string TransactionSubType { get; set; }

        [JsonPropertyName("transactionAmount")]
        public string TransactionAmount { get; set; }

        [JsonPropertyName("DebitCreditIndicator")]
        public string DebitCreditIndicator { get; set; }

        [JsonPropertyName("valueDate")]
        public string ValueDate { get; set; }

        [JsonPropertyName("transactionReference")]
        public string TransactionReference { get; set; }

        [JsonPropertyName("currencyDescription")]
        public string CurrencyDescription { get; set; }

        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }

        [JsonPropertyName("instrumentNo")]
        public string InstrumentNo { get; set; }

        [JsonPropertyName("instrumentType")]
        public string InstrumentType { get; set; }

        [JsonPropertyName("instrumentDate")]
        public string InstrumentDate { get; set; }

        [JsonPropertyName("SerialNum")]
        public string SerialNum { get; set; }

        [JsonPropertyName("postedTime")]
        public string PostedTime { get; set; }

        [JsonPropertyName("BankCode")]
        public string BankCode { get; set; }

        [JsonPropertyName("OriginatingBankCode")]
        public string OriginatingBankCode { get; set; }

        [JsonPropertyName("transactionRemarks")]
        public string TransactionRemarks { get; set; }

        [JsonPropertyName("contraAccount")]
        public string ContraAccount { get; set; }

        [JsonPropertyName("contraAmount")]
        public string ContraAmount { get; set; }

        [JsonPropertyName("contraTrantype")]
        public string ContraTrantype { get; set; }

        [JsonPropertyName("contraAccountName")]
        public string ContraAccountName { get; set; }

        [JsonPropertyName("reversalInd")]
        public string ReversalInd { get; set; }

        [JsonPropertyName("userTranCode")]
        public string UserTranCode { get; set; }

        [JsonPropertyName("userTranDescription")]
        public string UserTranDescription { get; set; }

        [JsonPropertyName("stan")]
        public string Stan { get; set; }
    }

    // Request DTO
    public class ChargeRequestv2
    {
        public string TransactionReference { get; set; }
        public string AccountId { get; set; }
        public string ChargeType { get; set; }
        public decimal ChargeAmount { get; set; }  // Changed to decimal
        public string Currency { get; set; } = "GHS";  // Changed to decimal

        public static ChargeRequestv2 Map(ChargeRequest chargeRequest)
        {
            return new ChargeRequestv2
            {
                TransactionReference = chargeRequest.RequestReference,
                AccountId = chargeRequest.AccountNumber,
                ChargeAmount = chargeRequest.ChargeAmount,
                ChargeType = chargeRequest.EventId
            };
        }
    }

    public class ChargeRequest
    {
        public string RequestReference { get; set; }
        public string AccountNumber { get; set; }
        public string EventId { get; set; }
        public decimal ChargeAmount { get; set; }
    }

    // Response DTO
    public class ChargeResponse
    {
        public string TransactionId { get; set; }
        public string ChrgAmt { get; set; } // Consider converting to decimal if needed
        public string ErrorFlg { get; set; }
    }

}
