using System;
using System.Data;
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
            int TownID = Convert.ToInt32(Request.Form["TownID"]);
            string EOrgName = Request.Form["EOrgName"];
            int ECategoryID = Convert.ToInt32(Request.Form["Category"]);
            string ExpiredDate = Request.Form["ExpiredDate"];
            DateTime EDate = Convert.ToDateTime(ExpiredDate);
            db.ChangeByQuery("insert into EOrg(Name, ExpiredDate, ECategoryID, TownID) values('"+EOrgName+"','"+ExpiredDate+"',"+ ECategoryID + ","+TownID+")");
            int AllID = db.getIntByQuery("select * from EOrg where Name='"+EOrgName+"'","ID");
            db.ChangeByQuery("insert into Login(AllID,AccessLevel) values("+AllID+","+2+")");
            return RedirectToAction("setEventOrgAccount", "Owner");
        }
        public ActionResult DeleteAccount()
        {
            int EOrgID = Convert.ToInt32(Request.QueryString["EOrgID"]);
            DataTable dtEvent = db.getAllByQuery("select * from Event where EOrgID=" + EOrgID);
            foreach(DataRow row in dtEvent.Rows)
            {
                int ID = Convert.ToInt32(row["ID"]);
                //(1) Delete Event, Row, Seat, CustomerTicket
                db.ChangeByQuery("delete from CustomerTicket where SeatID in(select ID from Seat where EID=" + ID + ")");
                db.ChangeByQuery("delete from Seat where EID=" + ID);
                db.ChangeByQuery("delete from Row where EID=" + ID);
                db.ChangeByQuery("delete from Event where ID=" + ID);
            }
            db.ChangeByQuery("delete from Login where AllID=" + EOrgID + " and AccessLevel=2");
            db.ChangeByQuery("delete from EOrg where ID=" + EOrgID);
            return RedirectToAction("setEventOrgAccount");
        }
    }
}