using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Finacle.Core.Domain;

namespace Finacle.Core
{
    public partial class FinacleClient
    {
        public IntraBankTransferResponse TransferFunds(IntraBankTransferRequest transferRequest, string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
                throw new ArgumentNullException(nameof(requestId));
            if (transferRequest == null)
                throw new ArgumentNullException(nameof(transferRequest));
            if (transferRequest.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transferRequest.Amount));
            if (string.IsNullOrWhiteSpace(transferRequest.SourceAccountId) || string.IsNullOrWhiteSpace(transferRequest.DestinationAccountId))
                throw new ArgumentException("Invalid SourceAccount || DestinationAccount", nameof(transferRequest));

            var requestUri = "api/v1/Transactions/transfer";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(transferRequest), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("RequestId", requestId);

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<IntraBankTransferResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public IntraBankTransferResponse TransferFunds(string sourceAccount, string destAccount, double amount, string channelReference, string remarks, string channelId, string requestId, string docNum = null, string chargeEventId = null)
        {
            var transferRequest = new IntraBankTransferRequest
            {
                SourceAccountId = sourceAccount,
                DestinationAccountId = destAccount,
                Amount = (decimal)amount,
                Remarks = remarks,
                ChannelReference = channelReference,
                ChannelId = channelId,
                DocNum = docNum,
                ChargeEventId = chargeEventId
            };

            return TransferFunds(transferRequest, requestId);
        }

        public ChargeResponse ChargeTransaction(ChargeRequest request, string requestId)
        {
            if (string.IsNullOrEmpty(requestId))
                throw new ArgumentNullException(nameof(requestId));
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v1/Transactions/charges")
            {
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("RequestId", requestId);

            var response = _httpClient.SendAsync(requestMessage).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<ChargeResponse>(responseContent);
        }

        public BankDateDto GetTransactionBankDate(string branchCode, string requestId = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/transactions/bankdate?branchCode={branchCode}");
            requestMessage.Headers.Add("RequestId", requestId ?? Guid.NewGuid().ToString());

            var response = _httpClient.SendAsync(requestMessage).Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<BankDateDto>(content);
        }
    }
}
