using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class TellerDetails
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("bankBodDate")]
        public string BankBodDate { get; set; }

        [JsonPropertyName("workclass")]
        public string Workclass { get; set; }

        [JsonPropertyName("solid")]
        public string SolId { get; set; }

        [JsonPropertyName("userAccountExpiryDate")]
        public string UserAccountExpiryDate { get; set; }

        [JsonPropertyName("userPasswordExpiryDate")]
        public string UserPasswordExpiryDate { get; set; }

        [JsonPropertyName("userdisabledDateFrom")]
        public string UserdisabledDateFrom { get; set; }

        [JsonPropertyName("userdisabledDateTo")]
        public string UserdisabledDateTo { get; set; }

        [JsonPropertyName("RecOut")]
        public string RecOut { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("errorFlg")]
        public string ErrorFlg { get; set; }
    }

}
