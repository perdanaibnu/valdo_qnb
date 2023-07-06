using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Valdo.Models
{
    internal sealed class LendingMetaData { }

    [MetadataType(typeof(LendingMetaData))]
    public partial class m_lending { }

    public static class LendingExtensions
    {
        public static List<DTCol> getDTCols(this m_lending tbl)
        {
            var result = new List<DTCol>
            {
                new DTCol {name = "lending_id", title = "ID Lending", link = "lending_id" },
                new DTCol {name = "account_no", title = "Account No" },
                new DTCol {name = "customer_id", title = "ID Customer" },
                new DTCol {name = "balance", title = "Balance", format = "num" },
                new DTCol {name = "plafond", title = "Plafond", format = "num" }
            };
            return result;
        }
    }
}