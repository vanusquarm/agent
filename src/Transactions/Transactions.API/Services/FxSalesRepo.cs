using MassTransit.Middleware;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Transactions.API.Entities;

namespace Transactions.API.Services
{
    public class FxSalesRepo
    {
        private readonly DbContext _dapperDAO;

        public FxSalesRepo(DbContext dapperDAO)
        {
            _dapperDAO = dapperDAO;
        }
        //public async Task<decimal> GetFxSalesLimit(string Cif_Id, string Amount)
        //{
        //    //var dapperDAO = new DapperDAO<decimal>();
        //    //var dbConnection = DatabaseHelper.GetSQLConnection(ConfigSettings.ConnectionString.EoneConnectionString);
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("@Cif_Id", Cif_Id);
        //    parameters.Add("@Amount", Amount);
        //    decimal limitResp = await _dapperDAO.GetObjectWithDictionaryParam<decimal>(ConfigSettings.ConnectionString.EoneConnectionString_Select, parameters, "Check_Daily_limit_FXSaleV2_CIF", CommandType.StoredProcedure);
        //    return limitResp;
        //}

        //public async Task<FxSalesResponse> GetDailyFxSaleRates()
        //{
        //    var response = new FxSalesResponse();
        //    using (IDbConnection _con = new SqlConnection(ConfigSettings.ConnectionString.EoneConnectionString_Select))
        //    {
        //        _con.Open();
        //        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //        parameters.Add("@Current", "1");
        //        IEnumerable<FxRateDbResponse> dailyFxRate = await _con.QueryAsync<FxRateDbResponse>("proc_GetFxSaleRate", parameters, commandType: CommandType.StoredProcedure);
        //        if (dailyFxRate == null)
        //        {
        //            response.ResponseCode = ResponseManager.NoRecordReturned.Item1;
        //            response.ResponseDescription = ResponseManager.NoRecordReturned.Item2 + "This feature will be made available shortly";
        //            return response;
        //        }
        //        response.LstRate = new List<FxSaleRate>();
        //        foreach (var item in dailyFxRate)
        //        {
        //            var fxSaleRate = new FxSaleRate();
        //            fxSaleRate.CurrencyCode = item.Currency_Code == null ? "0" : Convert.ToString(item.Currency_Code);
        //            fxSaleRate.CurrencyName = item.Currency_Code == null ? "" : Convert.ToString(item.Currency_Code);
        //            fxSaleRate.CurrencySymbol = item.Currency_Symbol == null ? "" : Convert.ToString(item.Currency_Symbol);
        //            fxSaleRate.BidSpread = item.Bid_Spread == null ? 0 : Convert.ToDecimal(item.Bid_Spread);
        //            fxSaleRate.OfferSpread = item.Offer_Spread == null ? 0 : Convert.ToDecimal(item.Offer_Spread);
        //            fxSaleRate.InputRate = item.Input_Rate == null ? 0 : Convert.ToDecimal(item.Input_Rate);
        //            fxSaleRate.Bid = item.Bid == null ? 0 : Convert.ToDecimal(item.Bid);
        //            fxSaleRate.Offer = item.Offer == null ? 0 : Convert.ToDecimal(item.Offer);
        //            fxSaleRate.ApprovedBy = item.Approved_By == null ? string.Empty : Convert.ToString(item.Approved_By);
        //            fxSaleRate.Limit = item.Limit.Contains("Limit") ? item.Limit == null ? 0 : Convert.ToDecimal(item.Limit) : 0;
        //            response.LstRate.Add(fxSaleRate);
        //        }
        //        response.ResponseCode = ResponseManager.Success.Item1;
        //        response.ResponseDescription = ResponseManager.Success.Item2;
        //        return response;
        //    }
        //}

        //public async Task<int> InsertFxSales(FxBehaviorRequest req)
        //{
        //    //var dapperDAO = new DapperDAO<int>();
        //    //var dbConnection = DatabaseHelper.GetSQLConnection(ConfigSettings.ConnectionString.EoneConnectionString);
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("@Cif_Id", req.customerReference);
        //    parameters.Add("@Source_Acct_No", req.DebitAccount);
        //    parameters.Add("@Destination_Acct_No", req.CreditAccount);
        //    parameters.Add("@TrAmount", req.Amount);
        //    parameters.Add("@Medium", req.Channel);
        //    parameters.Add("@Remarks", req.Remarks);
        //    parameters.Add("@CurrencyCode", req.DebitCurrencyCode);
        //    parameters.Add("@CustomerName", req.Name);
        //    parameters.Add("@CustomerAddress", "");
        //    parameters.Add("@Status", "INITIATED");
        //    parameters.Add("@DollarRate", req.DollarRate);
        //    parameters.Add("@RequestId", req.RequestId);
        //    parameters.Add("@TellerId", req.TellerId);
        //    parameters.Add("@SupervisorId", req.SupervisorId);
        //    parameters.Add("@TransCurrencyCode", req.DebitCurrencyCode);
        //    parameters.Add("@TransCurrencyToCode", req.CreditCurrencyCode);
        //    parameters.Add("@TransCurrencyTo", req.CreditCurrencyCode);
        //    parameters.Add("@RateApprovedby", req.ApprovedBy);

        //    int dailyFxRate = await _dapperDAO.SetObject<int>(ConfigSettings.ConnectionString.EoneConnectionString_Update, parameters, "Log_FXSaleV2_CIF", CommandType.StoredProcedure);
        //    return dailyFxRate;
        //}

        //public async Task<int> PatchFxSales(CreateTransferResponse req, string RequestId, string ResponseCode)
        //{
        //    //var dapperDAO = new DapperDAO<int>();
        //    //var dbConnection = DatabaseHelper.GetSQLConnection(ConfigSettings.ConnectionString.EoneConnectionString);
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("@RequestId", RequestId);
        //    parameters.Add("@BasisResp", ResponseCode);
        //    parameters.Add("@TraSeq1", req.TransactionReference);
        //    parameters.Add("@Status", ResponseCode == ResponseManager.Success.Item1 ? "POSTED" : "FAILED");
        //    _dapperDAO.TranslogDTOs
        //        .FromSqlRaw("EXEC dbo.procGetTranslogByAgent @Agent", new SqlParameter("@Agent", agent))
        //        .AsNoTracking()
        //        .ToList();
        //    int dailyFxRate = await _dapperDAO.SetObject<int>(ConfigSettings.ConnectionString.EoneConnectionString_Update, parameters, "proc_UpdateFXSale_LogV2_CIF", CommandType.StoredProcedure);
        //    return dailyFxRate;
        //}
    }

}