using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class GetCustomerAcctsResponse
    {
        [JsonPropertyName("basicAccountDetails")]
        public List<Account> BasicAccountDetails { get; set; }

        [JsonPropertyName("isLastSet")]
        public string IsLastSet { get; set; }

        [JsonPropertyName("setNumber")]
        public int? SetNumber { get; set; }
    }

    public class Account
    {
        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("productType")]
        public string ProductType { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("effectiveAvailableBalance")]
        public int? EffectiveAvailableBalance { get; set; }

        [JsonPropertyName("accountNickName")]
        public string AccountNickName { get; set; }

        [JsonPropertyName("accountOpenDate")]
        public string AccountOpenDate { get; set; }

        [JsonPropertyName("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonPropertyName("accountState")]
        public string AccountState { get; set; }

        [JsonPropertyName("lastUpdatedTime")]
        public DateTime? LastUpdatedTime { get; set; }

        [JsonPropertyName("branchIdentifier")]
        public string BranchIdentifier { get; set; }

        [JsonPropertyName("accountFreezeType")]
        public string AccountFreezeType { get; set; }

        [JsonPropertyName("IBAN")]
        public string IBAN { get; set; }

        [JsonPropertyName("applicationReferenceIdentification")]
        public string ApplicationReferenceIdentification { get; set; }

        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; }

        [JsonPropertyName("productConcept")]
        public string ProductConcept { get; set; }

        [JsonPropertyName("relatedPartyDetails")]
        public List<RelatedPartyDetail> RelatedPartyDetails { get; set; }
    }

    //public class RelatedPartyDetail
    //{
    //    [JsonPropertyName("relationType")]
    //    public string RelationType { get; set; }

    //    [JsonPropertyName("relationCode")]
    //    public string RelationCode { get; set; }

    //    [JsonPropertyName("designationCode")]
    //    public string DesignationCode { get; set; }

    //    [JsonPropertyName("relatedPartycustomerReference")]
    //    public string RelatedPartycustomerReference { get; set; }
    //}


}
