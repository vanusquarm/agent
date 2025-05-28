using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Customers.API.Entities.DTOs
{
    public class GetRetailCustomerResponse
    {
        [JsonPropertyName("customerIdentifier")]
        public string CustomerIdentifier { get; set; }

        [JsonPropertyName("salutation")]
        public string Salutation { get; set; }

        [JsonPropertyName("BVN")]
        public object BVN { get; set; }

        [JsonPropertyName("TierInformation")]
        public object TierInformation { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("suffix")]
        public object Suffix { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("dob")]
        public string Dob { get; set; }

        [JsonPropertyName("minor")]
        public string Minor { get; set; }

        [JsonPropertyName("seniorCitizen")]
        public string SeniorCitizen { get; set; }

        [JsonPropertyName("guardianDetails")]
        public List<object> GuardianDetails { get; set; }

        [JsonPropertyName("phoneDetails")]
        public List<object> PhoneDetails { get; set; }

        [JsonPropertyName("emailDetails")]
        public List<object> EmailDetails { get; set; }

        [JsonPropertyName("addressDetails")]
        public List<AddressDetail> AddressDetails { get; set; }

        [JsonPropertyName("customerDocuments")]
        public List<CustomerDocument> CustomerDocuments { get; set; }

        [JsonPropertyName("relationshipManagerDetails")]
        public List<RelationshipManagerDetail> RelationshipManagerDetails { get; set; }

        [JsonPropertyName("employmentDetails")]
        public List<object> EmploymentDetails { get; set; }

        [JsonPropertyName("riskRatingDetails")]
        public List<object> RiskRatingDetails { get; set; }

        [JsonPropertyName("relatedParty")]
        public List<object> RelatedParty { get; set; }

        [JsonPropertyName("productPreferenceDetails")]
        public List<object> ProductPreferenceDetails { get; set; }

        [JsonPropertyName("communicationPreferenceDetails")]
        public List<object> CommunicationPreferenceDetails { get; set; }

        [JsonPropertyName("educationDetails")]
        public List<object> EducationDetails { get; set; }

        [JsonPropertyName("primaryBranch")]
        public string PrimaryBranch { get; set; }

        [JsonPropertyName("segment")]
        public object Segment { get; set; }

        [JsonPropertyName("subsegment")]
        public object Subsegment { get; set; }

        [JsonPropertyName("relationshipStartDate")]
        public string RelationshipStartDate { get; set; }

        [JsonPropertyName("countryOfPrimaryCitizenship")]
        public string CountryOfPrimaryCitizenship { get; set; }

        [JsonPropertyName("taxResident")]
        public string TaxResident { get; set; }

        [JsonPropertyName("taxIdType")]
        public object TaxIdType { get; set; }

        [JsonPropertyName("taxIdentificationNumber")]
        public object TaxIdentificationNumber { get; set; }

        [JsonPropertyName("taxExemptionCode")]
        public string TaxExemptionCode { get; set; }

        [JsonPropertyName("maritalStatus")]
        public object MaritalStatus { get; set; }

        [JsonPropertyName("motherMaidenName")]
        public object MotherMaidenName { get; set; }

        [JsonPropertyName("kycStatus")]
        public string KycStatus { get; set; }

        [JsonPropertyName("reviewedDate")]
        public string ReviewedDate { get; set; }
    }
    //public class AddressDetail
    //{
    //    [JsonPropertyName("addressId")]
    //    public string AddressId { get; set; }

    //    [JsonPropertyName("addressFormat")]
    //    public string AddressFormat { get; set; }

    //    [JsonPropertyName("addressType")]
    //    public string AddressType { get; set; }

    //    [JsonPropertyName("line1")]
    //    public string Line1 { get; set; }

    //    [JsonPropertyName("line2")]
    //    public string Line2 { get; set; }

    //    [JsonPropertyName("line3")]
    //    public string Line3 { get; set; }

    //    [JsonPropertyName("houseNumber")]
    //    public string HouseNumber { get; set; }

    //    [JsonPropertyName("buildingLevel")]
    //    public object BuildingLevel { get; set; }

    //    [JsonPropertyName("premiseName")]
    //    public string PremiseName { get; set; }

    //    [JsonPropertyName("streetNumber")]
    //    public string StreetNumber { get; set; }

    //    [JsonPropertyName("streetName")]
    //    public string StreetName { get; set; }

    //    [JsonPropertyName("locality")]
    //    public object Locality { get; set; }

    //    [JsonPropertyName("suburb")]
    //    public object Suburb { get; set; }

    //    [JsonPropertyName("town")]
    //    public string Town { get; set; }

    //    [JsonPropertyName("postalCode")]
    //    public string PostalCode { get; set; }

    //    [JsonPropertyName("country")]
    //    public string Country { get; set; }

    //    [JsonPropertyName("state")]
    //    public string State { get; set; }

    //    [JsonPropertyName("city")]
    //    public string City { get; set; }

    //    [JsonPropertyName("residingSince")]
    //    public string ResidingSince { get; set; }

    //    [JsonPropertyName("residingTill")]
    //    public string ResidingTill { get; set; }

    //    [JsonPropertyName("preferredAddress")]
    //    public string PreferredAddress { get; set; }

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

    //    [JsonPropertyName("documentNumber")]
    //    public string DocumentNumber { get; set; }

    //    [JsonPropertyName("documentForDetails")]
    //    public List<DocumentForDetail> DocumentForDetails { get; set; }

    //    [JsonPropertyName("documentType")]
    //    public string DocumentType { get; set; }

    //    [JsonPropertyName("issueDate")]
    //    public object IssueDate { get; set; }

    //    [JsonPropertyName("expiryDate")]
    //    public object ExpiryDate { get; set; }

    //    [JsonPropertyName("placeOfIssue")]
    //    public object PlaceOfIssue { get; set; }

    //    [JsonPropertyName("issuingAuthority")]
    //    public object IssuingAuthority { get; set; }

    //    [JsonPropertyName("countryOfIssue")]
    //    public object CountryOfIssue { get; set; }
    //}

    //public class DocumentForDetail
    //{
    //    [JsonPropertyName("documentFor")]
    //    public List<string> DocumentFor { get; set; }
    //}

    //public class RelationshipManagerDetail
    //{
    //    [JsonPropertyName("department")]
    //    public object Department { get; set; }

    //    [JsonPropertyName("relationshipManagerId")]
    //    public string RelationshipManagerId { get; set; }

    //    [JsonPropertyName("relationshipManagerName")]
    //    public string RelationshipManagerName { get; set; }

    //    [JsonPropertyName("primaryRelationshipManager")]
    //    public string PrimaryRelationshipManager { get; set; }

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

    // Get By Phone Number response
    public class CustomerDetail
    {
        [JsonPropertyName("cifId")]
        public string CifId { get; set; }

        [JsonPropertyName("Foracid")]
        public string Foracid { get; set; }

        [JsonPropertyName("SchemeType")]
        public string SchemeType { get; set; }

        [JsonPropertyName("SchemeCode")]
        public string SchemeCode { get; set; }

        [JsonPropertyName("Firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("MiddleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("Lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("CifStatus")]
        public string CifStatus { get; set; }

        [JsonPropertyName("DOB")]
        public string DOB { get; set; }

        [JsonPropertyName("Gender")]
        public string Gender { get; set; }

        [JsonPropertyName("BVN")]
        public string BVN { get; set; }

        [JsonPropertyName("Tier")]
        public string Tier { get; set; }

        [JsonPropertyName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("CustomerType")]
        public string CustomerType { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("balance")]
        public string Balance { get; set; }

        [JsonPropertyName("acctOpendate")]
        public string AcctOpendate { get; set; }

        [JsonPropertyName("acctSolId")]
        public string AcctSolId { get; set; }
    }

    public class CustomerDetailsResponse
    {
        [JsonPropertyName("CustomerDetails")]
        public List<CustomerDetail> CustomerDetails { get; set; }

        [JsonPropertyName("errorFlg")]
        public string? ErrorFlg { get; set; }

        [JsonPropertyName("errorFlg")]
        public string? ErrorMsg { get; set; }

        public GetRetailCustomerResponse Transform()
        {
            var response = new GetRetailCustomerResponse();
            response.CustomerIdentifier = CustomerDetails[0].CifId;
            response.FirstName = CustomerDetails[0].FirstName;
            response.MiddleName = CustomerDetails[0].MiddleName;
            response.LastName = CustomerDetails[0].LastName;
            response.PrimaryBranch = CustomerDetails[0].AcctSolId;
            response.RelationshipStartDate = CustomerDetails[0].AcctOpendate;
            response.TierInformation = CustomerDetails[0].Tier;
            response.KycStatus = CustomerDetails[0].CifStatus == "A" ? "APPROVED" : "PENDING";
            response.Gender = CustomerDetails[0].Gender;
            response.Dob = CustomerDetails[0].DOB;
            response.CustomerName = $"{CustomerDetails[0].FirstName} {CustomerDetails[0].MiddleName} {CustomerDetails[0].LastName}";
            response.EmailDetails = new List<object>
            {
                new
                {
                    Email = CustomerDetails[0].Email
                }
            };
            response.AddressDetails = new List<AddressDetail>
            {
                new AddressDetail
                {
                    Line1 = CustomerDetails[0].Address
                }
            };
            response.PhoneDetails = new List<object>
            {
                new
                {
                    CustomerDetails[0].PhoneNumber
                }
            };
            return response;
        }
    }
}
