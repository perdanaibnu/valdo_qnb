﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using Valdo.Models;
using static Valdo.SharedFunctions;
using static Valdo.SharedExtensions;

namespace Valdo.Controllers
{
    public class FundingController : Controller
    {
        private dbEntities db;

        public FundingController()
        {
            db = new dbEntities();
            db.Database.CommandTimeout = 0;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            ViewBag.Title = "Funding";
            return View();
        }

        public JsonResult getColumnConfig()
        {
            var result = new m_funding().getDTCols();
            return new JsonResult()
            {
                Data = new { result, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue,
            };
        }

        public JsonResult getDataList(DataTableAjaxPostModel model, string id) => GetJsonResultSS(model, $"select t.* from m_funding t {(!string.IsNullOrEmpty(id) ? $"where t.funding_id = {id}" : "")}".concatIn(), order: $"funding_id");

        public ActionResult Form(string id = null)
        {
            ViewBag.Title = "Funding";
            m_funding tbl;
            string action = "New Data";
            if (string.IsNullOrEmpty(id)) tbl = new m_funding();
            else
            {
                action = "Update Data";
                tbl = db.m_funding.FirstOrDefault(a => a.funding_id == id);
            }
            ViewBag.action = action; var is_new_data = (action == "New Data");
            ViewBag.customer_id = getCustomer().toDDLField().ToSL(tbl.customer_id);
            return View(tbl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(m_funding tbl, string action)
        {
            var msg = ""; var result = "failed"; var hdrid = "";
            var servertime = getServerDateTime();

            msg += validate(ref tbl, action.ToLower() == "new data");

            if (string.IsNullOrEmpty(msg))
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (action == "New Data")
                        {
                            db.m_funding.Add(tbl); db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(tbl).State = EntityState.Modified; db.SaveChanges();
                        }
                        objTrans.Commit(); msg = "Data has ben saved <br />"; hdrid = tbl.funding_id.ToString();
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

        private string validate(ref m_funding val, bool is_new)
        {
            var msg = string.Empty;
            if (string.IsNullOrEmpty(val.funding_id)) msg += "Please fill Funding ID !<br/>";
            if (string.IsNullOrEmpty(val.account_no)) msg += "Please fill Account No !<br/>";
            if (string.IsNullOrEmpty(val.customer_id)) msg += "Please fill Customer ID !<br/>";
            if (val.balance < 0) msg += "Balance must be more than 0 !<br/>";

            var funding_id = val.funding_id;
            if (string.IsNullOrEmpty(msg))
            {
                if (db.m_funding.Where(x => x.funding_id == funding_id).Any() && is_new) msg += $"ID Funding {funding_id} has been used by another data. Please input another!<br />";
            }
            return msg;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            m_funding tbl = db.m_funding.FirstOrDefault(a => a.funding_id == id);
            string result = "success";
            string msg = "";
            if (tbl == null) result = "failed";
            msg = "Data can't be found!";
            if (result == "success")
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.m_funding.Remove(tbl);
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