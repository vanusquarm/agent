using System;
using System.Collections.Generic;

namespace Finacle.Core.Domain
{

    public class IntraBankTransferRequest
    {
        public string TransactionType { get; set; }
        public string TransactionSubType { get; set; }
        public string UserTransactionCode { get; set; }
        public string ChannelIdentification { get; set; }
        public string EntererId { get; set; }
        public List<IntraBankTransferTransactionDetail> TransferTransactionDetail { get; set; }
    }

    public class IntraBankTransferTransactionDetail
    {
        public string SerialNumber { get; set; }
        public string PartTransactionType { get; set; }
        public string AccountIdentification { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionCurrency { get; set; }
        public DateTime TransactionValueDate { get; set; }
        public string TransactionParticulars { get; set; }
        public decimal Rate { get; set; }
        public string RateCode { get; set; }
        public string PartTransactionRemarks { get; set; }
        public string TreasuryReferenceNumber { get; set; }
        public decimal TreasuryRate { get; set; }
        public string UserPartTransactionCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string TransactionParticularCode { get; set; }
        public IntraBankMiscellaneousDetailsType MiscellaneousDetailsType { get; set; }
        public string PartTransactionRemarks2 { get; set; }
        public string PartitionedEntityType { get; set; }
        public string PartitionedEntityID { get; set; }
        public string TodRequired { get; set; }
        public TodDetails TodDetails { get; set; }
        public AdditionalDetails AdditionalDetails { get; set; }
        public IntraBankInstrumentDetails InstrumentDetails { get; set; }
        public string ChargeEventId { get; set; }
        public List<ChargeDetails> ChargeDetails { get; set; }
        public List<LienDetails> LienDetailsList { get; set; }
    }

    public class ChargeDetails
    {
        public string ChargeSerialNumber { get; set; }
        public decimal ChargeAmount { get; set; }
    }


    public class IntraBankMiscellaneousDetailsType
    {
        public string DocumentNumber { get; set; }
        public string FreeTxt2 { get; set; }
        public string FreeTxt3 { get; set; }
        public string FreeTxt4 { get; set; }
    }

    public class TodDetails
    {
        public DateTime TodExpiryDate { get; set; }
        public string TodPermittedBy { get; set; }
        public string TodInterestLevel { get; set; }
        public string TodRemarks { get; set; }
    }

    public class AdditionalDetails
    {
        public string TdFlowCode { get; set; }
        public DemandDraftPaymentDetails DemandDraftPaymentDetails { get; set; }
        public string DemandDraftIssueAccountIdentification { get; set; }
    }

    public class DemandDraftPaymentDetails
    {
        public DateTime DemandDraftDate { get; set; }
        public string DemandDraftAccountIdentification { get; set; }
        public string DemandDraftPayeeName { get; set; }
        public string DemandDraftPayeeAccount { get; set; }
        public string BranchCode { get; set; }
        public string BankCode { get; set; }
        public string CreateCreditPartTransaction { get; set; }
    }

    public class LienDetails
    {
        public string ModuleType { get; set; }
        public string LienIdentification { get; set; }
        public decimal LienReleaseAmount { get; set; }
    }

    public class IntraBankInstrumentDetails
    {
        public DateTime InstrumentDate { get; set; }
        public string InstrumentAlpha { get; set; }
        public string InstrumentNumber { get; set; }
        public string InstrumentType { get; set; }

    }
}