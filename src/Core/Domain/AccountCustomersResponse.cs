using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Finacle.Core.Domain
{
    public class AccountCustomersResponse
    {
        [JsonPropertyName("accountDetails")]
        public List<AccountDetail> AccountDetails { get; set; }

        [JsonPropertyName("customerDetails")]
        public List<CustomerDetail> CustomerDetails { get; set; }
    }

    public class AccountDetail
    {
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("accountStatus")]
        public string AccountStatus { get; set; }

        [JsonPropertyName("accountOpeningDate")]
        public string AccountOpeningDate { get; set; }

        [JsonPropertyName("accountFreezeStatus")]
        public string AccountFreezeStatus { get; set; }

        [JsonPropertyName("lienAmount")]
        public string LienAmount { get; set; }

        [JsonPropertyName("clearBalanceAmount")]
        public string ClearBalanceAmount { get; set; }

        [JsonPropertyName("acctSolId")]
        public string AcctSolId { get; set; }

        [JsonPropertyName("schmtype")]
        public string Schmtype { get; set; }

        [JsonPropertyName("accountCurrency")]
        public string AccountCurrency { get; set; }

        [JsonPropertyName("schmcode")]
        public string Schmcode { get; set; }

        [JsonPropertyName("BranchName")]
        public string BranchName { get; set; }

        [JsonPropertyName("cash_dr_lim")]
        public string CashDrLim { get; set; }

        [JsonPropertyName("transfer_dr_lim")]
        public string TransferDrLim { get; set; }

        [JsonPropertyName("cash_cr_lim")]
        public string CashCrLim { get; set; }

        [JsonPropertyName("transfer_cr_lim")]
        public string TransferCrLim { get; set; }

        [JsonPropertyName("Dr_transactioncount")]
        public string DrTransactioncount { get; set; }

        [JsonPropertyName("Dr_transactionsum")]
        public string DrTransactionsum { get; set; }

        [JsonPropertyName("acctOwnerShip")]
        public string AcctOwnerShip { get; set; }

        [JsonPropertyName("availableBalanceAmount")]
        public string AvailableBalanceAmount { get; set; }

        [JsonPropertyName("acctstate")]
        public string Acctstate { get; set; }
    }

    public class CustomerDetail
    {
        [JsonPropertyName("customerReference")]
        public string CustomerReference { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phonenumber")]
        public string Phonenumber { get; set; }

        [JsonPropertyName("DateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("CustSubSectCode")]
        public string CustSubSectCode { get; set; }

        [JsonPropertyName("CustShortName")]
        public string CustShortName { get; set; }

        [JsonPropertyName("CustomerTier")]
        public string CustomerTier { get; set; }

        [JsonPropertyName("BVN")]
        public string BVN { get; set; }

        [JsonPropertyName("Rating")]
        public string Rating { get; set; }

        [JsonPropertyName("RELATIONSHIPMANAGERID")]
        public string RELATIONSHIPMANAGERID { get; set; }

        [JsonPropertyName("RELATIONSHIPMANAGERNAME")]
        public string RELATIONSHIPMANAGERNAME { get; set; }

        [JsonPropertyName("RMGRDEPARTMENT")]
        public string RMGRDEPARTMENT { get; set; }

        [JsonPropertyName("AddressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("AddressLine2")]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("cityCode")]
        public string CityCode { get; set; }

        [JsonPropertyName("ocuupationCode")]
        public string OcuupationCode { get; set; }

        [JsonPropertyName("salutation")]
        public string Salutation { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("middlename")]
        public string Middlename { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("empstatus")]
        public string Empstatus { get; set; }
    }
}
