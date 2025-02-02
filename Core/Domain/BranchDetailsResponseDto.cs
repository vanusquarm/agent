using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class BranchAddressDto
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

    public class ClearingDetailsDto
    {
        [JsonPropertyName("branchLatencyForClearingValueDate")]
        public int BranchLatencyForClearingValueDate { get; set; }

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

    public class PaymentDetailsDto
    {
        [JsonPropertyName("swiftBranchName")]
        public string SwiftBranchName { get; set; }

        [JsonPropertyName("swiftAddressLine1")]
        public string SwiftAddressLine1 { get; set; }

        [JsonPropertyName("swiftAddressLine2")]
        public string SwiftAddressLine2 { get; set; }

        [JsonPropertyName("swiftAddressLine3")]
        public string SwiftAddressLine3 { get; set; }
    }

    public class BranchDetails
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
        public BranchAddressDto Address { get; set; }

        [JsonPropertyName("clearingDetails")]
        public ClearingDetailsDto ClearingDetails { get; set; }

        [JsonPropertyName("paymentDetails")]
        public PaymentDetailsDto PaymentDetails { get; set; }
    }

    public class BranchDetailsResponseDto
    {
        [JsonPropertyName("branchList")]
        public List<BranchDetails> BranchList { get; set; }
    }


    public class BranchCustomerAccounts
    {
        public List<BranchDetails> BranchList { get; set; }
    }
}

