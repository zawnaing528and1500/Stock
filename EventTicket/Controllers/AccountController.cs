using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Diagnostics;
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
                    return RedirectToAction("Dashboard", "MyanmarITStar");//Changes_second parioty
                }
                else if (AccessLevel == 2)
                {
                    return RedirectToAction("Dashboard", "Member");//Most Important
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
            //Get Referral Code
            int ReferredCode=0;
            if (Request.QueryString["ReferredCode"] != null)
            {
                ReferredCode = Convert.ToInt32(Request.QueryString["ReferredCode"]);
                Debug.WriteLine(ReferredCode);
            }
            ViewBag.ReferredCode = ReferredCode;
            return View();
        }
        public ActionResult ProcessRegisterForm()
        {
            string ServerMessage = "";         

            #region Get Form Data
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];
            string Email = Request.Form["Email"];
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            int ReferredCode = Convert.ToInt32(Request.Form["ReferredCode"]);
            string FBLink = Request.Form["FBLink"];
            #endregion

            //Get Referral Code
            //Check Duplicate Phone and Username

            int RefferalCode = GetRandomRefferalCode(1000,9999);

            if (db.CheckByQuery("select * from Member where Phone=N'" + Phone + "'"))
            {
                ServerMessage = "duplicatePhone";
            }
            else if (db.CheckByQuery("select * from Login where Username=N'" + Username + "'"))
            {
                ServerMessage = "duplicateUsername";
            }
            else if (db.CheckByQuery("select * from Member where Email=N'" + Email + "'"))
            {
                ServerMessage = "duplicateEmail";
            }
            else
            {
                db.ChangeByQuery("insert into Member values(N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + Email + "'," + RefferalCode + ",'" + DateTime.Now.ToString("MM.dd.yyyy") + "','"+FBLink+"','False')");
                int LastMemberID = db.getIntByQuery("select * from Member where Phone=N'"+Phone+"'","ID");
                db.ChangeByQuery("insert into Login values("+LastMemberID+",N'"+Username+"',N'"+Password+"',2,'True','"+ DateTime.Now.ToString("MM.dd.yyyy")+ "')");
               
                //Get ID of Parent Node 
                int Parent = db.getIntByQuery("select * from Member where ReferralCode="+ReferredCode, "ID");
                //Get TreeLevel
                int TreeLevel = db.getIntByQuery("select * from Tree where Parent=" + Parent, "TreeLevel");
                TreeLevel = TreeLevel + 1;
                //Insert into Tree
                db.ChangeByQuery("insert into Tree values("+Parent+","+LastMemberID+","+TreeLevel+",'"+ DateTime.Now.ToString("MM.dd.yyyy") + "')");

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

        #region Generate Refferal Code
        private int GetRandomRefferalCode(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        #endregion

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