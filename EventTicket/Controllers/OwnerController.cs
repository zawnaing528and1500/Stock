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

        DBBase db = new DBBase();
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
            db.ChangeByQuery("update Login set Username='" + Username + "', Password= '" + Password + "' where AllID=" + EOrgID+" and AccessLevel=2");
            return RedirectToAction("setEventOrgAccount", "Owner");
        }
        public ActionResult ProcessForAddEventOrgAccount()
        {
            string EOrgName = Request.Form["EOrgName"];
            string Email = Request.Form["Email"];
            string ExpiredDate = Request.Form["ExpiredDate"];
            DateTime EDate = Convert.ToDateTime(ExpiredDate);
            db.ChangeByQuery("insert into EOrg(Name, Email, ExpiredDate) values('"+EOrgName+"','"+Email+"','"+ExpiredDate+"')");
            int AllID = db.getIntByQuery("select * from EOrg where Name='"+EOrgName+"'","ID");
            db.ChangeByQuery("insert into Login(AllID,AccessLevel) values("+AllID+","+2+")");
            return RedirectToAction("setEventOrgAccount", "Owner");
        }
    }
}