using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Valdo.Models
{
    internal sealed class CustomerMetaData { }

    [MetadataType(typeof(CustomerMetaData))]
    public partial class m_customer { }

    public static class CustomerExtensions
    {
        public static List<DTCol> getDTCols(this m_customer tbl)
        {
            var result = new List<DTCol>
            {
                new DTCol {name = "customer_id", title = "ID Customer", link = "customer_id" },
                new DTCol {name = "customer_name", title = "Name" },
                new DTCol {name = "customer_address", title = "Address" }
            };
            return result;
        }
    }
}