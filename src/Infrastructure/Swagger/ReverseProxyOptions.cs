namespace Finacle.Infrastructure.Swagger
{
    public class ReverseProxyOptions
    {
        public Dictionary<string, Cluster> Clusters { get; set; }
    }

    public class Cluster
    {
        public Dictionary<string, Destination> Destinations { get; set; }
    }

    public class Destination
    {
        public string Address { get; set; }
        public List<Swagger> Swaggers { get; set; }
    }

    public class Swagger
    {
        public string PrefixPath { get; set; }
        public List<string> Paths { get; set; }
    }
}