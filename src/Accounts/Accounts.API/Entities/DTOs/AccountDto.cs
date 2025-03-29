using System.Text.Json.Serialization;

namespace Acccounts.API.Entities.DTOs
{
    //public class AccountDto
    //{
    //    [JsonPropertyName("accountIdentification")]
    //    public string AccountIdentification { get; set; }

    //    [JsonPropertyName("iBAN")]
    //    public string IBAN { get; set; }
    //}

    public class AccountDto
    {
        [JsonPropertyName("customerReference")]
        public string? CustomerReference { get; set; }

        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("productCategory")]
        public string? ProductCategory { get; set; }

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("productDescription")]
        public string? ProductDescription { get; set; }

        [JsonPropertyName("branchName")]
        public string? BranchName { get; set; }

        [JsonPropertyName("iBAN")]
        public string? IBAN { get; set; }

        [JsonPropertyName("accountIdentification")]
        public string? AccountIdentification { get; set; }

        [JsonPropertyName("accountName")]
        public string? AccountName { get; set; }

        [JsonPropertyName("accountNickName")]
        public string? AccountNickName { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string? AccountCurrency { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string? AccountOpeningDate { get; set; }

        [JsonPropertyName("accountStatus")]
        public string? AccountStatus { get; set; }

        [JsonPropertyName("accountState")]
        public string? AccountState { get; set; }

        [JsonPropertyName("modeOfOperation")]
        public string? ModeOfOperation { get; set; }

        [JsonPropertyName("relatedPartyDetails")]
        public List<RelatedPartyDetail>? RelatedPartyDetails { get; set; }

        [JsonPropertyName("accountFreezeStatus")]
        public string? AccountFreezeStatus { get; set; }

        [JsonPropertyName("effectiveRateOfInterest")]
        public int EffectiveRateOfInterest { get; set; }

        [JsonPropertyName("accountAPY")]
        public double AccountAPY { get; set; }

        [JsonPropertyName("accountMinimumBalanceAmount")]
        public int AccountMinimumBalanceAmount { get; set; }

        [JsonPropertyName("effectiveAvailableBalanceAmount")]
        public int EffectiveAvailableBalanceAmount { get; set; }

        [JsonPropertyName("availableBalanceAmount")]
        public int AvailableBalanceAmount { get; set; }

        [JsonPropertyName("clearBalanceAmount")]
        public int ClearBalanceAmount { get; set; }

        [JsonPropertyName("lienAmount")]
        public int LienAmount { get; set; }

        [JsonPropertyName("taxRate")]
        public int TaxRate { get; set; }

        [JsonPropertyName("sourcingDealerIdentification")]
        public string? SourcingDealerIdentification { get; set; }
    }

    public class RelatedPartyDetail
    {
        [JsonPropertyName("relatedPartyCustomerReference")]
        public string? RelatedPartyCustomerReference { get; set; }

        [JsonPropertyName("relationType")]
        public string? RelationType { get; set; }

        [JsonPropertyName("relationCode")]
        public string? RelationCode { get; set; }

        [JsonPropertyName("designationCode")]
        public string? DesignationCode { get; set; }
    }

    public class AccountDetailsDto
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("productDescription")]
        public string ProductDescription { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("productCategory")]
        public string ProductCategory { get; set; }

        [JsonPropertyName("branchCode")]
        public string BranchCode { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string AccountOpeningDate { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("lienAmount")]
        public string LienAmount { get; set; }

        [JsonPropertyName("accountFreezeStatus")]
        public string AccountFreezeStatus { get; set; }

        [JsonPropertyName("clearBalanceAmount")]
        public string ClearBalanceAmount { get; set; }

        [JsonPropertyName("availableBalanceAmount")]
        public string AvailableBalanceAmount { get; set; }

        [JsonPropertyName("effectiveAvailableBalanceAmount")]
        public string EffectiveAvailableBalanceAmount { get; set; }

        [JsonPropertyName("modeOfOperation")]
        public string ModeOfOperation { get; set; }

        [JsonPropertyName("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("customerShortName")]
        public string CustomerShortName { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("accountState")]
        public string AccountState { get; set; }

        [JsonPropertyName("accountAPY")]
        public string AccountAPY { get; set; }

        [JsonPropertyName("sourcingDealeridentification")]
        public string SourcingDealeridentification { get; set; }

        [JsonPropertyName("accountNickname")]
        public string AccountNickname { get; set; }

        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }

        [JsonPropertyName("accountMinimumBalanceAmount")]
        public string AccountMinimumBalanceAmount { get; set; }

        [JsonPropertyName("effectiveRateOfInterest")]
        public string EffectiveRateOfInterest { get; set; }

        [JsonPropertyName("IBAN")]
        public string IBAN { get; set; }

        [JsonPropertyName("taxRate")]
        public string TaxRate { get; set; }

        [JsonPropertyName("relatedPartyDetails")]
        public List<RelatedPartyDetail> RelatedPartyDetails { get; set; }
    }

}
