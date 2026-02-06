using System;
using System.Collections.Generic;
using System.Text;

namespace Finacle.Core.Domain
{
    public class Header
    {
        public string DeliveryChannel { get; set; } = "API";
        public string Duptranchkreqd { get; set; } = "Y";
        public string Requesttype { get; set; } = "SRV";
    }
}
