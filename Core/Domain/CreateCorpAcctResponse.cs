using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class CreateCorpAcctResponse
    {
        [JsonPropertyName("accountIdentification")]
        public string AccountIdentification { get; set; }

        [JsonPropertyName("iBAN")]
        public string IBAN { get; set; }
    }
}
