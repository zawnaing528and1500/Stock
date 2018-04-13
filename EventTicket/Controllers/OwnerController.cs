using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
namespace EventTicket.Controllers
{
    public class OwnerController : Controller
    {
        // GET: Owner
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult setEventOrgAccount()
        {
            return View();
        }
        public ActionResult ProcessForSetEventOrgAccount()
        {
            int EOrgID = Convert.ToInt32(Request.Form["EOrgID"]);
            string Username = Request.Form["Username"];
            string Password = Request.Form["Password"];
            DBBase db = new DBBase();
            db.ChangeByQuery("update Login set Username='" + Username + "', Password= '" + Password + "' where AllID=" + EOrgID);
            return RedirectToAction("setEventOrgAccount", "Owner");
        }
    }
}