using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public IntraBankTransferResponse TransferFunds(IntraBankTransferRequest transferRequest)
        {
            if (transferRequest == null)
                throw new ArgumentNullException(nameof(transferRequest));

            if (transferRequest.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transferRequest.Amount));

            if (string.IsNullOrWhiteSpace(transferRequest.SourceAccountId) ||
                string.IsNullOrWhiteSpace(transferRequest.DestinationAccountId))
            {
                throw new ArgumentException("Invalid SourceAccount || DestinationAccount", nameof(transferRequest));
            }

            var requestUri = "api/v1/Transactions/transfer";
            var jsonContent = new StringContent(JsonSerializer.Serialize(transferRequest), Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(requestUri, jsonContent).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<IntraBankTransferResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public IntraBankTransferResponse TransferFunds(string sourceAccount, string destAccount, double amount, string channelReference, string remarks, string channelId, string chargeEventId)
        {
            var transferRequest = new IntraBankTransferRequest
            {
                SourceAccountId = sourceAccount,
                DestinationAccountId = destAccount,
                Amount = (decimal)amount,
                Remarks = remarks,
                ChannelReference = channelReference,
                ChannelId = channelId,
                DocNum = null,
                ChargeEventId = chargeEventId
            };

            return TransferFunds(transferRequest);
        }
    }
}