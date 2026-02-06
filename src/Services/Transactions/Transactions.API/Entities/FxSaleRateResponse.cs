namespace Transactions.API.Entities
{
    public class FxSaleRateResponse
    {
        public List<CustomerDetail> CustomerDetails { get; set; }
        public char? errorFlg { get; set; }
        public string errorMsg { get; set; }
    }

    public class CustomerDetail
    {
        public string Fixedcurncy { get; set; }
        public string Varientcurncy { get; set; }
        public string Date { get; set; }
        public string Currencyunit { get; set; }
        public string Ratecode { get; set; }
    }
}
