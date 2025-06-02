using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Finacle.Core
{    public class CreateCustomerDto
    {
        [JsonPropertyName("customerIdentifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CustomerId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("subEntityType")]
        public string SubEntityType { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("dob")]
        public string Dob { get; set; }

        [JsonPropertyName("addressDetails")]
        public List<AddressDetail> AddressDetails { get; set; }

        [JsonPropertyName("customerDocuments")]
        public List<CustomerDocument> CustomerDocuments { get; set; }

        [JsonPropertyName("relationshipManagerDetails")]
        public List<RelationshipManagerDetail> RelationshipManagerDetails { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string PrimaryBranch { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public string RelationshipStartDate { get; set; }

        [JsonPropertyName("countryOfPrimaryCitizenship")]
        public string CountryOfPrimaryCitizenship { get; set; }

        [JsonPropertyName("taxResident")]
        public string TaxResident { get; set; }

        [JsonPropertyName("taxExemptionCode")]
        public string TaxExemptionCode { get; set; }
    }

    public class AddressDetail
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
        public string ResidingSince { get; set; }

        [JsonPropertyName("residingTill")]
        public string ResidingTill { get; set; }

        [JsonPropertyName("preferredAddress")]
        public string PreferredAddress { get; set; }
    }

    public class CustomerDocument
    {
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
        public string DocumentFor { get; set; }
    }

    public class RelationshipManagerDetail
    {
        [JsonPropertyName("relationshipManagerId")]
        public string RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string RelationshipManagerName { get; set; }

        [JsonPropertyName("relationshipManagerPhoneNumber")]
        public string RelationshipManagerPhoneNumber { get; set; }

        [JsonPropertyName("relationshipManagerEmailAddress")]
        public string RelationshipManagerEmailAddress { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string PrimaryRelationshipManager { get; set; }
    }

}
