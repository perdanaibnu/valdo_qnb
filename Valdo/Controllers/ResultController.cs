using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using Valdo.Models;
using static Valdo.SharedFunctions;
using static Valdo.SharedExtensions;

namespace Valdo.Controllers
{
    public class ResultController : Controller
    {
        private dbEntities db;

        public ResultController()
        {
            db = new dbEntities();
            db.Database.CommandTimeout = 0;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            ViewBag.Title = "Result";
            return View();
        }

        public JsonResult getColumnConfig()
        {
            var result = new r_result().getDTCols();
            return new JsonResult()
            {
                Data = new { result, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue,
            };
        }

        public JsonResult getDataList(DataTableAjaxPostModel model, string id) => GetJsonResultSS(model, $"select t.customer_id, t.customer_name, t.customer_address, isnull((select sum(balance) from m_lending where customer_id = t.customer_id), 0.0) lending_balance, isnull((select sum(balance) from m_funding where customer_id = t.customer_id), 0.0) funding_balance, isnull(stuff((select distinct ', ' + agunan_id from m_agunan where customer_id = t.customer_id for xml path('')), 1, 1, ''), '') agunan_id from m_customer t {(!string.IsNullOrEmpty(id) ? $"where t.customer_id = {id}" : "")}".concatIn(), order: $"customer_id");
    }
}