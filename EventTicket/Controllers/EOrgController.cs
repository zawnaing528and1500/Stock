using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;

namespace EventTicket.Controllers
{
    public class EOrgController : Controller
    {

        DBBase d = new DBBase();
        // GET: EOrg
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEventData(HttpPostedFileBase file)
        {
            string ImageName = "";
            if (file.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                ImageName = fileName;
                var path = System.IO.Path.Combine(Server.MapPath("~/Image"), fileName);
                file.SaveAs(path);
            }
            string Name = Request.Form["Name"];
            string Category = Request.Form["Category"];
            string Date = Request.Form["Date"];
            string Place = Request.Form["Place"];
            string Email = Request.Form["Email"];
            string Phone = Request.Form["Phone"];
            string IsFree = Request.Form["isFree"];
            int TotalTicket = Convert.ToInt32(Request.Form["TotalTicket"]);
            string Description = Request.Form["Description"];
            //Get EOrgID. Set to 1 in unit testing
            int ECategoryID = Convert.ToInt32(Category);
            DateTime EDate = Convert.ToDateTime(Date);
            int EOrgID = 1;
            d.ChangeByQuery("insert into Event(EOrgID,ECategoryID,Name,ImageName,Place,EDate,Email,Phone,TotalTicket,IsFree,Description) values("+EOrgID+","+ECategoryID+",'"+Name+"','"+ImageName+"','"+Place+"','"+EDate+"','"+Email+"','"+Phone+"','"+TotalTicket+"','"+IsFree+"','"+Description+"')");
            Seat s = new Seat();
            s.setTotalTicket(TotalTicket, 1);
            s.setFirstTimeSeat();
            return View();
        }
        public ActionResult Manage()
        {
            //Check if there is event or not. Retrieve EOrgID from Session. 
            int EOrgID = 1;//Convert.ToInt32(Session["CurrentUserID"]);
            if(d.CheckByQuery("select * from Event where EOrgID=" + EOrgID) == false)
            {
                return RedirectToAction("NotFound");
            }

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}