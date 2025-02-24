using System;
using System.Collections.Generic;
using System.Text;

namespace Finacle.Core.Domain
{
    public class AccountStatementsRequest
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? SetNumber { get; set; }
        public int? MaximumRecords { get; set; }
    }

}
