using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Valdo.Models
{
    internal sealed class FundingMetaData { }

    [MetadataType(typeof(FundingMetaData))]
    public partial class m_funding { }

    public static class FundingExtensions
    {
        public static List<DTCol> getDTCols(this m_funding tbl)
        {
            var result = new List<DTCol>
            {
                new DTCol {name = "funding_id", title = "ID Funding", link = "funding_id" },
                new DTCol {name = "account_no", title = "Account No" },
                new DTCol {name = "customer_id", title = "ID Customer" },
                new DTCol {name = "balance", title = "Balance", format = "num" }
            };
            return result;
        }
    }
}