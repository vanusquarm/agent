using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class CreateRetailCustomerResponseDto2
    {
        [JsonPropertyName("customerIdentifier")]
        public string CustomerIdentifier { get; set; }

        [JsonPropertyName("salutation")]
        public string Salutation { get; set; }

        [JsonPropertyName("subEntityType")]
        public string SubEntityType { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("dob")]
        public DateTime Dob { get; set; }

        [JsonPropertyName("minor")]
        public string Minor { get; set; }

        [JsonPropertyName("seniorCitizen")]
        public string SeniorCitizen { get; set; }

        [JsonPropertyName("maritalStatus")]
        public string MaritalStatus { get; set; }

        [JsonPropertyName("motherMaidenName")]
        public string MotherMaidenName { get; set; }

        [JsonPropertyName("taxIdType")]
        public string TaxIdType { get; set; }

        [JsonPropertyName("taxIdentificationNumber")]
        public string TaxIdentificationNumber { get; set; }

        [JsonPropertyName("taxExemptionCode")]
        public string TaxExemptionCode { get; set; }

        [JsonPropertyName("taxResident")]
        public string TaxResident { get; set; }

        [JsonPropertyName("segment")]
        public string Segment { get; set; }

        [JsonPropertyName("subsegment")]
        public string Subsegment { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string PrimaryBranch { get; set; }

        [JsonPropertyName("customerAddress")]
        public List<CustomerAddress22> CustomerAddress { get; set; }

        [JsonPropertyName("customerDocument")]
        public List<CustomerDocument22> CustomerDocument { get; set; }

        [JsonPropertyName("customerEmail")]
        public List<string> CustomerEmail { get; set; }

        [JsonPropertyName("customerPhone")]
        public List<CustomerPhone22> CustomerPhone { get; set; }

        [JsonPropertyName("customerEmploymentDetails")]
        public List<object> CustomerEmploymentDetails { get; set; }

        [JsonPropertyName("customerRiskRatingDetails")]
        public List<object> CustomerRiskRatingDetails { get; set; }

        [JsonPropertyName("customerRelatedParty")]
        public List<object> CustomerRelatedParty { get; set; }

        [JsonPropertyName("newRelationshipManager")]
        public List<RelationshipManager22> NewRelationshipManager { get; set; }

        [JsonPropertyName("customerProductPreference")]
        public List<object> CustomerProductPreference { get; set; }

        [JsonPropertyName("customerCommunicationPreference")]
        public List<object> CustomerCommunicationPreference { get; set; }

        [JsonPropertyName("customerEducationDetails")]
        public List<object> CustomerEducationDetails { get; set; }
    }

    public class CustomerAddress22
    {
        [JsonPropertyName("addressFormat")]
        public string AddressFormat { get; set; }

        [JsonPropertyName("addressType")]
        public string AddressType { get; set; }

        [JsonPropertyName("line1")]
        public string Line1 { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("residingSince")]
        public DateTime ResidingSince { get; set; }

        [JsonPropertyName("residingTill")]
        public DateTime ResidingTill { get; set; }

        [JsonPropertyName("preferredAddress")]
        public string PreferredAddress { get; set; }

        [JsonPropertyName("holdMail")]
        public string HoldMail { get; set; }
    }

    public class CustomerDocument22
    {
        [JsonPropertyName("documentNumber")]
        public string DocumentNumber { get; set; }

        [JsonPropertyName("documentForDetails")]
        public List<DocumentForDetail> DocumentForDetails { get; set; }

        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonPropertyName("issueDate")]
        public DateTime IssueDate { get; set; }

        [JsonPropertyName("expiryDate")]
        public DateTime ExpiryDate { get; set; }

        [JsonPropertyName("placeOfIssue")]
        public string PlaceOfIssue { get; set; }

        [JsonPropertyName("countryOfIssue")]
        public string CountryOfIssue { get; set; }
    }

    public class DocumentForDetail22
    {
        [JsonPropertyName("documentFor")]
        public List<string> DocumentFor { get; set; }
    }

    public class CustomerPhone22
    {
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("phoneType")]
        public string PhoneType { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("preferredPhoneNumber")]
        public string PreferredPhoneNumber { get; set; }
    }

    public class RelationshipManager22
    {
        [JsonPropertyName("relationshipManagerId")]
        public string RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string RelationshipManagerName { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string PrimaryRelationshipManager { get; set; }
    }
}
