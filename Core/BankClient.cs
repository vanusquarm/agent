using System;
using System.Collections.Generic;
using System.Text;

namespace Finacle.Core
{
    public partial class FinacleClient
    {

        List<accounts> GetDebitableAccounts(string Mobile, string customerid, string amount, bool hasibank, string accountsource, string source = null)
        {
            return new List<accounts>();
        }
        public class accounts
        {
            public string MenuNumber { get; set; }
            public string AccountNumber { get; set; }
            public double Balance { get; set; }
            public bool IsSelected { get; set; }
            public string RestrictSeq { get; set; }

        }
    }
}
