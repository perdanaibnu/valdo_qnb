using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Valdo.Models
{
    internal sealed class AgunanMetaData { }

    [MetadataType(typeof(AgunanMetaData))]
    public partial class m_agunan { }

    public static class AgunanExtensions
    {
        public static List<DTCol> getDTCols(this m_agunan tbl)
        {
            var result = new List<DTCol>
            {
                new DTCol {name = "agunan_id", title = "ID Agunan", link = "agunan_id" },
                new DTCol {name = "account_no", title = "Account No" },
                new DTCol {name = "customer_id", title = "ID Customer" },
                new DTCol {name = "type", title = "Type" },
                new DTCol {name = "amount", title = "Amount", format = "num" }
            };
            return result;
        }
    }
}