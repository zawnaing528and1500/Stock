using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Diagnostics;
using System.Net.Mail;
namespace EventTicket.Controllers
{
    public class AccountController : Controller
    {
        DBBase db = new DBBase();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        #region FAQ
        public ActionResult FAQ()
        {
            return View();
        }
        #endregion

        #region Team 
        public ActionResult Team()
        {
            return View();
        }
        #endregion

        #region Login Section
        public ActionResult LoginForm()
        {
            return View();
        }
        public ActionResult ProcessLoginForm()
        {
            #region Form Data
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            Username = Username.Replace(" ", string.Empty);
            Username = Username.Replace("\'", string.Empty);
            Password = Password.Replace(" ", string.Empty);
            Password = Password.Replace("\'", string.Empty);
            #endregion

            System.Diagnostics.Debug.WriteLine(Password);

            LoginAuthorization l = new LoginAuthorization();
            if (l.checkUser(Username, Password))
            {
                int UserID = l.getUserID();
                int AccessLevel = l.getAccessLevel();

                Session["CurrentUserID"] = UserID;
                Session["LoginSession"] = null;

                if (AccessLevel == 1)
                {
                    //14 and 29 reset RequestActive
                    if(DateTime.Now.Day == 14 || DateTime.Now.Day == 29)
                    {
                        db.ChangeByQuery("delete from RequestActive");
                        //In-Active Count
                        Tree t = new Tree();
                        t.InactiveCount();
                        db.ChangeByQuery("update Member set Active='False'");
                    }
                    return RedirectToAction("Dashboard", "MyanmarITStar");//Changes_second parioty
                }
                else if (AccessLevel == 2)
                {
                    //Wallet Balance Update
                    Tree t = new Tree();
                    t.updateWalletBalance(UserID);
                    return RedirectToAction("Dashboard", "Member");//Most Important
                }
                else if (AccessLevel == 3)
                {
                    return RedirectToAction("Dashboard", "Staff");//Most Important
                }
                else
                {
                    return RedirectToAction("Index", "Customer");
                }
            }
            else
            {
                Session["RegisterSession"] = "wrong";
                return RedirectToAction("LoginForm", "Account");
            }
        }
        #endregion

        #region Register Section
        public ActionResult RegisterForm()
        {
            if (DateTime.Now.Day == 30)
            {
                return RedirectToAction("BlockRegistration");
            }
                //Get Referral Code
            int ReferredCode=0;
            if (Request.QueryString["ReferredCode"] != null)
            {
                ReferredCode = Convert.ToInt32(Request.QueryString["ReferredCode"]);
                Debug.WriteLine(ReferredCode);
            }
            else
            {
                ViewBag.ReferredCode = 1111;
            }
            ViewBag.ReferredCode = ReferredCode;
            return View();
        }
        public string BlockRegistration()
        {
            return "<font color='red'>DM Account Registraion is not allowed at 14 th and 29 th of a month</font>";
        }
        public ActionResult RegisterCodeForm()
        {
            #region Get Form Data
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];
            string Email = Request.Form["Email"];
            string Username = Email;
            string Password = Request.Form["Password"];
            int ReferredCode = Convert.ToInt32(Request.Form["ReferredCode"]);
            string FBLink = Request.Form["FBLink"];
            int TownshipID = Convert.ToInt32(Request.Form["TownshipID"]);
            int JobID = Convert.ToInt32(Request.Form["JobID"]);
            #endregion

            #region Sending Confirmation Code
            int RegisterCode = GetRandomNumber(1000, 9999);
            Session["RegisterCode"] = RegisterCode;
            Tree t = new Tree();
            t.SendEmail("Myanmar IT Star Company Limited", "Comfirmation Code:" + RegisterCode, Email);
            #endregion

            #region Assign Data to viewbag
            ViewBag.Name = Name;
            ViewBag.Phone = Phone;
            ViewBag.Address = Address;
            ViewBag.Email = Email;
            ViewBag.Username = Username;
            ViewBag.Password = Password;
            ViewBag.ReferredCode = ReferredCode;
            ViewBag.FBLink = FBLink;
            ViewBag.TownshipID = TownshipID;
            ViewBag.JobID = JobID;
            #endregion

            return View();
        }
        public ActionResult ProcessRegisterForm()
        {
            string ServerMessage = "";

            int RegisterCode = Convert.ToInt32(Session["RegisterCode"]);
            int Code = Convert.ToInt32(Request.Form["Code"]);
            if(RegisterCode != Code)
            {
                Session["RegisterSession"] = "wrongEmail";
                return RedirectToAction("LoginForm");
            }

            #region Get Form Data
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];
            string Email = Request.Form["Email"];
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            int ReferredCode = Convert.ToInt32(Request.Form["ReferredCode"]);
            string FBLink = Request.Form["FBLink"];
            int TownshipID = Convert.ToInt32(Request.Form["TownshipID"]);
            int JobID = Convert.ToInt32(Request.Form["JobID"]);

            #endregion


            //Get Referral Code
            //Check Duplicate Phone and Username

            int RefferalCode = GetReferralCode();

            if (db.CheckByQuery("select * from Member where Phone=N'" + Phone + "'"))
            {
                ServerMessage = "duplicatePhone";
            }
            else if (db.CheckByQuery("select * from Login where Username=N'" + Username + "'"))
            {
                ServerMessage = "duplicateEmail";
            }
            else if (db.CheckByQuery("select * from Member where Email=N'" + Email + "'"))
            {
                ServerMessage = "duplicateEmail";
            }
            else
            {
                db.ChangeByQuery("insert into Member values(N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + Email + "'," + RefferalCode + ",'" + DateTime.Now.ToString("MM.dd.yyyy") + "','"+FBLink+"','False',"+TownshipID+","+JobID+")");
                int LastMemberID = db.getIntByQuery("select * from Member where Phone=N'"+Phone+"' and Email='"+Email+"'","ID");
                db.ChangeByQuery("insert into Login values("+LastMemberID+",N'"+Username+"',N'"+Password+"',2,'True','"+ DateTime.Now.ToString("MM.dd.yyyy")+ "')");
               
                //Get ID of Parent Node 
                int Parent = db.getIntByQuery("select * from Member where ReferralCode="+ReferredCode, "ID");
                //Get TreeLevel
                int TreeLevel = db.getIntByQuery("select * from Tree where Parent=" + Parent, "TreeLevel");
                TreeLevel = TreeLevel + 1;
                //Insert into Tree
                db.ChangeByQuery("insert into Tree values("+Parent+","+LastMemberID+","+TreeLevel+",'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");

                //Create Wallet Account with 8 radom code
                string MemberWallet = "M"+ GetRandomCodeForWallet();
                db.ChangeByQuery("insert into Wallet values("+LastMemberID+",'"+ MemberWallet + "',0,'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");

                ServerMessage = "successful";
            }
            Session["RegisterSession"]= ServerMessage;
            if(ServerMessage == "successful")
            {
                return RedirectToAction("LoginForm", "Account");
            }
            else
            {
                return RedirectToAction("RegisterForm", "Account");
            }
        }

        #endregion


        #region Generate Random Code
        private int GetRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        #endregion

        #region Generate ReferralCode
        private int GetReferralCode()
        {
            Random random = new Random();
            int Code = random.Next(10000, 99999);
            if (db.CheckByQuery("select * from Member where ReferralCode=" + Code))
            {
                GetReferralCode();
            }
            return Code;
        }
        #endregion

        #region Generating Wallet Code
        private int GetRandomCodeForWallet()
        {
            Random random = new Random();
            int WalletNo = random.Next(10000000, 99999999);
            String WalletNumber = "M" + WalletNo;
            if (db.CheckByQuery("select * from Wallet where WalletNumber=N'" + WalletNumber + "'"))
            {
                GetRandomCodeForWallet();
            }
            return WalletNo;
        }
        #endregion

        #region Forgot Password Section
        public ActionResult ForgotPasswordForm()
        {
            return View();
        }
        public ActionResult ProcessForgotPasswordForm()
        {
            string Email = Request.Form["Email"];
            if(!(db.CheckByQuery("select * from Member where Email='" + Email+"'")))
            {
                Session["ForgotPasswordSession"] = "error";
                return RedirectToAction("ForgotPasswordForm");
            }
            else
            {
                //Send Code to Email
                Tree t = new Tree();
                int ForgotPasswordCode = GetRandomNumber(1000,9999);
                Session["ForgotPasswordCode"] = ForgotPasswordCode;
                Session["ForgotEmail"] = Email;
                t.SendEmail("DM Group-Online Home Jobs Program", "Use "+ ForgotPasswordCode + " to reset your password.", Email);
                return RedirectToAction("CheckCodeForm");
            }
            return View();
        }
        public ActionResult CheckCodeForm()
        {
            return View();
        }
        public ActionResult CheckForgotPasswordCode()
        {
            int Code =Convert.ToInt32(Request.Form["Code"]);
            int SessionCode = Convert.ToInt32(Session["ForgotPasswordCode"]);
            if(!(Code == SessionCode))
            {
                Session["CheckCodeSession"] = "error";
                return RedirectToAction("CheckCodeForm");
            }
            else
            {
                return RedirectToAction("ReForgotPasswordForm");
            }
        }
        public ActionResult ReForgotPasswordForm()
        {
            return View();
        }
        public ActionResult ProcessReForgotPasswordForm()
        {
            int NewPassword = Convert.ToInt32(Request.Form["NewPassword"]);
            string Email = Session["ForgotEmail"].ToString();
            int MemberID = db.getIntByQuery("select * from Member where Email='"+Email+"'","ID");
            db.ChangeByQuery("update Login set Password='" + NewPassword + "' where AllID=" + MemberID + " and AccessLevel=2");
            Session.Remove("ForgotPasswordSession"); Session.Remove("ForgotPasswordCode"); Session.Remove("ForgotEmail"); Session.Remove("CheckCodeSession");
            return RedirectToAction("LoginForm");
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            Session.RemoveAll();
            Response.Redirect("/Account/LoginForm");
            return View();
        }
        #endregion

    }
}