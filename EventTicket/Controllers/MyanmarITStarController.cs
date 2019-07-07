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
        DBBase db = new DBBase();Tree t = new Tree();
        // GET: MyanmarITStar
        public ActionResult Dashboard()
        {
            return View();
        }

        #region Accept Active
        public ActionResult AcceptActive()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            //int ID = Convert.ToInt32(Request.QueryString["ID"]);
            //int MemberID = db.getIntByQuery("select * from RequestActive where ID="+ID, "MemberID");
            //db.ChangeByQuery("update Member set Active='True' where ID="+MemberID);
            return View();
        }
        public ActionResult AcceptRequest()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            //Delete and Change Active in Member Table
            db.ChangeByQuery("delete from RequestActive where ID=" + ID);
            db.ChangeByQuery("update Member set Active='True' where ID=" + MemberID);
            return RedirectToAction("AcceptActive");
        }
        public ActionResult RejectRequest()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            //Delete 
            db.ChangeByQuery("delete from RequestActive where ID=" + ID);
            return RedirectToAction("AcceptActive");
        }
        #endregion

        #region Notification
        public ActionResult Notification()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult SeeRequestActiveByBank()
        {
            return View();
        }
        #region Withdraw Request
        public ActionResult WithdrawRequest()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult AcceptWithdrawRequest()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int WithdrawHistoryID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            int MemberBankID = db.getIntByQuery("select * from WithdrawHistory where ID="+WithdrawHistoryID, "MemberBankID");
            int BankID = db.getIntByQuery("select * from MemberBank where ID=" + MemberBankID, "BankID");

            string Email = db.getStringByQuery("select * from Member where ID=" + MemberID, "Email");
            int WithdrawedAmount = db.getIntByQuery("select * from WithdrawHistory where ID="+WithdrawHistoryID, "WithdrawedAmount");
            db.ChangeByQuery("update Wallet set Balance=Balance-"+WithdrawedAmount+"  where MemberID="+MemberID);
            db.ChangeByQuery("update WithdrawHistory set Proof='True' where ID="+WithdrawHistoryID+" and MemberID="+MemberID);
            string MailBody = "Dear " + db.getStringByQuery("select * from Member where ID=" + MemberID, "Name") + ",<br><br> We have sent "+ WithdrawedAmount + "Ks to your bank account " + db.getStringByQuery("select * from Bank where ID=" + BankID, "Name") + " Account (" + db.getStringByQuery("select * from MemberBank where ID=" + MemberBankID, "AccountNumber") + ").<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            t.SendEmail("Withdraw Fund Sent-DM Group", MailBody, Email);
            return RedirectToAction("WithdrawRequest");
        }
        public ActionResult RejectWithdrawRequest()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int WithdrawHistoryID = Convert.ToInt32(Request.QueryString["ID"]);
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            int MemberBankID = db.getIntByQuery("select * from WithdrawHistory where ID=" + WithdrawHistoryID, "MemberBankID");
            int BankID = db.getIntByQuery("select * from MemberBank where ID=" + MemberBankID, "BankID");

            string Email = db.getStringByQuery("select * from Member where ID=" + MemberID, "Email");
            int WithdrawedAmount = db.getIntByQuery("select * from WithdrawHistory where ID=" + WithdrawHistoryID, "WithdrawedAmount");
            db.ChangeByQuery("delete From WithdrawHistory where ID="+WithdrawHistoryID);
            string MailBody = "Dear " + db.getStringByQuery("select * from Member where ID=" + MemberID, "Name") + ",<br><br> We have Rejected of Withdrawing " + WithdrawedAmount + "Ks to your bank account [" + db.getStringByQuery("select * from Bank where ID=" + BankID, "Name") + " Account (" + db.getStringByQuery("select * from MemberBank where ID=" + MemberBankID, "AccountNumber") + ") because bank account name does not matched with your name.<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            t.SendEmail("Withdraw Fund Rejected-DM Group", MailBody, Email);
            return RedirectToAction("WithdrawRequest");
        }
        #endregion

        #endregion

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