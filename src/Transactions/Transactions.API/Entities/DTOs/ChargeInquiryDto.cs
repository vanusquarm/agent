using System.Text.Json.Serialization;

namespace Transactions.API.Entities.DTOs;

public class ChargeInquiryRequest
{
    public string accountId { get; set; }
    public string tranAmt { get; set; }

    public string eventId { get; set; }
    public string eventType { get; set; }
}

public class PerformChargeInquiryDto
{
    public required string AccountId { get; set; }
    public required decimal TranAmt { get; set; }
    public required string EventId { get; set; }
    public required string EventType { get; set; }
}


public class ChargeDetails
{
    public string ChargeSerialNumber { get; set; }
    public decimal ChargeAmount { get; set; }
}

public class ChargeDetailsDto
{
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; }

    [JsonPropertyName("eventId")]
    public string EventId { get; set; }

    [JsonPropertyName("chrgAmt")]
    public string ChrgAmt { get; set; }

    [JsonPropertyName("chrgCrncy")]
    public string? ChrgCrncy { get; set; }

    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; } = string.Empty;

    [JsonPropertyName("delivery_channel")]
    public string? DeliveryChannel { get; set; } = "API";

    [JsonPropertyName("duptranchkreqd")]
    public string? Duptranchkreqd { get; set; } = "Y";

    [JsonPropertyName("requesttype")]
    public string? Requesttype { get; set; } = "SRV";

    [JsonPropertyName("requestreference")]
    public required string Requestreference { get; set; }
}

public class PerformChargeRequestDto
{
    public string TransactionReference { get; set; }
    public string AccountId { get; set; }
    public string ChargeType { get; set; }
    public decimal ChargeAmount { get; set; }
    public string Currency { get; set; }
}

public class StatementChargeRquest
{
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; }

    [JsonPropertyName("eventId")]
    public string EventId { get; set; }

    [JsonPropertyName("noOfPages")]
    public string NoOfPages { get; set; }

    [JsonPropertyName("chrgCrncy")]
    public string ChrgCrncy { get; set; }

    [JsonPropertyName("remarks")]
    public string Remarks { get; set; }
}

