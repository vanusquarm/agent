using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class BranchAddressDto
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }

    public class ClearingDetailsDto
    {
        public int BranchLatencyForClearingValueDate { get; set; }
        public string MicrCenterCode { get; set; }
        public string MicrBankCode { get; set; }
        public string MicrBranchCode { get; set; }
        public string ClearingLocality { get; set; }
        public string ClearingRepresentative { get; set; }
        public string DemandDraftIssueBranch { get; set; }
        public string BranchNameForDemandDraftPrinting { get; set; }
        public string Clearing { get; set; }
        public string CtsEnabled { get; set; }
        public string CollectionAgentBank { get; set; }
        public string CollectionAgentBranch { get; set; }
        public string RoutingBankCode { get; set; }
        public string RoutingBranchCode { get; set; }
        public string SpecialCollectionCenter { get; set; }
    }

    public class PaymentDetailsDto
    {
        public string SwiftBranchName { get; set; }
        public string SwiftAddressLine1 { get; set; }
        public string SwiftAddressLine2 { get; set; }
        public string SwiftAddressLine3 { get; set; }
    }

    public class BranchDetails
    {
        public string BranchCode { get; set; }
        public string BankCode { get; set; }
        public string DeletedFlag { get; set; }
        public string BankType { get; set; }
        public string BranchName { get; set; }
        public string BranchShortName { get; set; }
        public BranchAddressDto Address { get; set; }
        public ClearingDetailsDto ClearingDetails { get; set; }
        public PaymentDetailsDto PaymentDetails { get; set; }
    }

    public class BranchDetailsResponseDto
    {
        public List<BranchDetails> BranchList { get; set; }
    }


    public class BranchCustomerAccounts
    {
        public List<BranchDetails> BranchList { get; set; }
    }
}

