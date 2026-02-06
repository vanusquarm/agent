namespace Transactions.API.Entities
{
    public class FinacleConfig
    {
    }

    public class Header
    {
        public string DeliveryChannel { get; set; } = "API";
        public string Duptranchkreqd { get; set; } = "Y";
        public string Requesttype { get; set; } = "SRV";
    }
}
