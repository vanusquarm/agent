using Finacle.Infrastructure.Utils;
using MassTransit.Futures.Contracts;
using System.Text.Json.Serialization;

namespace Customers.API.Entities
{
    public class CreateCustomerDto
    {
        [JsonPropertyName("customerIdentifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CustomerId { get; set; }

        [JsonPropertyName("salutation")]
        public string? Salutation { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("subEntityType")]
        public string? SubEntityType { get; set; } = "CUST";

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("dob")]
        public string? Dob { get; set; }

        [JsonPropertyName("minor")]
        public string? Minor { get; set; }

        [JsonPropertyName("seniorCitizen")]
        public string? SeniorCitizen { get; set; }

        [JsonPropertyName("guardianDetails")]
        public GuardianDetails? GuardianDetails { get; set; }

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

        [JsonPropertyName("employmentDetails")]
        public List<EmploymentDetail>? EmploymentDetails { get; set; }

        [JsonPropertyName("riskRatingDetails")]
        public List<RiskRatingDetail>? RiskRatingDetails { get; set; }

        [JsonPropertyName("relatedParty")]
        public List<object>? RelatedParty { get; set; } = new List<object>();

        [JsonPropertyName("productPreferenceDetails")]
        public List<object>? ProductPreferenceDetails { get; set; } = new List<object>();

        [JsonPropertyName("communicationPreferenceDetails")]
        public List<object>? CommunicationPreferenceDetails { get; set; } = new List<object>();

        [JsonPropertyName("educationDetails")]
        public List<object>? EducationDetails { get; set; } = new List<object>();

        [JsonPropertyName("primaryBranch")]
        public string? PrimaryBranch { get; set; }

        [JsonPropertyName("segment")]
        public string? Segment { get; set; }

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

        public CreateCustomerDto(CreateBasicCustomerDto basicCustomerDto)
        {
            Salutation = basicCustomerDto.Salutation;
            FirstName = basicCustomerDto.FirstName;
            MiddleName = basicCustomerDto.MiddleName;
            LastName = basicCustomerDto.LastName;
            Gender = basicCustomerDto.Gender;
            Dob = basicCustomerDto.Dob;
            Minor = basicCustomerDto.Minor;
            SeniorCitizen = basicCustomerDto.SeniorCitizen;
            GuardianDetails = basicCustomerDto.GuardianId != null ? new GuardianDetails(basicCustomerDto.GuardianId) : null;
            PhoneDetails = basicCustomerDto.PhoneNumber != null ? new List<PhoneDetail> { new PhoneDetail(basicCustomerDto.PhoneNumber) } : null;
            EmailDetails = basicCustomerDto.EmailAddress != null ? new List<EmailDetail> { new EmailDetail(basicCustomerDto.EmailAddress) } : null;
            AddressDetails = new List<AddressDetail>
            {
                new AddressDetail(
                    basicCustomerDto.HouseNumber,
                    basicCustomerDto.Street ?? string.Empty,
                    basicCustomerDto.City ?? string.Empty,
                    basicCustomerDto.State ?? string.Empty,
                    basicCustomerDto.Country ?? "Ghana",
                    basicCustomerDto.PostalCode ?? string.Empty,
                    basicCustomerDto.GPS ?? string.Empty,
                    DateTime.UtcNow.ToString("yyyy-MM-dd")
                )
            };
            CustomerDocuments = basicCustomerDto.DocumentName != null ? new List<CustomerDocument>
            {
                new CustomerDocument(
                    basicCustomerDto.DocumentName,
                    basicCustomerDto.DocumentNumber,
                    basicCustomerDto.DocumentType ?? "NID",
                    basicCustomerDto.IssueDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    basicCustomerDto.ExpiryDate ?? DateTime.UtcNow.AddYears(10).ToString("yyyy-MM-dd"),
                    basicCustomerDto.PlaceOfIssue ?? "Accra",
                    basicCustomerDto.CountryOfIssue ?? "Ghana"
                )
            } : null;
            RelationshipManagerDetails = basicCustomerDto.RelationshipManagerId != null ? new List<RelationshipManagerDetail>
            {
                new RelationshipManagerDetail(
                    basicCustomerDto.RelationshipManagerId,
                    basicCustomerDto.RelationshipManagerName ?? "Default RM",
                    basicCustomerDto.RelationshipManagerEmail ?? "",
                    basicCustomerDto.RelationshipManagerPcCode)
            } : null;
            EmploymentDetails = basicCustomerDto.EmployerName != null ? new List<EmploymentDetail>
            {
                new EmploymentDetail(
                    basicCustomerDto.OccupationCode,
                    basicCustomerDto.EmployerName,
                    basicCustomerDto.EmploymentStartDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd")
                )
            } : null;
            RiskRatingDetails = basicCustomerDto.RiskRating != null ? new List<RiskRatingDetail>
            {
                new RiskRatingDetail(
                    basicCustomerDto.RiskRating,
                    basicCustomerDto.RiskRatingDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd")
                )
            } : null;
            PrimaryBranch = basicCustomerDto.PrimaryBranch;
            Segment = basicCustomerDto.Segment;
            RelationshipStartDate = basicCustomerDto.RelationshipStartDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
            CountryOfPrimaryCitizenship = basicCustomerDto.CountryOfPrimaryCitizenship;
            TaxResident = basicCustomerDto.TaxResident;
            TaxIdType = basicCustomerDto.TaxIdType;
            TaxIdentificationNumber = basicCustomerDto.TaxIdentificationNumber;
            TaxExemptionCode = basicCustomerDto.TaxExemptionCode;
            EmploymentStatus = basicCustomerDto.EmploymentStatus;
            AnnualSalaryIncome = basicCustomerDto.AnnualSalaryIncome;
            MaritalStatus = basicCustomerDto.MaritalStatus;
            MotherMaidenName = basicCustomerDto.MotherMaidenName;
            KycStatus = basicCustomerDto.KycStatus;
            ReviewedDate = basicCustomerDto.ReviewedDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
        }
    }

    public class CreateBasicCustomerDto
    {
        [JsonPropertyName("customerIdentifier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CustomerId { get; set; }
        public string? Salutation { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Dob { get; set; }
        public string? Minor { get; set; } = "N";
        public string? SeniorCitizen { get; set; } = "N";
        public string? GuardianId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

        // Address Details
        public string HouseNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? GPS { get; set; }
        public string? Country { get; set; }

        // Customer Document Details
        public string? DocumentName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? DocumentType { get; set; } // e.g., "Passport", "National ID"
        public string? IssueDate { get; set; } // e.g., "2023-01-01"
        public string? ExpiryDate { get; set; } // e.g., "2033-01-01"
        public string? PlaceOfIssue { get; set; } // e.g., "Accra"
        public string? CountryOfIssue { get; set; } // e.g., "GH"

        // Relationship Manager Details 
        public string? RelationshipManagerId { get; set; }
        public string? RelationshipManagerName { get; set; }
        public string? RelationshipManagerEmail { get; set; }
        public string RelationshipManagerPcCode { get; set; }

        // Employment Details
        public string OccupationCode { get; set; } = "2104";
        public string? EmployerName { get; set; }
        public string? EmploymentStartDate { get; set; }
        public string? EmploymentStatus { get; set; }
        public int? AnnualSalaryIncome { get; set; }

        // Risk Rating Details
        public string? RiskRating { get; set; }
        public string? RiskRatingDate { get; set; }

        public string? PrimaryBranch { get; set; }
        public string? Segment { get; set; }
        public string? RelationshipStartDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string? CountryOfPrimaryCitizenship { get; set; }
        public string? TaxResident { get; set; }
        public string? TaxIdType { get; set; }
        public string? TaxIdentificationNumber { get; set; }
        public string? TaxExemptionCode { get; set; }
        public string? MaritalStatus { get; set; }
        public string? MotherMaidenName { get; set; }
        public string? KycStatus { get; set; }
        public string? ReviewedDate { get; set; }
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

        [JsonPropertyName("houseNumber")]
        public string HouseNumber { get; set; }

        [JsonPropertyName("streetNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StreetNumber { get; set; }

        [JsonPropertyName("streetName")]
        public string? StreetName { get; set; }

        [JsonPropertyName("locality")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locality { get; set; }

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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ResidingTill { get; set; }

        [JsonPropertyName("preferredAddress")]
        public string? PreferredAddress { get; set; }

        public AddressDetail(string houseNumber, string street, string city, string state, string country, string postalCode, string gps, string residingSince)
        {
            AddressFormat = "STRUCTURED";
            HouseNumber = houseNumber;
            AddressType = "Home";
            Line1 = street;
            StreetName = "NA";
            Line2 = gps;
            PostalCode = postalCode;
            Country = country;
            State = state;
            City = city;
            ResidingSince = residingSince;
            PreferredAddress = "Y"; // Assuming this is the primary address
        }
    }

    public class CustomerDocument
    {
        [JsonPropertyName("documentNumber")]
        public string? DocumentNumber { get; set; }

        [JsonPropertyName("documentForDetails")]
        public List<DocumentForDetail>? DocumentForDetails { get; set; }

        [JsonPropertyName("documentType")]
        public string? DocumentType { get; set; }

        [JsonPropertyName("issueDate")]
        public string? IssueDate { get; set; }

        [JsonPropertyName("expiryDate")]
        public string? ExpiryDate { get; set; }

        [JsonPropertyName("placeOfIssue")]
        public string? PlaceOfIssue { get; set; }

        [JsonPropertyName("countryOfIssue")]
        public string? CountryOfIssue { get; set; }

        public CustomerDocument(string documentName, string documentNumber, string documentType, string issueDate, string expiryDate, string placeOfIssue, string countryOfIssue)
        {
            DocumentNumber = documentNumber;
            DocumentForDetails = new List<DocumentForDetail> { new DocumentForDetail { DocumentFor = documentName ?? "Identity Proof" } };
            DocumentType = documentType;
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            PlaceOfIssue = placeOfIssue;
            CountryOfIssue = countryOfIssue;
        }
    }

    public class DocumentForDetail
    {
        [JsonPropertyName("documentFor")]
        public string? DocumentFor { get; set; }
    }

    public class EmailDetail
    {
        [JsonPropertyName("emailType")]
        public string? EmailType { get; set; }

        [JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("preferredEmailAddress")]
        public string? PreferredEmailAddress { get; set; }

        public EmailDetail(string email)
        {
            EmailType = "HMEML";
            EmailAddress = email;
            PreferredEmailAddress = "Y";
        }
    }

    public class EmploymentDetail
    {
        [JsonPropertyName("natureOfEmployment")]
        public string? NatureOfEmployment { get; set; }

        [JsonPropertyName("employmentStatus")]
        public string? EmploymentStatus { get; set; }

        [JsonPropertyName("employerName")]
        public string? EmployerName { get; set; }

        [JsonPropertyName("occupation")]
        public string? Occupation { get; set; }

        [JsonPropertyName("employmentStartDate")]
        public string? EmploymentStartDate { get; set; }

        [JsonPropertyName("currentEmployment")]
        public string? CurrentEmployment { get; set; }

        public EmploymentDetail(string occupationCode, string employerName, string employmentStartDate)
        {
            NatureOfEmployment = "SALARIED";
            EmploymentStatus = "Salaried";
            EmployerName = employerName;
            Occupation = occupationCode;
            EmploymentStartDate = employmentStartDate;
            CurrentEmployment = "Y";
        }
    }

    public class GuardianDetails
    {
        [JsonPropertyName("relationshipType")]
        public string? RelationshipType { get; set; }

        [JsonPropertyName("guardianCifType")]
        public string? GuardianCifType { get; set; }

        [JsonPropertyName("guardianId")]
        public string? GuardianId { get; set; }

        public GuardianDetails(string guardianId)
        {
            RelationshipType = "KIN";
            GuardianCifType = "Retail";
            GuardianId = guardianId;
        }
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

        public PhoneDetail(string phoneNumber)
        {
            if (Utils.IsValidPhoneNumber(phoneNumber, out string localNumber))
            {
                CountryCode = "+233";
                PhoneType = "HOMEPH1";
                PhoneNumber = localNumber;
                PreferredPhoneNumber = "Y";
            }
        }
    }

    public class RelationshipManagerDetail
    {
        [JsonPropertyName("relationshipManagerId")]
        public string? RelationshipManagerId { get; set; }

        [JsonPropertyName("relationshipManagerName")]
        public string? RelationshipManagerName { get; set; }

        [JsonPropertyName("primaryRelationshipManager")]
        public string? PrimaryRelationshipManager { get; set; }

        [JsonPropertyName("pcCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PcCode { get; set; }

        [JsonPropertyName("pcName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PcName { get; set; }

        [JsonPropertyName("pcEmail")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PcEmail { get; set; }

        [JsonPropertyName("pcPhNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PcPhNumber { get; set; }

        [JsonPropertyName("teamName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TeamName { get; set; }

        [JsonPropertyName("teamEmail")]
        public string? TeamEmail { get; set; }

        public RelationshipManagerDetail(string relationshipManagerId, string relationshipManagerName, string relationshipManagerEmail, string relationshipManagerPcCode)
        {
            RelationshipManagerId = relationshipManagerId;
            RelationshipManagerName = relationshipManagerName;
            PrimaryRelationshipManager = "Y";
            TeamEmail = relationshipManagerEmail; // Assuming team email is the same as RM email
            PcCode = relationshipManagerPcCode;
        }
    }

    public class RiskRatingDetail
    {
        [JsonPropertyName("riskRating")]
        public string? RiskRating { get; set; }

        [JsonPropertyName("customerRating")]
        public string? CustomerRating { get; set; }

        [JsonPropertyName("riskRatingDate")]
        public string? RiskRatingDate { get; set; }

        public RiskRatingDetail(string riskRating, string riskRatingDate)
        {
            RiskRating = riskRating; // Low, Medium, High, etc.
            CustomerRating = riskRating.ToUpper() + " RISK";
            RiskRatingDate = riskRatingDate;
        }
    }
}
