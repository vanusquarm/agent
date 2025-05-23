using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class TransferValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
    public bool RequiresAdditionalVerification { get; set; }
    public List<string> AvailableVerificationMethods { get; set; } = new List<string>();
}

public class IntraBankTransferRequest
{
    [JsonPropertyName("transactionType")]
    public string TransactionType { get; set; }
 
    [JsonPropertyName("transactionSubType")]
    public string TransactionSubType { get; set; }
 
    [JsonPropertyName("channelIdentification")]
    public string ChannelIdentification { get; set; }
 
    [JsonPropertyName("transferTransactionDetail")]
    public List<IntraBankTransferTransactionDetail> TransferTransactionDetail { get; set; }
 
    [JsonPropertyName("entererId")]
    public string EntererId { get; set; }

    [JsonPropertyName("userTransactionCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string UserTransactionCode { get; set; }
}
 
public class IntraBankTransferTransactionDetail
{
    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }
 
    [JsonPropertyName("partTransactionType")]
    public string PartTransactionType { get; set; }
 
    [JsonPropertyName("accountIdentification")]
    public string AccountIdentification { get; set; }
 
    [JsonPropertyName("transactionAmount")]
    public decimal TransactionAmount { get; set; }
 
    [JsonPropertyName("transactionCurrency")]
    public string TransactionCurrency { get; set; }
 
    [JsonPropertyName("transactionValueDate")]
    [JsonConverter(typeof(DateFormatConverter))]
    public DateTime TransactionValueDate { get; set; }
 
    [JsonPropertyName("transactionParticulars")]
    public string TransactionParticulars { get; set; }
 
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
 
    [JsonPropertyName("partTransactionRemarks")]
    public string PartTransactionRemarks { get; set; }
 
    [JsonPropertyName("referenceNumber")]
    public string ReferenceNumber { get; set; }
 
    [JsonPropertyName("miscellaneousDetailsType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TransferMiscellaneousDetailsType MiscellaneousDetailsType { get; set; }
 
    [JsonPropertyName("partTransactionRemarks2")]
    public string PartTransactionRemarks2 { get; set; }
 
    [JsonPropertyName("lienDetailsList")]
    public List<LienDetails> LienDetailsList { get; set; }

    [JsonPropertyName("chargeEventId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ChargeEventId { get; set; }

    [JsonPropertyName("userPartTransactionCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string UserPartTransactionCode { get; set; }
}
 
public class TransferMiscellaneousDetailsType
{
    [JsonPropertyName("documentnumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DocumentNumber { get; set; }
 
    [JsonPropertyName("freetxt2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Freetxt2 { get; set; }
 
    [JsonPropertyName("freetxt3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Freetxt3 { get; set; }
 
    [JsonPropertyName("freetxt4")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Freetxt4 { get; set; }
}
 
public class LienDetails
{
    // Add any properties for lien details here
}
    
public class DateFormatConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd", null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}
