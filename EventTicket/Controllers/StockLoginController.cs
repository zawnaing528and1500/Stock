using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;

namespace EventTicket.Controllers
{
    public class StockLoginController : Controller
    {
        // GET: StockLogin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoginForm()
        {
            return View();
        }
        public ActionResult CheckUsernameAndPassword()
        {
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            Username = Username.Replace(" ", string.Empty);
            Username = Username.Replace("\'", string.Empty);
            Password = Password.Replace(" ", string.Empty);
            Password = Password.Replace("\'", string.Empty);

            System.Diagnostics.Debug.WriteLine(Password);

            StockLoginAuthorization l = new StockLoginAuthorization();
            if (l.checkUser(Username, Password))
            {

                int UserID = l.getUserID();
                int AccessLevel = l.getAccessLevel();
                //System.Diagnostics.Debug.WriteLine(UserID);
                //System.Diagnostics.Debug.WriteLine(AccessLevel);

                Session["CurrentUserID"] = UserID;
                Session["LoginSession"] = null;

                if (AccessLevel == 1)
                {
                    return RedirectToAction("CustomerForm", "StockShop");//Changes_second parioty
                }
                else if (AccessLevel == 2)
                {
                    return RedirectToAction("OutStockForm", "StockShop");//Most Important
                }
                else
                {
                    return RedirectToAction("Index", "Customer");
                }
            }
            else
            {
                Session["LoginSession"] = "error";
                return RedirectToAction("LoginForm", "StockLogin");
            }
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            Response.Redirect("/StockLogin/LoginForm");
            return View();
        }
    }
}