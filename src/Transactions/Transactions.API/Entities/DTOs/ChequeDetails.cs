using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs
{
    public class ChequeDetails
    {
        public string InstrumentDate { get; internal set; }
        public string InstrumentAlpha { get; internal set; } 
        public string InstrumentNumber { get; internal set; }
        public string InstrumentType { get; internal set; } 
    }

    public class InstrumentDetails
    {

        [JsonPropertyName("instrumentDate")]
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime InstrumentDate { get; set; } // Date of the instrument, e.g., cheque date

        [JsonPropertyName("instrumentAlpha")]
        [StringLength(6)]
        public string InstrumentAlpha { get; set; } // Abrreivated Bank Name

        [JsonPropertyName("intrumentNumber")]
        [StringLength(16)]
        public string InstrumentNumber { get; set; } // Cheque number or instrument number

        [JsonPropertyName("intrumentType")]
        [StringLength(5)]
        public string InstrumentType { get; set; } // Type of instrument, e.g., "CHQ", "DD", etc.
    }
}