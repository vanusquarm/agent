using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class GetCustomerAcctsResponse
    {
        [JsonPropertyName("basicAccountDetails")]
        public List<Account>? BasicAccountDetails { get; set; }

        [JsonPropertyName("isLastSet")]
        public string? IsLastSet { get; set; }

        [JsonPropertyName("setNumber")]
        public int? SetNumber { get; set; }
    }

    public partial class Account
    {
        [JsonPropertyName("customerReference")]
        public string? CustomerReference { get; set; }

        [JsonPropertyName("accountIdentification")]
        public string? AccountIdentification { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string? AccountCurrency { get; set; }

        [JsonPropertyName("accountName")]
        public string? AccountName { get; set; }

        [JsonPropertyName("productType")]
        public string? ProductType { get; set; }

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("effectiveAvailableBalance")]
        public int? EffectiveAvailableBalance { get; set; }

        [JsonPropertyName("accountNickName")]
        public string? AccountNickName { get; set; }

        [JsonPropertyName("accountOpenDate")]
        public string? AccountOpenDate { get; set; }

        [JsonPropertyName("accountStatus")]
        public string? AccountStatus { get; set; }

        [JsonPropertyName("accountState")]
        public string? AccountState { get; set; }

        [JsonPropertyName("lastUpdatedTime")]
        public DateTime? LastUpdatedTime { get; set; }

        [JsonPropertyName("branchIdentifier")]
        public string? BranchIdentifier { get; set; }

        [JsonPropertyName("accountFreezeType")]
        public string? AccountFreezeType { get; set; }

        [JsonPropertyName("IBAN")]
        public string? IBAN { get; set; }

        [JsonPropertyName("applicationReferenceIdentification")]
        public string? ApplicationReferenceIdentification { get; set; }

        [JsonPropertyName("productCategory")]
        public string? ProductCategory { get; set; }

        [JsonPropertyName("productConcept")]
        public string? ProductConcept { get; set; }

        [JsonPropertyName("relatedPartyDetails")]
        public List<RelatedPartyDetail>? RelatedPartyDetails { get; set; }
        
        //[JsonPropertyName("availableBalance")]
        //public double AvailableBalance { get; set; }
    }

    public class RelatedPartyDetail
    {
        [JsonPropertyName("relationType")]
        public string? RelationType { get; set; }

        [JsonPropertyName("relationCode")]
        public string? RelationCode { get; set; }

        [JsonPropertyName("designationCode")]
        public string? DesignationCode { get; set; }

        [JsonPropertyName("relatedPartycustomerReference")]
        public string? RelatedPartycustomerReference { get; set; }
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

    public partial class Account
    {
        [JsonPropertyName("productDescription")]
        public string? ProductDescription { get; set; }

        [JsonPropertyName("branchCode")]
        public string? BranchCode { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string? AccountOpeningDate { get; set; }

        [JsonPropertyName("lienAmount")]
        public string? LienAmount { get; set; }

        [JsonPropertyName("accountFreezeStatus")]
        public string? AccountFreezeStatus { get; set; }

        [JsonPropertyName("clearBalanceAmount")]
        public string? ClearBalanceAmount { get; set; }

        [JsonPropertyName("availableBalanceAmount")]
        public string? AvailableBalanceAmount { get; set; }

        [JsonPropertyName("effectiveAvailableBalanceAmount")]
        public string? EffectiveAvailableBalanceAmount { get; set; }

        [JsonPropertyName("modeOfOperation")]
        public string? ModeOfOperation { get; set; }

        [JsonPropertyName("customerShortName")]
        public string? CustomerShortName { get; set; }

        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("accountAPY")]
        public string? AccountAPY { get; set; }

        [JsonPropertyName("sourcingDealeridentification")]
        public string? SourcingDealeridentification { get; set; }

        [JsonPropertyName("accountNickname")]
        public string? AccountNickname { get; set; }

        [JsonPropertyName("branchName")]
        public string? BranchName { get; set; }

        [JsonPropertyName("accountMinimumBalanceAmount")]
        public string? AccountMinimumBalanceAmount { get; set; }

        [JsonPropertyName("effectiveRateOfInterest")]
        public string? EffectiveRateOfInterest { get; set; }

        [JsonPropertyName("taxRate")]
        public string? TaxRate { get; set; }
    }


}
