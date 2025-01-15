using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class GetRCResponse
    {
        [JsonPropertyName("customerIdentifier")]
        public string? CustomerIdentifier { get; set; }

        [JsonPropertyName("salutation")]
        public string? Salutation { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("customerName")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("dob")]
        public string? Dob { get; set; }

        [JsonPropertyName("minor")]
        public string? Minor { get; set; }

        [JsonPropertyName("guardianDetails")]
        public GuardianDetails? GuardianDetails { get; set; }

        [JsonPropertyName("seniorCitizen")]
        public string? SeniorCitizen { get; set; }

        [JsonPropertyName("phoneDetails")]
        public List<PhoneDetail>? PhoneDetails { get; set; }

        [JsonPropertyName("emailDetails")]
        public List<EmailDetail>? EmailDetails { get; set; }

        [JsonPropertyName("addressDetails")]
        public List<AddressDetail>? AddressDetails { get; set; }

        [JsonPropertyName("customerDocuments")]
        public List<CustomerDocument>? CustomerDocuments { get; set; }

        [JsonPropertyName("relationshipManagerDetails")]
        public List<RelationshipManagerDetail>? RelationshipManagerDetails { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string? PrimaryBranch { get; set; }

        [JsonPropertyName("segment")]
        public string? Segment { get; set; }

        [JsonPropertyName("subsegment")]
        public string? Subsegment { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public string? RelationshipStartDate { get; set; }

        [JsonPropertyName("countryOfPrimaryCitizenship")]
        public string? CountryOfPrimaryCitizenship { get; set; }

        [JsonPropertyName("taxResident")]
        public string? TaxResident { get; set; }

        [JsonPropertyName("taxIdType")]
        public string? TaxIdType { get; set; }

        [JsonPropertyName("taxIdentificationNumber")]
        public string? TaxIdentificationNumber { get; set; }

        [JsonPropertyName("taxExemptionCode")]
        public string? TaxExemptionCode { get; set; }

        [JsonPropertyName("employmentStatus")]
        public string? EmploymentStatus { get; set; }

        [JsonPropertyName("annualSalaryIncome")]
        public int? AnnualSalaryIncome { get; set; }

        [JsonPropertyName("maritalStatus")]
        public string? MaritalStatus { get; set; }

        [JsonPropertyName("motherMaidenName")]
        public string? MotherMaidenName { get; set; }

        [JsonPropertyName("kycStatus")]
        public string? KycStatus { get; set; }

        [JsonPropertyName("reviewedDate")]
        public string? ReviewedDate { get; set; }
    }

    //public class AddressDetail
    //{
    //    [JsonPropertyName("addressId")]
    //    public string? AddressId { get; set; }

    //    [JsonPropertyName("deleteAddress")]
    //    public string? DeleteAddress { get; set; }

    //    [JsonPropertyName("addressFormat")]
    //    public string? AddressFormat { get; set; }

    //    [JsonPropertyName("addressType")]
    //    public string? AddressType { get; set; }

    //    [JsonPropertyName("line1")]
    //    public string? Line1 { get; set; }

    //    [JsonPropertyName("line2")]
    //    public string? Line2 { get; set; }

    //    [JsonPropertyName("line3")]
    //    public string? Line3 { get; set; }

    //    [JsonPropertyName("houseNumber")]
    //    public string? HouseNumber { get; set; }

    //    [JsonPropertyName("buildingLevel")]
    //    public string? BuildingLevel { get; set; }

    //    [JsonPropertyName("premiseName")]
    //    public string? PremiseName { get; set; }

    //    [JsonPropertyName("streetNumber")]
    //    public string? StreetNumber { get; set; }

    //    [JsonPropertyName("streetName")]
    //    public string? StreetName { get; set; }

    //    [JsonPropertyName("locality")]
    //    public string? Locality { get; set; }

    //    [JsonPropertyName("suburb")]
    //    public string? Suburb { get; set; }

    //    [JsonPropertyName("town")]
    //    public string? Town { get; set; }

    //    [JsonPropertyName("postalCode")]
    //    public string? PostalCode { get; set; }

    //    [JsonPropertyName("country")]
    //    public string? Country { get; set; }

    //    [JsonPropertyName("state")]
    //    public string? State { get; set; }

    //    [JsonPropertyName("city")]
    //    public string? City { get; set; }

    //    [JsonPropertyName("residingSince")]
    //    public string? ResidingSince { get; set; }

    //    [JsonPropertyName("residingTill")]
    //    public string? ResidingTill { get; set; }

    //    [JsonPropertyName("preferredAddress")]
    //    public string? PreferredAddress { get; set; }

    //    [JsonPropertyName("holdMail")]
    //    public string? HoldMail { get; set; }

    //    [JsonPropertyName("holdMailInitiatedby")]
    //    public string? HoldMailInitiatedby { get; set; }

    //    [JsonPropertyName("holdMailReason")]
    //    public string? HoldMailReason { get; set; }

    //    [JsonPropertyName("holdMailStartDate")]
    //    public string? HoldMailStartDate { get; set; }

    //    [JsonPropertyName("holdmailEndDate")]
    //    public string? HoldmailEndDate { get; set; }

    //    [JsonPropertyName("ownershipType")]
    //    public string? OwnershipType { get; set; }

    //    [JsonPropertyName("LGA")]
    //    public string? LGA { get; set; }

    //    [JsonPropertyName("SocialMediaHandle")]
    //    public string? SocialMediaHandle { get; set; }
    //}

    //public class CustomerDocument
    //{
    //    [JsonPropertyName("documentId")]
    //    public string? DocumentId { get; set; }

    //    [JsonPropertyName("deleteDocument")]
    //    public string? DeleteDocument { get; set; }

    //    [JsonPropertyName("documentNumber")]
    //    public string? DocumentNumber { get; set; }

    //    [JsonPropertyName("documentForDetails")]
    //    public List<DocumentForDetail>? DocumentForDetails { get; set; }

    //    [JsonPropertyName("documentType")]
    //    public string? DocumentType { get; set; }

    //    [JsonPropertyName("issueDate")]
    //    public string? IssueDate { get; set; }

    //    [JsonPropertyName("expiryDate")]
    //    public string? ExpiryDate { get; set; }

    //    [JsonPropertyName("placeOfIssue")]
    //    public string? PlaceOfIssue { get; set; }

    //    [JsonPropertyName("issuingAuthority")]
    //    public string? IssuingAuthority { get; set; }

    //    [JsonPropertyName("countryOfIssue")]
    //    public string? CountryOfIssue { get; set; }
    //}

    //public class DocumentForDetail
    //{
    //    [JsonPropertyName("documentFor")]
    //    public string? DocumentFor { get; set; }
    //}

    //public class EmailDetail
    //{
    //    [JsonPropertyName("emailId")]
    //    public string? EmailId { get; set; }

    //    [JsonPropertyName("deleteEmail")]
    //    public string? DeleteEmail { get; set; }

    //    [JsonPropertyName("emailType")]
    //    public string? EmailType { get; set; }

    //    [JsonPropertyName("emailAddress")]
    //    public string? EmailAddress { get; set; }

    //    [JsonPropertyName("preferredEmailAddress")]
    //    public string? PreferredEmailAddress { get; set; }
    //}

    public class GuardianDetails
    {
        [JsonPropertyName("relationshipType")]
        public string? RelationshipType { get; set; }

        [JsonPropertyName("guardianType")]
        public string? GuardianType { get; set; }

        [JsonPropertyName("guardianCifType")]
        public string? GuardianCifType { get; set; }

        [JsonPropertyName("guardianId")]
        public string? GuardianId { get; set; }

        [JsonPropertyName("guardianName")]
        public string? GuardianName { get; set; }

        [JsonPropertyName("guardianPhoneNumber")]
        public string? GuardianPhoneNumber { get; set; }

        [JsonPropertyName("guardianEmailAddress")]
        public string? GuardianEmailAddress { get; set; }

        [JsonPropertyName("guardianRemarks")]
        public string? GuardianRemarks { get; set; }
    }

    //public class PhoneDetail
    //{
    //    [JsonPropertyName("phoneId")]
    //    public string? PhoneId { get; set; }

    //    [JsonPropertyName("deletePhoneFlag")]
    //    public string? DeletePhoneFlag { get; set; }

    //    [JsonPropertyName("countryCode")]
    //    public string? CountryCode { get; set; }

    //    [JsonPropertyName("phoneType")]
    //    public string? PhoneType { get; set; }

    //    [JsonPropertyName("phoneNumber")]
    //    public string? PhoneNumber { get; set; }

    //    [JsonPropertyName("preferredPhoneNumber")]
    //    public string? PreferredPhoneNumber { get; set; }
    //}

    //public class RelationshipManagerDetail
    //{
    //    [JsonPropertyName("department")]
    //    public string? Department { get; set; }

    //    [JsonPropertyName("id")]
    //    public string? Id { get; set; }

    //    [JsonPropertyName("relationshipId")]
    //    public string? RelationshipId { get; set; }

    //    [JsonPropertyName("deleteRelationship")]
    //    public string? DeleteRelationship { get; set; }

    //    [JsonPropertyName("relationshipManagerId")]
    //    public string? RelationshipManagerId { get; set; }

    //    [JsonPropertyName("relationshipManagerName")]
    //    public string? RelationshipManagerName { get; set; }

    //    [JsonPropertyName("relationshipManagerPhoneNumber")]
    //    public string? RelationshipManagerPhoneNumber { get; set; }

    //    [JsonPropertyName("relationshipManagerEmailAddress")]
    //    public string? RelationshipManagerEmailAddress { get; set; }

    //    [JsonPropertyName("primaryRelationshipManager")]
    //    public string? PrimaryRelationshipManager { get; set; }

    //    [JsonPropertyName("pcCode")]
    //    public string? PcCode { get; set; }

    //    [JsonPropertyName("pcName")]
    //    public string? PcName { get; set; }

    //    [JsonPropertyName("pcEmail")]
    //    public string? PcEmail { get; set; }

    //    [JsonPropertyName("pcPhNumber")]
    //    public string? PcPhNumber { get; set; }

    //    [JsonPropertyName("teamName")]
    //    public string? TeamName { get; set; }

    //    [JsonPropertyName("teamEmail")]
    //    public string? TeamEmail { get; set; }

    //    [JsonPropertyName("businessEntity")]
    //    public string? BusinessEntity { get; set; }
    //}


}
