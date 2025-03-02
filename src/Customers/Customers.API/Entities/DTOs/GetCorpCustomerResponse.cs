using Customers.API.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class GetCorpCustomerResponse
    {
        [JsonPropertyName("customerIdentifier")]
        public string CustomerIdentifier { get; set; }

        [JsonPropertyName("corporateName")]
        public string CorporateName { get; set; }

        [JsonPropertyName("BVN")]
        public string BVN { get; set; }

        [JsonPropertyName("TierInformation")]
        public string TierInformation { get; set; }

        [JsonPropertyName("dateOfIncorporation")]
        public string DateOfIncorporation { get; set; }

        [JsonPropertyName("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonPropertyName("countryOfIncorporation")]
        public string CountryOfIncorporation { get; set; }

        [JsonPropertyName("taxIdentificationNumber")]
        public string TaxIdentificationNumber { get; set; }

        [JsonPropertyName("corporateEntityType")]
        public string CorporateEntityType { get; set; }

        [JsonPropertyName("corporateClassification")]
        public string CorporateClassification { get; set; }

        [JsonPropertyName("standardIndustryCode")]
        public string StandardIndustryCode { get; set; }

        [JsonPropertyName("segment")]
        public string Segment { get; set; }

        [JsonPropertyName("taxIdType")]
        public string TaxIdType { get; set; }

        [JsonPropertyName("publiclyHeld")]
        public string PubliclyHeld { get; set; }

        [JsonPropertyName("healthCode")]
        public string HealthCode { get; set; }

        [JsonPropertyName("noOfEmployees")]
        public int NoOfEmployees { get; set; }

        [JsonPropertyName("subsegment")]
        public string Subsegment { get; set; }

        [JsonPropertyName("sector")]
        public string Sector { get; set; }

        [JsonPropertyName("subsector")]
        public string Subsector { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string PrimaryBranch { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public string RelationshipStartDate { get; set; }

        [JsonPropertyName("reviewedDate")]
        public string ReviewedDate { get; set; }

        [JsonPropertyName("kycStatus")]
        public string KycStatus { get; set; }

        [JsonPropertyName("relationshipManagerDetails")]
        public List<RelationshipManagerDetail> RelationshipManagerDetails { get; set; }

        [JsonPropertyName("addressDetails")]
        public List<AddressDetail> AddressDetails { get; set; }

        [JsonPropertyName("emailDetails")]
        public List<EmailDetail> EmailDetails { get; set; }

        [JsonPropertyName("phoneDetails")]
        public List<PhoneDetail> PhoneDetails { get; set; }

        [JsonPropertyName("customerDocuments")]
        public List<CustomerDocument> CustomerDocuments { get; set; }

        [JsonPropertyName("riskRatingDetails")]
        public List<RiskRatingDetail> RiskRatingDetails { get; set; }

        [JsonPropertyName("relatedParty")]
        public List<object> RelatedParty { get; set; }

        [JsonPropertyName("productPreferenceDetails")]
        public List<ProductPreferenceDetail> ProductPreferenceDetails { get; set; }

        [JsonPropertyName("communicationPreferenceDetails")]
        public List<object> CommunicationPreferenceDetails { get; set; }
    }

    //public class AddressDetail
    //{
    //    [JsonPropertyName("addressId")]
    //    public string AddressId { get; set; }

    //    [JsonPropertyName("addressFormat")]
    //    public string AddressFormat { get; set; }

    //    [JsonPropertyName("addressType")]
    //    public string AddressType { get; set; }

    //    [JsonPropertyName("residingSince")]
    //    public string ResidingSince { get; set; }

    //    [JsonPropertyName("residingTill")]
    //    public string ResidingTill { get; set; }

    //    [JsonPropertyName("line1")]
    //    public string Line1 { get; set; }

    //    [JsonPropertyName("line2")]
    //    public object Line2 { get; set; }

    //    [JsonPropertyName("line3")]
    //    public object Line3 { get; set; }

    //    [JsonPropertyName("houseNumber")]
    //    public object HouseNumber { get; set; }

    //    [JsonPropertyName("buildingLevel")]
    //    public object BuildingLevel { get; set; }

    //    [JsonPropertyName("premiseName")]
    //    public object PremiseName { get; set; }

    //    [JsonPropertyName("streetNumber")]
    //    public object StreetNumber { get; set; }

    //    [JsonPropertyName("streetName")]
    //    public object StreetName { get; set; }

    //    [JsonPropertyName("locality")]
    //    public object Locality { get; set; }

    //    [JsonPropertyName("suburb")]
    //    public object Suburb { get; set; }

    //    [JsonPropertyName("town")]
    //    public object Town { get; set; }

    //    [JsonPropertyName("postalCode")]
    //    public string PostalCode { get; set; }

    //    [JsonPropertyName("country")]
    //    public string Country { get; set; }

    //    [JsonPropertyName("state")]
    //    public string State { get; set; }

    //    [JsonPropertyName("city")]
    //    public string City { get; set; }

    //    [JsonPropertyName("holdMail")]
    //    public string HoldMail { get; set; }

    //    [JsonPropertyName("holdMailInitiatedby")]
    //    public object HoldMailInitiatedby { get; set; }

    //    [JsonPropertyName("holdMailReason")]
    //    public object HoldMailReason { get; set; }

    //    [JsonPropertyName("holdMailStartDate")]
    //    public object HoldMailStartDate { get; set; }

    //    [JsonPropertyName("holdmailEndDate")]
    //    public object HoldmailEndDate { get; set; }

    //    [JsonPropertyName("ownershipType")]
    //    public object OwnershipType { get; set; }

    //    [JsonPropertyName("LGA")]
    //    public object LGA { get; set; }

    //    [JsonPropertyName("SocialMediaHandle")]
    //    public object SocialMediaHandle { get; set; }
    //}

    //public class CustomerDocument
    //{
    //    [JsonPropertyName("documentId")]
    //    public string DocumentId { get; set; }

    //    [JsonPropertyName("documentType")]
    //    public string DocumentType { get; set; }

    //    [JsonPropertyName("documentNumber")]
    //    public string DocumentNumber { get; set; }

    //    [JsonPropertyName("issuingAuthority")]
    //    public object IssuingAuthority { get; set; }

    //    [JsonPropertyName("countryOfIssue")]
    //    public string CountryOfIssue { get; set; }

    //    [JsonPropertyName("placeOfIssue")]
    //    public string PlaceOfIssue { get; set; }

    //    [JsonPropertyName("issueDate")]
    //    public string IssueDate { get; set; }

    //    [JsonPropertyName("expiryDate")]
    //    public object ExpiryDate { get; set; }

    //    [JsonPropertyName("documentForDetails")]
    //    public List<DocumentForDetail> DocumentForDetails { get; set; }
    //}

    //public class DocumentForDetail
    //{
    //    [JsonPropertyName("documentFor")]
    //    public List<string> DocumentFor { get; set; }
    //}

    //public class RelationshipManagerDetail
    //{
    //    [JsonPropertyName("relationshipId")]
    //    public string RelationshipId { get; set; }

    //    [JsonPropertyName("relationshipManagerId")]
    //    public string RelationshipManagerId { get; set; }

    //    [JsonPropertyName("relationshipManagerName")]
    //    public string RelationshipManagerName { get; set; }

    //    [JsonPropertyName("primaryRelationshipManager")]
    //    public string PrimaryRelationshipManager { get; set; }

    //    [JsonPropertyName("department")]
    //    public object Department { get; set; }

    //    [JsonPropertyName("pcCode")]
    //    public object PcCode { get; set; }

    //    [JsonPropertyName("pcName")]
    //    public object PcName { get; set; }

    //    [JsonPropertyName("pcEmail")]
    //    public object PcEmail { get; set; }

    //    [JsonPropertyName("pcPhNumber")]
    //    public object PcPhNumber { get; set; }

    //    [JsonPropertyName("teamName")]
    //    public object TeamName { get; set; }

    //    [JsonPropertyName("teamEmail")]
    //    public object TeamEmail { get; set; }
    //}

}
