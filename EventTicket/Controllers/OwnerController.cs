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
        public ActionResult ProcessApplyOrgAccount()
        {
            string Info = "success";
            string Name = Request.Form["Name"];
            int TownID = Convert.ToInt32(Request.Form["TownID"]);
            int ECategory = Convert.ToInt32(Request.Form["Category"]);
            string Email = Request.Form["Email"];
            string Phone = Request.Form["Phone"];
            string Version = Request.Form["Version"];
            string AccountType = Request.Form["AccountType"];
            //Check with phone number. Insert into AppliedAccount. 
            if(db.CheckByQuery("select * from EOrg where Phone='" + Phone + "'") || db.CheckByQuery("select * from AppliedAccount where Phone='" + Phone + "'"))
            {
                Info = "duplicatePhone";
            }
            else
            {
                db.ChangeByQuery("insert into AppliedAccount(Name, TownID, ECategoryID, Email, Phone, Version, AccountType) values(N'"+Name+"',"+TownID+","+ECategory+",'"+Email+"','"+Phone+"','"+Version+"','"+AccountType+"')");
            }
            Session["Info"] = Info;
            string url = Session["appliedAccountUrl"].ToString();
            Response.Redirect(url);
            return View();
        }
        public ActionResult SeeAppliedAccount()
        {
            return View();
        }
        public ActionResult RemoveAppliedAccount()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from AppliedAccount where ID="+ID);
            return RedirectToAction("SeeAppliedAccount");
        }
        public ActionResult AddAppliedAccount()
        {
            string ExpiredDate = "";
            string Name = Request.QueryString["Name"];
            int TownID = Convert.ToInt32(Request.QueryString["TownID"]);
            string Email = Request.QueryString["Email"];
            string Version = Request.QueryString["Version"];
            string AccountType = Request.QueryString["AccountType"];
            int ECategoryID = Convert.ToInt32(Request.QueryString["ECategoryID"]);
            string Phone = Request.QueryString["Phone"];
            if(AccountType.Equals("1 year"))
            {
                ExpiredDate = "07/17/2019";
            }
            else if(AccountType.Equals("2 year"))
            {
                ExpiredDate = "07/17/2020";
            }
            else if (AccountType.Equals("3 year"))
            {
                ExpiredDate = "07/17/2021";
            }
            //Insert into EOrg and Login (AllID, Username, Password, AccessLevel)
            db.ChangeByQuery("insert into EOrg(Name,TownID,Email,ExpiredDate,ECategoryID,Phone) values(N'"+Name+"',"+TownID+",'"+Email+"','"+ExpiredDate+"',"+ECategoryID+",'"+Phone+"')");
            int AllID = db.getIntByQuery("select * from EOrg where Name=N'"+Name+"' and Phone='"+Phone+"'","ID");
            db.ChangeByQuery("insert into Login(AllID,Username,Password,AccessLevel) values("+AllID+",'Not Set','Not Set',2)");
            db.ChangeByQuery("delete from AppliedAccount where Name=N'" + Name + "' and Phone='" + Phone + "'");
            return RedirectToAction("setEventOrgAccount");
        }
    }
}