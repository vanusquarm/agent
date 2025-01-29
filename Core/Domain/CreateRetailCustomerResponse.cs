using System;
using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class CreateRetailCustomerResponse
    {
    public string CustomerIdentifier { get; set; }
    public string Salutation { get; set; }
    public string BVN { get; set; }
    public string TierInformation { get; set; }
    public string EST { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }
    public string Gender { get; set; }
    public DateTime Dob { get; set; }
    public string Minor { get; set; }
    public GuardianDetailDto GuardianDetails { get; set; }
    public string SeniorCitizen { get; set; }
    public List<PhoneDetailDto> PhoneDetails { get; set; }
    public List<EmailDetailDto> EmailDetails { get; set; }
    public List<AddressDetailDto> AddressDetails { get; set; }
    public List<EmploymentDetailDto> EmploymentDetails { get; set; }
    public List<RiskRatingDetailDto> RiskRatingDetails { get; set; }
    public List<RelatedPartyDto> RelatedParty { get; set; }
    public List<ProductPreferenceDetailDto> ProductPreferenceDetails { get; set; }
    public List<CommunicationPreferenceDetailDto> CommunicationPreferenceDetails { get; set; }
    public List<EducationDetailDto> EducationDetails { get; set; }
    public List<CustomerDocumentDto> CustomerDocuments { get; set; }
    public List<RelationshipManagerDetailDto> RelationshipManagerDetails { get; set; }
    public string PrimaryBranch { get; set; }
    public string Segment { get; set; }
    public string SubSegment { get; set; }
    public DateTime RelationshipStartDate { get; set; }
    public string CountryOfPrimaryCitizenship { get; set; }
    public string TaxResident { get; set; }
    public string TaxIdType { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string TaxExemptionCode { get; set; }
    public string EmploymentStatus { get; set; }
    public int AnnualSalaryIncome { get; set; }
    public string MaritalStatus { get; set; }
    public string MotherMaidenName { get; set; }
    public string KycStatus { get; set; }
    public DateTime ReviewedDate { get; set; }

    }
}