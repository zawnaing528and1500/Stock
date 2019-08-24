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
        Tree t = new Tree();
        // GET: Member
        public ActionResult GetBrowser()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }

        #region History

        #region For Transfer
        public ActionResult TransferHistory()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int Type = Convert.ToInt32(Request.QueryString["Type"]);
            if (TempData["FromDate"] != null)
            {
                ViewBag.FromDate = TempData["FromDate"].ToString();
                ViewBag.ToDate = TempData["ToDate"].ToString();
                ViewBag.Type = TempData["Type"].ToString();
            }
            else
            {
                ViewBag.FromDate = DateTime.Now.AddDays(-1).ToString("MM.dd.yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM.dd.yyyy");
                ViewBag.Type = Type;
            }
            return View();
        }
        public ActionResult ProcessTransferHistory()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }

            string FromDate = Request.Form["FromDate"];
            string ToDate = Request.Form["ToDate"];
            string Type = Request.Form["Type"];

            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["Type"] = Type;

            return RedirectToAction("TransferHistory", "Member");
        }
        #endregion

        #region Withdraw History
        public ActionResult WithdrawHistory()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            if (TempData["FromDate"] != null)
            {
                ViewBag.FromDate = TempData["FromDate"].ToString();
                ViewBag.ToDate = TempData["ToDate"].ToString();
            }
            else
            {
                ViewBag.FromDate = DateTime.Now.AddDays(-1).ToString("MM.dd.yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM.dd.yyyy");
            }
            return View();
        }
        public ActionResult ProcessWithdrawHistory()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }

            string FromDate = Request.Form["FromDate"];
            string ToDate = Request.Form["ToDate"];
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;

            return RedirectToAction("WithdrawHistory", "Member");
        }
        #endregion

        #endregion

        #region Transfer Money
        public ActionResult TransferMoney()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult ConfirmTransferMoney()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            //If Balance is Zero or transfered money is greater than Balance, redirect  TransferMoney()

            string WalletToTransfer = Request.Form["WalletToTransfer"];
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            int Balance = db.getIntByQuery("select * from Wallet where MemberID=" + MemberID, "Balance");
            int AmountToTransfer = Convert.ToInt32(Request.Form["AmountToTransfer"]);
            if (!(db.CheckByQuery("select * from Wallet where WalletNumber=N'" + WalletToTransfer + "'")))
            {
                Session["TransferMoneySession"] = "invalidWallet";
                return RedirectToAction("TransferMoney");
            }
            if (Balance == 0 || AmountToTransfer > Balance)
            {
                Session["TransferMoneySession"] = "notEnoughBalance";
                return RedirectToAction("TransferMoney");
            }
            ViewBag.WalletToTransfer = WalletToTransfer;
            ViewBag.AmountToTransfer = AmountToTransfer.ToString();
            return View();
        }
        public ActionResult ProcessConfirmTransferMoney()
        {
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            string OwnWallet = db.getStringByQuery("select * from Wallet where MemberID=" + MemberID, "WalletNumber");
            string WalletToTransfer = Request.Form["WalletToTransfer"];
            int AmountToTransfer = Convert.ToInt32(Request.Form["AmountToTransfer"]);
            t.TransferMoney(OwnWallet, WalletToTransfer, AmountToTransfer);
            //Insert to Transfer History
            int ReceiverID = db.getIntByQuery("select * from Wallet where WalletNumber=N'" + WalletToTransfer + "'", "MemberID");
            string SenderName = db.getStringByQuery("select * from Member where ID=" + MemberID, "Name");
            string ReceiverName = db.getStringByQuery("select * from Member where ID=" + ReceiverID, "Name");
            db.ChangeByQuery("insert into TransferHistory values(" + ReceiverID + "," + MemberID + "," + AmountToTransfer + ",'" + DateTime.Now + "')");
            int TransactionID = db.getIntByQuery("select * from TransferHistory where SenderID=" + MemberID + " order by ID asc", "ID");
            //Send Email
            //Get Email of sender and receiver
            string MailBodyForSender = "Dear " + SenderName + ",<br><br>You have successfully sent " + AmountToTransfer + " Kyats to " + WalletToTransfer + "(" + ReceiverName + "). Transaction ID is " + TransactionID + ". Remember Transaction ID for further reference.<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            string MailModyForReceiver = "Dear " + ReceiverName + ",<br><br>You Received " + AmountToTransfer + " Kyats From " + OwnWallet + "(" + SenderName + ").<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            t.SendEmail("Transfer Money-Myanmar IT Star Company Limited", MailBodyForSender, db.getStringByQuery("select * from Member where ID=" + MemberID, "Email"));
            t.SendEmail("Transfer Money-Myanmar IT Star Company Limited", MailModyForReceiver, db.getStringByQuery("select * from Member where ID=" + ReceiverID, "Email"));
            Session["TransferMoneySession"] = "successful";
            return RedirectToAction("TransferMoney");
        }
        #endregion

        #region Withdraw Money
        public ActionResult WithdrawMoney()
        {
            return View();
        }
        public ActionResult ConfirmWithdrawMoney()
        {
            int AmountToWithdraw = Convert.ToInt32(Request.Form["AmountToWithdraw"]);
            int MemberBankID= Convert.ToInt32(Request.Form["MemberBankID"]);
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            int Balance = db.getIntByQuery("select * from Wallet where MemberID=" + MemberID, "Balance");
            if (Balance == 0 || AmountToWithdraw > Balance)
            {
                Session["WithdrawMoneySession"] = "notEnoughBalance";
                return RedirectToAction("WithdrawMoney");
            }
            ViewBag.AmountToWithdraw = AmountToWithdraw.ToString();
            ViewBag.MemberBankID = MemberBankID.ToString();
            return View();
        }
        public ActionResult ProcessConfirmWithdrawMoney()
        {
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            string Email = db.getStringByQuery("select * from Member where ID="+MemberID,"Email");
            int AmountToWithdraw = Convert.ToInt32(Request.Form["AmountToWithdraw"]);
            int MemberBankID = Convert.ToInt32(Request.Form["MemberBankID"]);
            int BankID = db.getIntByQuery("select * from MemberBank where ID="+MemberBankID, "BankID");

            db.ChangeByQuery("insert into WithdrawHistory values("+MemberID+","+AmountToWithdraw+","+MemberBankID+",'False','"+DateTime.Now+"')");
            string MailBody = "Dear "+db.getStringByQuery("select * from Member where ID="+MemberID,"Name")+",<br><br> We received your withdwraw Request. Withdraw Amount is "+AmountToWithdraw+"Ks. It will be deposited to your "+db.getStringByQuery("select * from Bank where ID="+BankID,"Name")+" Account ("+db.getStringByQuery("select * from MemberBank where ID="+MemberBankID,"AccountNumber")+ "). It will take 24 to 48 hours to appear in your bank Account.<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            t.SendEmail("Withdraw Request-Myanmar IT Star Company Limited", MailBody, Email);
            Session["WithdrawMoneySession"] = "successful";
            return RedirectToAction("WithdrawMoney");
        }
        #endregion

        #region Active Request by Bank Deposit- 7500 Kyats
        public ActionResult RequestActiveByBankDeposit()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessRequestActiveByBankDeposit()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            string AccountNumber = Request.Form["AccountNumber"];
            int CompanyBankID =Convert.ToInt32(Request.Form["CompanyBankID"]);
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            string Email = db.getStringByQuery("select * from Member where ID=" + MemberID, "Email");
            int Rate = db.getIntByQuery("select * from DollarRate", "Rate");

            if (!(db.CheckByQuery("select * from MemberBank where MemberID="+MemberID+" and AccountNumber=N'" + AccountNumber+"'")))
            {
                Session["RequestActiveByBankDepositSession"] = "invalidBankAccount";
                return RedirectToAction("RequestActiveByBankDeposit");
            }
            int MemberBankID = db.getIntByQuery("select * from MemberBank where MemberID=" + MemberID + " and AccountNumber=N'" + AccountNumber+"'", "ID");
            int BankID = db.getIntByQuery("select * from MemberBank where ID="+MemberBankID, "BankID");
            db.ChangeByQuery("insert into RequestActiveDepositHistory values(" + MemberID+","+db.getIntByQuery("select * from MemberBank where MemberID="+MemberID+" and AccountNumber=N'"+AccountNumber+"'","ID")+"," +5 * db.getIntByQuery("select * from DollarRate", "Rate") +",'False','"+DateTime.Now+"',"+CompanyBankID+")");
           
            string MailBody = "Dear " + db.getStringByQuery("select * from Member where ID=" + MemberID, "Name") + ",<br><br> We received your Active Request By Bank Account, "+ db.getStringByQuery("select * from Bank where ID=" + BankID, "Name") + ". Deposit amount should be "+5*Rate+" Ks. We will check and update your account Active.<br><br>Kind Regards,<br>Myanmar IT Star Company Limited";
            t.SendEmail("Receive Request Active By Bank-Myanmar IT Star Company Limited", MailBody, Email);
            Session["RequestActiveByBankDepositSession"] = "successful";
            return RedirectToAction("RequestActiveByBankDeposit");
        }

        #endregion


        #region Bank Account
        public ActionResult DeleteBankAccount()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from MemberBank where ID=" + ID);
            return RedirectToAction("AddBankForm");
        }
        public ActionResult AddBankForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        //BankID, AccountNumber
        public ActionResult ProcessAddBankForm()
        {
            int BankID = Convert.ToInt32(Request.Form["BankID"]);
            string AccountNumber = Request.Form["AccountNumber"];
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            db.ChangeByQuery("insert into MemberBank values(" + MemberID + "," + BankID + ",'" + AccountNumber + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
            return RedirectToAction("AddBankForm");
        }
        #endregion

        #region Setting
        public ActionResult Setting()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }

        #region Change 2Captcha Email
        public ActionResult Change2CaptchaEmailForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessChange2CaptchaEmailForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            string CaptchaEmail = Request.Form["CaptchaEmail"];
            if (!(db.CheckByQuery("select * from MemberCaptchaEmail where MemberID=" + MemberID)))
            {
                db.ChangeByQuery("insert into MemberCaptchaEmail values("+MemberID+",'"+ CaptchaEmail + "','"+DateTime.Now+"')");
            }
            return RedirectToAction("Change2CaptchaEmailForm");
        }
        public ActionResult DeleteChange2CaptchaEmailForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int MemberID = Convert.ToInt32(Session["CurrentUserID"]);
            db.ChangeByQuery("delete from MemberCaptchaEmail where MemberID="+MemberID);
            return RedirectToAction("Change2CaptchaEmailForm");
        }
        #endregion

        #endregion
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
        public ActionResult Aggreement()
        {
            return View();
        }
        public ActionResult Construction()
        {
            return View();
        }
    }
}