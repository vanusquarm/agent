using System;
using System.Collections.Generic;

namespace Finacle.Core.Domain
{
    public class RetailCustomerRequest
    {
        public string TierInformation { get; set; }
     
        public List<AddressDetailDto> AddressDetails { get; set; }
     
        public string CountryOfPrimaryCitizenship { get; set; }
     
        public List<CustomerDocumentDto> CustomerDocuments { get; set; }
     
        public DateTime Dob { get; set; }
     
        public string PrimaryBranch { get; set; }
     
        public List<RelationshipManagerDetailDto> RelationshipManagerDetails { get; set; }
     
        public string TaxExemptionCode { get; set; }
     
        public string TaxResident { get; set; }
     
        public string Salutation { get; set; }
     
        public string BVN { get; set; }
     
        public string EST { get; set; }
     
        public string FirstName { get; set; }
     
        public string MiddleName { get; set; }
     
        public string LastName { get; set; }
     
        public string Suffix { get; set; }
     
        public string Gender { get; set; }
     
        public string Minor { get; set; }
     
        public GuardianDetailDto GuardianDetails { get; set; }
     
        public string SeniorCitizen { get; set; }
     
        public List<PhoneDetailDto> PhoneDetails { get; set; }
     
        public List<EmailDetailDto> EmailDetails { get; set; }
     
        public List<EmploymentDetailDto> EmploymentDetails { get; set; }
     
        public List<RiskRatingDetailDto> RiskRatingDetails { get; set; }
     
        public List<RelatedPartyDto> RelatedParty { get; set; }
     
        public List<ProductPreferenceDetailDto> ProductPreferenceDetails { get; set; }
     
        public List<CommunicationPreferenceDetailDto> CommunicationPreferenceDetails { get; set; }
     
        public List<EducationDetailDto> EducationDetails { get; set; }
     
        public string Segment { get; set; }
     
        public string SubSegment { get; set; }
     
        public DateTime RelationshipStartDate { get; set; }
     
        public string TaxIdType { get; set; }
     
        public string TaxIdentificationNumber { get; set; }
     
        public string EmploymentStatus { get; set; }
     
        public int AnnualSalaryIncome { get; set; }
     
        public string MaritalStatus { get; set; }
     
        public string MotherMaidenName { get; set; }
     
        public string KycStatus { get; set; }
     
        public DateTime ReviewedDate { get; set; }
    }
     
    public class AddressDetailDto
    {
        public string AddressFormat { get; set; }
        public string AddressType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PreferredAddress { get; set; }
        public DateTime ResidingSince { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string HouseNumber { get; set; }
        public string BuildingLevel { get; set; }
        public string PremiseName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string Locality { get; set; }
        public string Suburb { get; set; }
        public string Town { get; set; }
        public string State { get; set; }
        public DateTime ResidingTill { get; set; }
        public string HoldMail { get; set; }
        public string HoldMailInitiatedBy { get; set; }
        public string HoldMailReason { get; set; }
        public DateTime HoldMailStartDate { get; set; }
        public DateTime HoldMailEndDate { get; set; }
        public string OwnershipType { get; set; }
        public string LGA { get; set; }
        public string SocialMediaHandle { get; set; }
    }
     
    public class CustomerDocumentDto
    {
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public List<DocumentForDetailDto> DocumentForDetails { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string PlaceOfIssue { get; set; }
        public string IssuingAuthority { get; set; }
        public string CountryOfIssue { get; set; }
    }
     
    public class DocumentForDetailDto
    {
        public string DocumentFor { get; set; }
    }
     
    public class RelationshipManagerDetailDto
    {
        public string RelationshipManagerId { get; set; }
        public string RelationshipManagerName { get; set; }
        public string Id { get; set; }
        public string RelationshipManagerPhoneNumber { get; set; }
        public string RelationshipManagerEmailAddress { get; set; }
        public string PrimaryRelationshipManager { get; set; }
        public string Department { get; set; }
        public string PcCode { get; set; }
        public string PcName { get; set; }
        public string PcEmail { get; set; }
        public string PcPhNumber { get; set; }
        public string TeamName { get; set; }
        public string TeamEmail { get; set; }
        public string BusinessEntity { get; set; }
    }
     
    public class GuardianDetailDto
    {
        public string RelationshipType { get; set; }
        public string GuardianType { get; set; }
        public string GuardianCifType { get; set; }
        public string GuardianId { get; set; }
        public string GuardianName { get; set; }
        public string GuardianPhoneNumber { get; set; }
        public string GuardianEmailAddress { get; set; }
        public string GuardianRemarks { get; set; }
    }
     
    public class PhoneDetailDto
    {
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }
        public string PreferredPhoneNumber { get; set; }
    }
     
    public class EmailDetailDto
    {
        public string EmailAddress { get; set; }
        public string EmailType { get; set; }
        public string PreferredEmailAddress { get; set; }
    }
     
    public class EmploymentDetailDto
    {
        public string NatureOfEmployment { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmployerName { get; set; }
        public string EmployerId { get; set; }
        public string EmployerType { get; set; }
        public string Industry { get; set; }
        public string CountryOfDeputation { get; set; }
        public string Occupation { get; set; }
        public string BusinessName { get; set; }
        public string CurrentEmployment { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public DateTime EmploymentEndDate { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int GrossIncome { get; set; }
    public string IncomeFrequency { get; set; }
    }

    public class RiskRatingDetailDto
    {
        public string CustomerRating { get; set; }
        public string RiskRatingModel { get; set; }
        public string RiskRating { get; set; }
        public DateTime RiskRatingDate { get; set; }
        public string Remarks { get; set; }
        public DateTime RiskProfileValidTill { get; set; }
        public string OverrideRiskRating { get; set; }
        public DateTime RiskRatingOverriddenOn { get; set; }
        public string RiskRatingOverriddenBy { get; set; }
    }

    public class RelatedPartyDto
    {
        public string Relationship { get; set; }
        public string RepresenterId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailId { get; set; }
        public string RepresenterCifType { get; set; }
        public string RepresenterCifSubType { get; set; }
        public string Remarks { get; set; }
        public string DocumentReferenceId { get; set; }
        public string PoaCategory { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GuardianType { get; set; }
        public string BVN { get; set; }
        public string NIN { get; set; }
        public string Percentage { get; set; }
    }
     
    public class ProductPreferenceDetailDto
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public DateTime PreferenceStartDate { get; set; }
        public DateTime PreferenceEndDate { get; set; }
        public string Remarks { get; set; }
    }
     
    public class CommunicationPreferenceDetailDto
    {
        public string Type { get; set; }
        public string DoNotDisturb { get; set; }
        public string ChannelType { get; set; }
        public string ModeOfCommunication { get; set; }
        public DateTime PreferenceStartDate { get; set; }
        public DateTime PreferenceEndDate { get; set; }
        public string Remarks { get; set; }
    }
     
    public class EducationDetailDto
    {
        public string Qualification { get; set; }
        public string Course { get; set; }
        public string Institute { get; set; }
        public string University { get; set; }
        public string CourseStatus { get; set; }
        public DateTime CourseStartDate { get; set; }
        public DateTime CourseEndDate { get; set; }
        public string ScoreType { get; set; }
        public string ScoreObtained { get; set; }
        public string ClassObtained { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }

}