using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    // Create Corporate Customer Response Object
    public class CreateCorpCustomerResponse
    {
        [JsonPropertyName("customerIdentifier")]
        public string CustomerIdentifier { get; set; }

        [JsonPropertyName("subEntityType")]
        public string SubEntityType { get; set; }

        [JsonPropertyName("corporateName")]
        public string CorporateName { get; set; }

        [JsonPropertyName("dateOfIncorporation")]
        public DateTime? DateOfIncorporation { get; set; }

        [JsonPropertyName("registrationNumber")]
        public string RegistrationNumber { get; set; }

        [JsonPropertyName("countryOfIncorporation")]
        public string CountryOfIncorporation { get; set; }

        [JsonPropertyName("corporateEntityType")]
        public string CorporateEntityType { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string PrimaryBranch { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public DateTime? RelationshipStartDate { get; set; }

        [JsonPropertyName("customerAddress")]
        public List<CustomerAddress> CustomerAddress { get; set; }

        [JsonPropertyName("customerCommunicationPreference")]
        public List<object> CustomerCommunicationPreference { get; set; }

        [JsonPropertyName("customerDocument")]
        public List<CustomerDocument> CustomerDocument { get; set; }

        [JsonPropertyName("customerEmail")]
        public List<object> CustomerEmail { get; set; }

        [JsonPropertyName("customerPhone")]
        public List<object> CustomerPhone { get; set; }

        [JsonPropertyName("customerRiskRatingDetails")]
        public List<object> CustomerRiskRatingDetails { get; set; }

        [JsonPropertyName("customerRelatedParty")]
        public List<object> CustomerRelatedParty { get; set; }

        [JsonPropertyName("newRelationshipManager")]
        public List<NewRelationshipManager> NewRelationshipManager { get; set; }

        [JsonPropertyName("customerProductPreference")]
        public List<object> CustomerProductPreference { get; set; }
    }
    public class CustomerAddress
    {
        [JsonPropertyName("addressId")]
        public string AddressId { get; set; }

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
        public string ResidingSince { get; set; }

        [JsonPropertyName("residingTill")]
        public string ResidingTill { get; set; }

        [JsonPropertyName("preferredAddress")]
        public string PreferredAddress { get; set; }

        [JsonPropertyName("holdMail")]
        public string HoldMail { get; set; }
    }

    public class CustomerDocument
    {
        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; }

        [JsonPropertyName("documentNumber")]
        public string DocumentNumber { get; set; }

        [JsonPropertyName("documentForDetails")]
        public List<DocumentForDetail> DocumentForDetails { get; set; }

        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonPropertyName("issueDate")]
        public string IssueDate { get; set; }

        [JsonPropertyName("expiryDate")]
        public string ExpiryDate { get; set; }

        [JsonPropertyName("placeOfIssue")]
        public string PlaceOfIssue { get; set; }

        [JsonPropertyName("countryOfIssue")]
        public string CountryOfIssue { get; set; }
    }

    public class DocumentForDetail
    {
        [JsonPropertyName("documentFor")]
        public List<string> DocumentFor { get; set; }
    }

    public class NewRelationshipManager
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("relationshipManagerId")]
        public string RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string RelationshipManagerName { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string PrimaryRelationshipManager { get; set; }

        [JsonPropertyName("pcCode")]
        public object PcCode { get; set; }

        [JsonPropertyName("pcName")]
        public object PcName { get; set; }

        [JsonPropertyName("pcEmail")]
        public object PcEmail { get; set; }

        [JsonPropertyName("pcPhNumber")]
        public object PcPhNumber { get; set; }

        [JsonPropertyName("teamName")]
        public object TeamName { get; set; }

        [JsonPropertyName("teamEmail")]
        public object TeamEmail { get; set; }
    }
}
