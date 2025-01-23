using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    internal class CreateSavingsAcctRequest
    {
        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("glSubHeadCode")]
        public string GlSubHeadCode { get; set; }

        [JsonPropertyName("acctReservationID")]
        public string AcctReservationID { get; set; }

        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("accountNickName")]
        public string AccountNickName { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("branchReference")]
        public string BranchReference { get; set; }

        [JsonPropertyName("modeOfOperation")]
        public string ModeOfOperation { get; set; }

        [JsonPropertyName("relatedPartyDetails")]
        public List<RelatedPartyDetail> RelatedPartyDetails { get; set; }

        [JsonPropertyName("interestCreditAccountIndicator")]
        public string InterestCreditAccountIndicator { get; set; }

        [JsonPropertyName("interestCreditAccountIdentification")]
        public string InterestCreditAccountIdentification { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string AccountOpeningDate { get; set; }

        [JsonPropertyName("sourcingDealerIdentification")]
        public string SourcingDealerIdentification { get; set; }
    }
    public class RelatedPartyDetail
    {
        [JsonPropertyName("relatedPartyCustomerReference")]
        public string RelatedPartyCustomerReference { get; set; }

        [JsonPropertyName("relationType")]
        public string RelationType { get; set; }

        [JsonPropertyName("relationCode")]
        public string RelationCode { get; set; }

        [JsonPropertyName("designationCode")]
        public string DesignationCode { get; set; }
    }

}
