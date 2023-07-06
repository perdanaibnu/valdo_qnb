using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Valdo.Models
{
    internal sealed class ResultMetaData { }

    [MetadataType(typeof(ResultMetaData))]
    public partial class r_result { }

    public static class ResultExtensions
    {
        public static List<DTCol> getDTCols(this r_result tbl)
        {
            var result = new List<DTCol>
            {
                new DTCol {name = "customer_id", title = "ID Customer" },
                new DTCol {name = "customer_name", title = "Name" },
                new DTCol {name = "customer_address", title = "Address" },
                new DTCol {name = "lending_balance", title = "Lending Balance", format = "num" },
                new DTCol {name = "funding_balance", title = "Funding Balance", format = "num" },
                new DTCol {name = "agunan_id", title = "ID Agunan" },
            };
            return result;
        }
    }
}