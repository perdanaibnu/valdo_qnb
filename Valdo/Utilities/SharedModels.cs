using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valdo
{
    public class DDLField
    {
        public string valuefield { get; set; }
        public string textfield { get; set; }

        public DDLField() { }

        public DDLField(string _value, string _text)
        {
            valuefield = _value;
            textfield = _text;
        }
    }

    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public SearchBuilder searchBuilder { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class SearchBuilder
    {
        public List<Criteria> criteria { get; set; }
        public string logic { get; set; }
    }

    public class Criteria
    {
        public string condition { get; set; }
        public string data { get; set; }
        public string origData { get; set; }
        public string type { get; set; }
        public List<string> value { get; set; }
        public string value1 { get; set; }
    }

    public class DTCol
    {
        public string name { get; set; }
        public string title { get; set; }
        public string allign { get; set; } = "left";
        public string link { get; set; }
        public string format { get; set; }
        public bool check { get; set; } = false;
        public bool search { get; set; } = true;
        public bool order { get; set; } = true;
        public bool visible { get; set; } = true;
        public string width { get; set; }
        public string length { get; set; }
        public string placeholder { get; set; }
        public string add_cls { get; set; }
    }
}