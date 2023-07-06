using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Valdo.Models;
using System.Data;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.AccessControl;
using System.Net.Mail;
using System.Drawing;
using System.Web.Hosting;

namespace Valdo
{
    public static class SharedFunctions
    {
        public static DateTime getServerDateTime()
        {
            return new dbEntities().Database.SqlQuery<DateTime>($"select getdate() dt").FirstOrDefault();
        }

        public static DateTime getServerDate()
        {
            return (new dbEntities().Database.SqlQuery<string>($"select format(getdate(), 'MM/dd/yyyy') dt").FirstOrDefault()).ToDateTime();
        }

        public static List<m_customer> getCustomer() => new dbEntities().m_customer.ToList();

        public static List<Dictionary<string, object>> toObject(DataTable tbl)
        {
            List<Dictionary<string, object>> rowsdtl = new List<Dictionary<string, object>>();
            if (tbl.Rows.Count > 0)
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    var row = new Dictionary<string, object>();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        row.Add(col.ColumnName, (dr[col] == DBNull.Value) ? "" : dr[col]);
                    }
                    rowsdtl.Add(row);
                }
            }
            return rowsdtl;
        }

        public static IList<Dictionary<string, object>> getListDataTable(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount, string select_query, string fixed_filter = "", string default_order = "", bool no_paging = false)
        {
            var take = model.length;
            var skip = model.start;

            var and_filter = "";
            if ((model.search != null) ? !string.IsNullOrEmpty(model.search.value) : false)//search in all column
            {
                var col_tosearch = model.columns.Where(w => w.searchable & w.data != "sAddFilter").Select(s => s.data).ToList();
                foreach (var s in model.search.value.Trim().Split(' '))
                {
                    var or_filter = "";
                    foreach (var a in col_tosearch)
                    {
                        if (or_filter != "") or_filter += " OR ";
                        or_filter += a + " LIKE '%" + s.toChar() + "%'";
                    }
                    if (or_filter != "")
                    {
                        and_filter += " AND (" + or_filter + ")";
                    }
                }
            }
            foreach (var obj in model.columns.Where(w => w.searchable & w.search != null).ToList())//search by column
            {
                if (!string.IsNullOrEmpty(obj.search.value))
                {
                    if (obj.search.value.ToLower().StartsWith("between") || obj.search.value.ToLower().StartsWith("in(") || obj.search.value.StartsWith("=") || obj.search.value.StartsWith("<>"))
                    {
                        if (obj.search.value.ToLower().StartsWith("between"))
                        {
                            var split = obj.search.value.Split('\'');
                            if (split != null && split.Length > 4)
                            {
                                var s2 = split[1].Split('/');
                                var s3 = split[3].Split('/');
                                if (s2 != null && s2.Length > 2 && s3 != null && s3.Length > 2) obj.search.value = $"{split[0]} '{s2[1]}/{s2[0]}/{s2[2]}' {split[2]} '{s3[1]}/{s3[0]}/{s3[2]}'";
                            }
                        }
                        and_filter += " AND " + obj.data + " " + obj.search.value;
                    }
                    else if (obj.data == "sAddFilter")
                    {
                        and_filter += obj.search.value;
                    }
                    else
                    {
                        var n_filter = "";
                        foreach (var s in obj.search.value.Split(' '))
                        {
                            if (n_filter != "") n_filter += " OR ";
                            n_filter += obj.data + " LIKE '" + obj.search.value + "'";
                        }
                        if (n_filter != "")
                        {
                            and_filter += " AND (" + n_filter + ")";
                        }
                    }
                }
            }
            if (model.searchBuilder != null)
            {
                if (model.searchBuilder.criteria != null && model.searchBuilder.criteria.Count > 0)
                {
                    string n_filter = string.Empty;
                    foreach (var item in model.searchBuilder.criteria)
                    {
                        if (!string.IsNullOrEmpty(n_filter)) n_filter += $" {model.searchBuilder.logic} ";
                        n_filter += FormatSearch(item.condition, item.origData, item.value, item.type);
                    }
                    if (!string.IsNullOrEmpty(n_filter)) and_filter += " AND (" + n_filter + ")";
                }
            }

            string orderBy = "";
            if (model.order != null)
            {
                foreach (var o in model.order)
                {
                    if (!string.IsNullOrEmpty(orderBy)) orderBy += ", ";
                    orderBy += model.columns[o.column].data + " " + o.dir.ToLower();
                }
            }

            // search the dbase taking into consideration table sorting and paging
            //var result = GetDataFromDbase(and_filter, take, skip, orderBy, out filteredResultsCount, out totalResultsCount);
            if (String.IsNullOrEmpty(orderBy))
            {
                if (string.IsNullOrEmpty(default_order))
                    default_order = model.columns.Select(s => s.data).First() + " DESC";
                orderBy = default_order;
            }

            var result_query = select_query + " WHERE 1=1 " + and_filter + fixed_filter + " ORDER BY " + orderBy + "";
            if (!no_paging) result_query += $" OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY";
            var result = toObject(new SharedConnections().GetDataTable(result_query, "result")); // SQL server 2016

            var rs_count = new SharedConnections().GetDataTable(select_query + " WHERE 1=1 " + and_filter + fixed_filter + " ", "result2");
            var rs_count2 = new SharedConnections().GetDataTable(select_query + " WHERE 1=1 " + fixed_filter + " ", "result3");
            // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
            filteredResultsCount = rs_count.Rows.Count;
            totalResultsCount = rs_count2.Rows.Count;
            if (result == null)
            {
                // empty collection...
                return new List<Dictionary<string, object>>();
            }
            return result;
        }

        private static string FormatSearch(string cond, string field, List<string> val, string type)
        {
            string result = string.Empty;
            string value = val == null ? "" : val[0];
            if (type == "string")
            {
                switch (cond)
                {
                    case "=": result = $"{field} = '{value}'"; break;
                    case "!=": result = $"{field} <> '{value}'"; break;
                    case "starts": result = $"{field} LIKE '{value}%'"; break;
                    case "!starts": result = $"{field} NOT LIKE '{value}%'"; break;
                    case "contains": result = $"{field} LIKE '%{value}%'"; break;
                    case "!contains": result = $"{field} NOT LIKE '%{value}%'"; break;
                    case "ends": result = $"{field} LIKE '%{value}'"; break;
                    case "!ends": result = $"{field} NOT LIKE '%{value}'"; break;
                    case "null": result = $"ISNULL({field}, '') = ''"; break;
                    case "!null": result = $"ISNULL({field}, '') <> ''"; break;
                    default: break;
                }
            }
            else if (type == "num")
            {
                switch (cond)
                {
                    case "between": result = $"{field} BETWEEN {value} AND {val[1]}"; break;
                    case "!between": result = $"{field} NOT BETWEEN {value} AND {val[1]}"; break;
                    case "null": result = $"{field} IS NULL"; break;
                    case "!null": result = $"{field} IS NOT NULL"; break;
                    default: result = $"{field} {cond} {value}"; break;
                }
            }
            else if (type == "date")
            {
                string fld = $"FORMAT({field}, 'yyyy-MM-dd')";
                switch (cond)
                {
                    case "between": result = $"{fld} BETWEEN '{value}' AND '{val[1]}'"; break;
                    case "!between": result = $"{fld} NOT BETWEEN '{value}' AND '{val[1]}'"; break;
                    case "null": result = $"ISNULL({fld}, '') = ''"; break;
                    case "!null": result = $"ISNULL({fld}, '') != ''"; break;
                    default: result = $"{fld} {cond} '{value}'"; break;
                }
            }
            return result;
        }

        public static JsonResult GetJsonResultSS(DataTableAjaxPostModel model, string query, string filter = "", string order = "", string seq = "", Dictionary<string, string> def = null)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>(); int recordsTotal = 0; int recordsFiltered = 0; string message = string.Empty;
            try
            {
                data = getListDataTable(model, out recordsFiltered, out recordsTotal, query, filter, order).ToList();
                if (data == null || data.Count <= 0) message = "Data not found!";
                else
                {
                    if (!string.IsNullOrEmpty(seq) || (def != null && def.Count > 0))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(seq)) data[i][seq] = i + 1;
                            if (def != null && def.Count > 0) foreach (var item in def) data[i][item.Key] = data[i][item.Value];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
            }
            return new JsonResult()
            {
                Data = new
                {
                    draw = model.draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data,
                    query = query
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue,
            };
        }
    }
}