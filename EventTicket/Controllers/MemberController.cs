using EventTicket.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventTicket.Controllers
{

    public class MemberController : Controller
    {
        DBBase db = new DBBase();
        // GET: Member
        public ActionResult CaptchaWebsite()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult RequestActive()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            if(db.CheckByQuery("select * from RequestActive where MemberID=" + MemberID))
            {
               Session["Message"] = "AlreadyRequest";
            }
            else
            {
                db.ChangeByQuery("insert into RequestActive values("+MemberID+")");
                Session["Message"] = "Success";
            }
            return RedirectToAction("Setting");
        }
        public ActionResult Setting()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult Dashboard()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult MyReferralByLevel()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int Level = Convert.ToInt32(Request.QueryString["Level"]);
            ViewBag.Level = Level;
            return View();
        }
        public ActionResult MyReferral()
        {

            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult MyReferralLink()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
    }
}