namespace Transactions.API.Entities
{
    public class CurrencyConversionReqVM
    {
        /// <summary>
        /// Currency code to convert from
        /// </summary>
        //[StringLength(3)]

        public string fromCurrencyCode { get; set; }

        /// <summary>
        ///  Currency rate code
        /// </summary>

        public string rateCode { get; set; }

        /// <summary>
        ///  Currency code to convert to
        /// </summary>
       // [StringLength(3)]

        public string toCurrencyCode { get; set; }

        /// <summary>
        ///  Amount
        /// </summary>

        public decimal transactionAmount { get; set; }
    }
}