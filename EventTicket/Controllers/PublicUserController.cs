using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.NetworkInformation;
using System.Diagnostics;
using EventTicket.App_Code;

namespace EventTicket.Controllers
{
    public class PublicUserController : Controller
    {
        // GET: PublicUser
        DBBase db = new DBBase();
        public ActionResult SeeAds()
        {
            return View();
        }
        public ActionResult SeePopup()
        {
            return View();
        }
        public ActionResult SeeBanner()
        {
            return View();
        }
        public ActionResult Index()
        {
            int CategoryID =Convert.ToInt32(Request.QueryString["CategoryID"]);
            ViewBag.CategoryID = CategoryID;
            return View();
        }
        public ActionResult SearchPost()
        {
            string SearchText = Request.Form["SearchText"];
            ViewBag.SearchText = SearchText;
            return View();
        }
        public ActionResult SeePost()
        {
            int CategoryID = Convert.ToInt32(Request.QueryString["CategoryID"]);
            ViewBag.CategoryID = CategoryID;
            return View();
        }
        #region All Functions Called by Actions
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        #endregion
        public ActionResult PostDescription()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("update Post set PostView = PostView + 1 where ID="+ID);
            ViewBag.ID = ID;
            Debug.WriteLine(GetMACAddress());
            return View();
        }

        #region Clicking Component
        public ActionResult SecreteCodeForm()
        {
            return View();
        }
        public ActionResult ProcessSecretCodeForm()
        {
            int SecretCode = Convert.ToInt32(Request.Form["SecretCode"]);
            if(db.CheckByQuery("select * from Secret where Code="+ SecretCode))
            {
                return RedirectToAction("ClickSection");
            }
            Session["SecretCodeSession"] = "wrong";
            return RedirectToAction("SecreteCodeForm");
        }
        public ActionResult ClickSection()
        {
            return View();
        }
        #endregion
    }
}