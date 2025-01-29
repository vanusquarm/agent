using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{

    public class BranchDetailsResponse
    {
        [JsonPropertyName("branchCode")]
        public string BranchCode { get; set; }

        [JsonPropertyName("bankCode")]
        public string BankCode { get; set; }

        [JsonPropertyName("deletedFlag")]
        public string DeletedFlag { get; set; }

        [JsonPropertyName("bankType")]
        public string BankType { get; set; }

        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }

        [JsonPropertyName("branchShortName")]
        public string BranchShortName { get; set; }

        [JsonPropertyName("address")]
        public Address Address { get; set; }

        [JsonPropertyName("isoCountryCode")]
        public string IsoCountryCode { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("faxNumber")]
        public string FaxNumber { get; set; }

        [JsonPropertyName("telexNumber")]
        public string TelexNumber { get; set; }

        [JsonPropertyName("weeklyOffDay")]
        public int? WeeklyOffDay { get; set; }

        [JsonPropertyName("paymentSystemIndicator")]
        public string PaymentSystemIndicator { get; set; }

        [JsonPropertyName("tradeFinance")]
        public string TradeFinance { get; set; }

        [JsonPropertyName("foreignCurrencyInterBranchTransaction")]
        public string ForeignCurrencyInterBranchTransaction { get; set; }

        [JsonPropertyName("regionalOfficeBranchCode")]
        public string RegionalOfficeBranchCode { get; set; }

        [JsonPropertyName("zonalOfficeBranchCode")]
        public string ZonalOfficeBranchCode { get; set; }

        [JsonPropertyName("dataCenterAlias")]
        public string DataCenterAlias { get; set; }

        [JsonPropertyName("sdsBcbCode")]
        public string SdsBcbCode { get; set; }

        [JsonPropertyName("localBankCode")]
        public string LocalBankCode { get; set; }

        [JsonPropertyName("factorCode")]
        public string FactorCode { get; set; }

        [JsonPropertyName("freeCode1")]
        public string FreeCode1 { get; set; }

        [JsonPropertyName("freeCode2")]
        public string FreeCode2 { get; set; }

        [JsonPropertyName("freeCode3")]
        public string FreeCode3 { get; set; }

        [JsonPropertyName("preferredLanguageCode")]
        public string PreferredLanguageCode { get; set; }

        [JsonPropertyName("addressLine1InPreferredLanguage")]
        public string AddressLine1InPreferredLanguage { get; set; }

        [JsonPropertyName("addressLine2InPreferredLanguage")]
        public string AddressLine2InPreferredLanguage { get; set; }

        [JsonPropertyName("addressLine3InPreferredLanguage")]
        public string AddressLine3InPreferredLanguage { get; set; }

        [JsonPropertyName("branchNameInPreferredLanguage")]
        public string BranchNameInPreferredLanguage { get; set; }

        [JsonPropertyName("branchShortNameInPreferredLanguage")]
        public string BranchShortNameInPreferredLanguage { get; set; }

        [JsonPropertyName("clearingDetails")]
        public ClearingDetails ClearingDetails { get; set; }

        [JsonPropertyName("paymentDetails")]
        public PaymentDetails PaymentDetails { get; set; }
    }


    public class Address
    {
        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("addressLine3")]
        public string AddressLine3 { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
    }

    public class ClearingDetails
    {
        [JsonPropertyName("branchLatencyForClearingValueDate")]
        public int? BranchLatencyForClearingValueDate { get; set; }

        [JsonPropertyName("micrCenterCode")]
        public string MicrCenterCode { get; set; }

        [JsonPropertyName("micrBankCode")]
        public string MicrBankCode { get; set; }

        [JsonPropertyName("micrBranchCode")]
        public string MicrBranchCode { get; set; }

        [JsonPropertyName("clearingLocality")]
        public string ClearingLocality { get; set; }

        [JsonPropertyName("clearingRepresentative")]
        public string ClearingRepresentative { get; set; }

        [JsonPropertyName("demandDraftIssueBranch")]
        public string DemandDraftIssueBranch { get; set; }

        [JsonPropertyName("branchNameForDemandDraftPrinting")]
        public string BranchNameForDemandDraftPrinting { get; set; }

        [JsonPropertyName("clearing")]
        public string Clearing { get; set; }

        [JsonPropertyName("ctsEnabled")]
        public string CtsEnabled { get; set; }

        [JsonPropertyName("collectionAgentBank")]
        public string CollectionAgentBank { get; set; }

        [JsonPropertyName("collectionAgentBranch")]
        public string CollectionAgentBranch { get; set; }

        [JsonPropertyName("routingBankCode")]
        public string RoutingBankCode { get; set; }

        [JsonPropertyName("routingBranchCode")]
        public string RoutingBranchCode { get; set; }

        [JsonPropertyName("specialCollectionCenter")]
        public string SpecialCollectionCenter { get; set; }
    }

    public class MisDetails
    {
        [JsonPropertyName("freeCode1")]
        public string FreeCode1 { get; set; }

        [JsonPropertyName("freeCode2")]
        public string FreeCode2 { get; set; }

        [JsonPropertyName("freeCode3")]
        public string FreeCode3 { get; set; }

        [JsonPropertyName("freeCode4")]
        public string FreeCode4 { get; set; }

        [JsonPropertyName("freeCode5")]
        public string FreeCode5 { get; set; }

        [JsonPropertyName("freeCode6")]
        public string FreeCode6 { get; set; }

        [JsonPropertyName("freeCode7")]
        public string FreeCode7 { get; set; }

        [JsonPropertyName("freeCode8")]
        public string FreeCode8 { get; set; }

        [JsonPropertyName("freeCode9")]
        public string FreeCode9 { get; set; }

        [JsonPropertyName("freeCode10")]
        public string FreeCode10 { get; set; }
    }

    public class PaymentDetails
    {
        [JsonPropertyName("branchTableMaintenancePaymentDetails")]
        public List<object> BranchTableMaintenancePaymentDetails { get; set; }

        [JsonPropertyName("swiftBranchName")]
        public string SwiftBranchName { get; set; }

        [JsonPropertyName("swiftAddressLine1")]
        public string SwiftAddressLine1 { get; set; }

        [JsonPropertyName("swiftAddressLine2")]
        public string SwiftAddressLine2 { get; set; }

        [JsonPropertyName("swiftAddressLine3")]
        public string SwiftAddressLine3 { get; set; }

        [JsonPropertyName("misDetails")]
        public MisDetails MisDetails { get; set; }
    }

}





