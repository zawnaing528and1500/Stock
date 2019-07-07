using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
namespace EventTicket.Controllers
{
    public class StaffController : Controller
    {
        DBBase db = new DBBase();
        Tree t = new Tree();
        // GET: Staff
        public ActionResult Dashboard()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult DeleteMember()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int MemberID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from WithdrawHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Wallet where MemberID=" + MemberID);

            //Delete Node
            int Parent = db.getIntByQuery("select * from Tree where Child=" + MemberID, "Parent");
            if(Parent == 0) { Parent = 80; }
            db.ChangeByQuery("update Tree set Parent= "+Parent+" where Parent=" + MemberID);

            db.ChangeByQuery("delete from Tree where Parent=" + MemberID);
            db.ChangeByQuery("delete from Tree where Child=" + MemberID);

            db.ChangeByQuery("delete from TransferHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from RequestActiveDepositHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from RequestActive where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Payment where MemberID=" + MemberID);
            db.ChangeByQuery("delete from MemberCaptchaEmail where MemberID=" + MemberID);
            db.ChangeByQuery("delete from MemberBank where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Login where AllID=" + MemberID+" and AccessLevel = 2");
            db.ChangeByQuery("delete from Member where ID=" + MemberID);
            return RedirectToAction("ViewAllMembers");
        }
        public ActionResult ViewAllMembers()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            if (TempData["From"] != null)
            {
                ViewBag.From = TempData["From"].ToString();
                ViewBag.To = TempData["To"].ToString();
            }
            else
            {
                ViewBag.From = 0;
                ViewBag.To = 100;
            }
            return View();
        }
        public ActionResult ProcessViewAllMembers()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }

            string From = Request.Form["From"];
            string To = Request.Form["To"];
            TempData["From"] = From;
            TempData["To"] = To;

            return RedirectToAction("ViewAllMembers", "Staff");
        }
    }
}