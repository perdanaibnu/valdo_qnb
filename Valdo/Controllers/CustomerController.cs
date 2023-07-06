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
    public class CustomerController : Controller
    {
        private dbEntities db;

        public CustomerController()
        {
            db = new dbEntities();
            db.Database.CommandTimeout = 0;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            ViewBag.Title = "Customer";
            return View();
        }

        public JsonResult getColumnConfig()
        {
            var result = new m_customer().getDTCols();
            return new JsonResult()
            {
                Data = new { result, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue,
            };
        }

        public JsonResult getDataList(DataTableAjaxPostModel model, string id) => GetJsonResultSS(model, $"select t.* from m_customer t {(!string.IsNullOrEmpty(id) ? $"where t.customer_id = {id}" : "")}".concatIn(), order: $"customer_id");

        public ActionResult Form(string id = null)
        {
            m_customer tbl;
            string action = "New Data";
            if (string.IsNullOrEmpty(id)) tbl = new m_customer();
            else
            {
                action = "Update Data";
                tbl = db.m_customer.FirstOrDefault(a => a.customer_id == id);
            }
            ViewBag.action = action; var is_new_data = (action == "New Data");
            ViewBag.Title = "Customer";
            return View(tbl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(m_customer tbl, string action)
        {
            var msg = ""; var result = "failed"; var hdrid = "";
            var servertime = getServerDateTime();

            msg += validate(ref tbl, (action.ToLower() == "new data"));

            if (string.IsNullOrEmpty(msg))
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (action == "New Data")
                        {
                            db.m_customer.Add(tbl); db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(tbl).State = EntityState.Modified; db.SaveChanges();
                        }
                        objTrans.Commit(); msg = "Data has ben saved <br />"; hdrid = tbl.customer_id.ToString();
                        result = "success";
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException e)
                    {
                        objTrans.Rollback();
                        var err = "";
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            err += "Entity of type " + eve.Entry.Entity.GetType().Name + " in state " + eve.Entry.State + " has the following validation errors:<br />";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                err += "- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage + "<br />";
                            }
                        }
                        msg = err;
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        msg = ex.ToString();
                    }
                }
            }
            return Json(new { result, msg, hdrid }, JsonRequestBehavior.AllowGet);
        }

        private string validate(ref m_customer val, bool is_new)
        {
            var msg = string.Empty;
            if (string.IsNullOrEmpty(val.customer_id)) msg += "Please fill Customer ID !<br/>";
            if (string.IsNullOrEmpty(val.customer_name)) val.customer_name = "";
            if (string.IsNullOrEmpty(val.customer_address)) val.customer_address = "";

            var customer_id = val.customer_id;
            if (string.IsNullOrEmpty(msg))
            {
                if (db.m_customer.Where(x => x.customer_id == customer_id).Any() && is_new) msg += $"ID Customer {customer_id} has been used by another data. Please input another!<br />";
            }
            return msg;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            m_customer tbl = db.m_customer.FirstOrDefault(a => a.customer_id == id);
            string result = "success";
            string msg = "";
            if (tbl == null) result = "failed";
            msg = "Data can't be found!";
            if (result == "success")
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.m_customer.Remove(tbl);
                        db.SaveChanges();
                        objTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        result = "failed";
                        msg = ex.ToString();
                    }
                }
            return Json(new { result, msg }, JsonRequestBehavior.AllowGet);
        }
    }
}