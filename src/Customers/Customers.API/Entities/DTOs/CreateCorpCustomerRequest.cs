using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class CreateCorpCustomerRequest
    {
        [JsonPropertyName("corporateName")]
        public required string CorporateName { get; set; }

        [JsonPropertyName("BVN")]
        public string? BVN { get; set; }

        [JsonPropertyName("TierInformation")]
        public string? TierInformation { get; set; }

        [JsonPropertyName("corporateEntityType")]
        public required string CorporateEntityType { get; set; }

        [JsonPropertyName("registrationNumber")]
        public required string RegistrationNumber { get; set; }

        [JsonPropertyName("dateOfIncorporation")]
        public required string DateOfIncorporation { get; set; }

        [JsonPropertyName("countryOfIncorporation")]
        public required string CountryOfIncorporation { get; set; }

        [JsonPropertyName("taxIdType")]
        public string? TaxIdType { get; set; }

        [JsonPropertyName("taxIdentificationNumber")]
        public string? TaxIdentificationNumber { get; set; }

        [JsonPropertyName("corporateClassification")]
        public string? CorporateClassification { get; set; }

        [JsonPropertyName("standardIndustryCode")]
        public string? StandardIndustryCode { get; set; }

        [JsonPropertyName("publiclyHeld")]
        public string? PubliclyHeld { get; set; }

        [JsonPropertyName("primaryBranch")]
        public required string PrimaryBranch { get; set; }

        [JsonPropertyName("segment")]
        public string? Segment { get; set; }

        [JsonPropertyName("subsegment")]
        public string? Subsegment { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public string? RelationshipStartDate { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("healthCode")]
        public string? HealthCode { get; set; }

        [JsonPropertyName("noOfEmployees")]
        public int? NoOfEmployees { get; set; }

        [JsonPropertyName("sector")]
        public string? Sector { get; set; }

        [JsonPropertyName("subsector")]
        public string? Subsector { get; set; }

        [JsonPropertyName("phoneDetails")]
        public List<PhoneDetail>? PhoneDetails { get; set; }

        [JsonPropertyName("emailDetails")]
        public List<EmailDetail>? EmailDetails { get; set; }

        [JsonPropertyName("addressDetails")]
        public List<AddressDetail>? AddressDetails { get; set; }

        [JsonPropertyName("riskRatingDetails")]
        public List<RiskRatingDetail>? RiskRatingDetails { get; set; }

        [JsonPropertyName("relatedParty")]
        public List<RelatedParty>? RelatedParty { get; set; }

        [JsonPropertyName("productPreferenceDetails")]
        public List<ProductPreferenceDetail>? ProductPreferenceDetails { get; set; }

        [JsonPropertyName("communicationPreferenceDetails")]
        public List<CommunicationPreferenceDetail>? CommunicationPreferenceDetails { get; set; }

        [JsonPropertyName("customerDocuments")]
        public List<CustomerDocument>? CustomerDocuments { get; set; }

        [JsonPropertyName("relationshipManagerDetails")]
        public List<RelationshipManagerDetail>? RelationshipManagerDetails { get; set; }

        [JsonPropertyName("keyRepresentativeName")]
        public required string KeyRepresentativeName { get; set; }

        [JsonPropertyName("kycStatus")]
        public required string KycStatus { get; set; }

        [JsonPropertyName("reviewedDate")]
        public required string ReviewedDate { get; set; }
    }
    public class AddressDetail
    {
        [JsonPropertyName("addressFormat")]
        public string? AddressFormat { get; set; }

        [JsonPropertyName("addressType")]
        public string? AddressType { get; set; }

        [JsonPropertyName("line1")]
        public string? Line1 { get; set; }

        [JsonPropertyName("line2")]
        public string? Line2 { get; set; }

        [JsonPropertyName("line3")]
        public string? Line3 { get; set; }

        [JsonPropertyName("houseNumber")]
        public string? HouseNumber { get; set; }

        [JsonPropertyName("buildingLevel")]
        public string? BuildingLevel { get; set; }

        [JsonPropertyName("premiseName")]
        public string? PremiseName { get; set; }

        [JsonPropertyName("streetNumber")]
        public string? StreetNumber { get; set; }

        [JsonPropertyName("streetName")]
        public string? StreetName { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("suburb")]
        public string? Suburb { get; set; }

        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("residingSince")]
        public string? ResidingSince { get; set; }

        [JsonPropertyName("residingTill")]
        public string? ResidingTill { get; set; }

        [JsonPropertyName("preferredAddress")]
        public string? PreferredAddress { get; set; }

        [JsonPropertyName("holdMail")]
        public string? HoldMail { get; set; }

        [JsonPropertyName("holdMailInitiatedby")]
        public string? HoldMailInitiatedby { get; set; }

        [JsonPropertyName("holdMailReason")]
        public string? HoldMailReason { get; set; }

        [JsonPropertyName("holdMailStartDate")]
        public string? HoldMailStartDate { get; set; }

        [JsonPropertyName("holdmailEndDate")]
        public string? HoldmailEndDate { get; set; }

        [JsonPropertyName("ownershipType")]
        public string? OwnershipType { get; set; }

        [JsonPropertyName("LGA")]
        public string? LGA { get; set; }

        [JsonPropertyName("SocialMediaHandle")]
        public string? SocialMediaHandle { get; set; }
    }

    public class CommunicationPreferenceDetail
    {
        [JsonPropertyName("doNotDisturb")]
        public string? DoNotDisturb { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("channelType")]
        public string? ChannelType { get; set; }

        [JsonPropertyName("modeOfCommunication")]
        public string? ModeOfCommunication { get; set; }

        [JsonPropertyName("preferenceStartDate")]
        public string? PreferenceStartDate { get; set; }

        [JsonPropertyName("preferenceEndDate")]
        public string? PreferenceEndDate { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }
    }

    //public class CustomerDocument
    //{
    //    [JsonPropertyName("documentNumber")]
    //    public string DocumentNumber { get; set; }

    //    [JsonPropertyName("documentForDetails")]
    //    public List<DocumentForDetail> DocumentForDetails { get; set; }

    //    [JsonPropertyName("documentType")]
    //    public string DocumentType { get; set; }

    //    [JsonPropertyName("issueDate")]
    //    public string IssueDate { get; set; }

    //    [JsonPropertyName("expiryDate")]
    //    public string ExpiryDate { get; set; }

    //    [JsonPropertyName("placeOfIssue")]
    //    public string PlaceOfIssue { get; set; }

    //    [JsonPropertyName("issuingAuthority")]
    //    public string IssuingAuthority { get; set; }

    //    [JsonPropertyName("countryOfIssue")]
    //    public string CountryOfIssue { get; set; }
    //}

    //public class DocumentForDetail
    //{
    //    [JsonPropertyName("documentFor")]
    //    public string DocumentFor { get; set; }
    //}

    public class EmailDetail
    {
        [JsonPropertyName("emailType")]
        public string? EmailType { get; set; }

        [JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("preferredEmailAddress")]
        public string? PreferredEmailAddress { get; set; }
    }

    public class PhoneDetail
    {
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("phoneType")]
        public string? PhoneType { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("preferredPhoneNumber")]
        public string? PreferredPhoneNumber { get; set; }
    }

    public class ProductPreferenceDetail
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("preferenceStartDate")]
        public string? PreferenceStartDate { get; set; }

        [JsonPropertyName("preferenceEndDate")]
        public string? PreferenceEndDate { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }
    }

    public class RelatedParty
    {
        [JsonPropertyName("relationship")]
        public string? Relationship { get; set; }

        [JsonPropertyName("representerId")]
        public string? RepresenterId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("emailId")]
        public string? EmailId { get; set; }

        [JsonPropertyName("representerCifType")]
        public string? RepresenterCifType { get; set; }

        [JsonPropertyName("representerCifSubType")]
        public string? RepresenterCifSubType { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }

        [JsonPropertyName("documentReferenceId")]
        public string? DocumentReferenceId { get; set; }

        [JsonPropertyName("percentage")]
        public string? Percentage { get; set; }

        [JsonPropertyName("designation")]
        public string? Designation { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("BVN")]
        public string? BVN { get; set; }

        [JsonPropertyName("NIN")]
        public string? NIN { get; set; }

        [JsonPropertyName("authorizedSignatory")]
        public string? AuthorizedSignatory { get; set; }
    }

    public class RelationshipManagerDetail
    {
        [JsonPropertyName("relationshipManagerId")]
        public string? RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string? RelationshipManagerName { get; set; }

        [JsonPropertyName("relationshipManagerPhoneNumber")]
        public string? RelationshipManagerPhoneNumber { get; set; }

        [JsonPropertyName("relationshipManagerEmailAddress")]
        public string? RelationshipManagerEmailAddress { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string? PrimaryRelationshipManager { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("pcCode")]
        public string? PcCode { get; set; }

        [JsonPropertyName("pcName")]
        public string? PcName { get; set; }

        [JsonPropertyName("pcEmail")]
        public string? PcEmail { get; set; }

        [JsonPropertyName("pcPhNumber")]
        public string? PcPhNumber { get; set; }

        [JsonPropertyName("teamName")]
        public string? TeamName { get; set; }

        [JsonPropertyName("teamEmail")]
        public string? TeamEmail { get; set; }
    }

    public class RiskRatingDetail
    {
        [JsonPropertyName("riskRatingModel")]
        public string? RiskRatingModel { get; set; }

        [JsonPropertyName("riskRating")]
        public string? RiskRating { get; set; }

        [JsonPropertyName("customerRating")]
        public string? CustomerRating { get; set; }

        [JsonPropertyName("riskRatingDate")]
        public string? RiskRatingDate { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }

        [JsonPropertyName("riskProfileValidTill")]
        public string? RiskProfileValidTill { get; set; }

        [JsonPropertyName("overrideRiskRating")]
        public string? OverrideRiskRating { get; set; }

        [JsonPropertyName("riskRatingOverriddenOn")]
        public string? RiskRatingOverriddenOn { get; set; }

        [JsonPropertyName("riskRatingOverriddenBy")]
        public string? RiskRatingOverriddenBy { get; set; }
    }


}
