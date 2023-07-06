using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Valdo.Models;
using System.Web.Mvc.Html;
using System.Text.RegularExpressions;

namespace Valdo
{
    public static class SharedExtensions
    {
        public static List<DDLField> toDDLField(this List<m_customer> _val)
        {
            var result = new List<DDLField>();
            if (_val != null && _val.Count > 0) foreach (var item in _val) result.Add(new DDLField(item.customer_id, string.Concat("[", item.customer_id, "] ", item.customer_name)));
            return result;
        }

        public static string toChar(this string str)
        {
            return (str ?? "").Replace("'", "''");
        }

        public static DateTime ToDateTime(this string val)
        {
            DateTime result = new DateTime(1900, 1, 1);
            if (!string.IsNullOrEmpty(val))
            {
                try { DateTime.TryParse(val, out result); }
                catch (Exception) { }
            }
            return result;
        }

        public static List<DDLField> toDDLField(this List<DDLField> _val)
        {
            var result = new List<DDLField>();
            if (_val != null && _val.Count > 0) foreach (var item in _val) result.Add(new DDLField(item.textfield, item.valuefield));
            return result;
        }

        public static SelectList ToSL(this List<DDLField> ddl, object selected = null)
        {
            return new SelectList(ddl, "valuefield", "textfield", selected);
        }

        public static MultiSelectList ToMSL(this List<DDLField> ddl, List<string> selected = null)
        {
            return new MultiSelectList(ddl, "valuefield", "textfield", selected);
        }

        public static List<string> getMenus() => new List<string>() { "Customer", "Lending", "Funding", "Agunan", "Result" }.ToList();

        public static string concatIn(this string val) => $"select * from ({val}) t";

        public static string isActive(this HtmlHelper html, string _name)
        {
            var routeData = html.ViewContext.RouteData;
            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];
            var returnActive = false;
            var menuView = new List<string>() { "Index", "Form" };
            var dtMenu = getMenus();
            if (dtMenu.Any(m => m.ToLower() == routeControl.ToLower() && menuView.Contains(routeAction)))
            {
                var menu = dtMenu.FirstOrDefault(m => m.ToLower() == routeControl.ToLower() && menuView.Contains(routeAction));
                returnActive = _name.ToLower() == menu.ToLower();
            }
            return returnActive ? "active" : "";
        }
    }
}