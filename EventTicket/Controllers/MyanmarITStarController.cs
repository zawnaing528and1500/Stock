using EventTicket.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventTicket.Controllers
{
    public class MyanmarITStarController : Controller
    {
        DBBase db = new DBBase();
        // GET: MyanmarITStar
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult AcceptActive()
        {
            //int ID = Convert.ToInt32(Request.QueryString["ID"]);
            //int MemberID = db.getIntByQuery("select * from RequestActive where ID="+ID, "MemberID");
            //db.ChangeByQuery("update Member set Active='True' where ID="+MemberID);
            return View();
        }
        public ActionResult AcceptRequest()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            //Delete and Change Active in Member Table
            db.ChangeByQuery("delete from RequestActive where ID="+ID);
            db.ChangeByQuery("update Member set Active='True' where ID=" + MemberID);
            return RedirectToAction("AcceptActive");
        }
        public ActionResult RejectRequest()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            //Delete 
            db.ChangeByQuery("delete from RequestActive where ID=" + ID);
            return RedirectToAction("AcceptActive");
        }
        public ActionResult ViewAllMembers()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
    }
}