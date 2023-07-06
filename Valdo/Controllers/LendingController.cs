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
    public class LendingController : Controller
    {
        private dbEntities db;

        public LendingController()
        {
            db = new dbEntities();
            db.Database.CommandTimeout = 0;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            ViewBag.Title = "Lending";
            return View();
        }

        public JsonResult getColumnConfig()
        {
            var result = new m_lending().getDTCols();
            return new JsonResult()
            {
                Data = new { result, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue,
            };
        }

        public JsonResult getDataList(DataTableAjaxPostModel model, string id) => GetJsonResultSS(model, $"select t.* from m_lending t {(!string.IsNullOrEmpty(id) ? $"where t.lending_id = {id}" : "")}".concatIn(), order: $"lending_id");

        public ActionResult Form(string id = null)
        {
            m_lending tbl;
            string action = "New Data";
            if (string.IsNullOrEmpty(id)) tbl = new m_lending();
            else
            {
                action = "Update Data";
                tbl = db.m_lending.FirstOrDefault(a => a.lending_id == id);
            }
            ViewBag.action = action; var is_new_data = (action == "New Data");
            ViewBag.Title = "Lending";
            ViewBag.customer_id = getCustomer().toDDLField().ToSL(tbl.customer_id);
            return View(tbl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(m_lending tbl, string action)
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
                            db.m_lending.Add(tbl); db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(tbl).State = EntityState.Modified; db.SaveChanges();
                        }
                        objTrans.Commit(); msg = "Data has ben saved <br />"; hdrid = tbl.lending_id.ToString();
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

        private string validate(ref m_lending val, bool is_new)
        {
            var msg = string.Empty;
            if (string.IsNullOrEmpty(val.lending_id)) msg += "Please fill Lending ID !<br/>";
            if (string.IsNullOrEmpty(val.account_no)) msg += "Please fill Account No !<br/>";
            if (string.IsNullOrEmpty(val.customer_id)) msg += "Please fill Customer ID !<br/>";
            if (val.balance < 0) msg += "Balance must be more than 0 !<br/>";
            if (val.plafond < 0) msg += "Plafond must be more than 0 !<br/>";

            var lending_id = val.lending_id;
            if (string.IsNullOrEmpty(msg))
            {
                if (db.m_lending.Where(x => x.lending_id == lending_id).Any() && is_new) msg += $"ID Lending {lending_id} has been used by another data. Please input another!<br />";
            }
            return msg;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            m_lending tbl = db.m_lending.FirstOrDefault(a => a.lending_id == id);
            string result = "success";
            string msg = "";
            if (tbl == null) result = "failed";
            msg = "Data can't be found!";
            if (result == "success")
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.m_lending.Remove(tbl);
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