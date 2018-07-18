using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;

namespace EventTicket.Controllers
{
    public class PublicUserController : Controller
    {
        DecryptParameter dp = new DecryptParameter();
        DBBase d = new DBBase();
        // GET: PublicUser
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ApplyEOrgAccount()
        {
            return View();
        }
        public ActionResult AllEvent()
        {
            return View();
        }
        public ActionResult MoreDescription()
        {
            string E_EID = Request.QueryString["E_EID"];
            E_EID = E_EID.Replace(" ", "+");
            String EID = dp.Decrypt(E_EID, "ETicket");
            ViewBag.EID = Convert.ToInt32(EID);
            return View();
        }
        public ActionResult SeeCinemaOrgByTown()
        {
            return View();
        }
        public ActionResult SeeCinemaOrg()
        {
            string Town = Request.QueryString["Town"];
            ViewBag.Town = Town;
            return View();
        }
        public ActionResult SeeCarGateByTown()
        {
            return View();
        }
        public ActionResult SeeCarGateDate()
        {
            string Town = Request.QueryString["Town"];
            ViewBag.Town = Town;
            return View();
        }
        public ActionResult SeeCarGate()
        {
            string Town = Request.QueryString["Town"];
            string EDate = Request.QueryString["EDate"];
            ViewBag.Town = Town;
            ViewBag.EDate = EDate;
            return View();
        }
        public ActionResult Event()
        {
            string ECategoryID = Request.QueryString["ECategoryID"];
            ViewBag.ECID = Convert.ToInt32(ECategoryID);
           
            if(Request.QueryString["EOrgID"] != null)
            {
                ViewBag.EOID = Convert.ToInt32(Request.QueryString["EOrgID"]);
            }
            return View();
        }

        public ActionResult AllTicket()
        {
            string E_EID = Request.QueryString["E_EID"];
            E_EID = E_EID.Replace(" ", "+");
            String EID = dp.Decrypt(E_EID, "ETicket");
            ViewBag.EID = Convert.ToInt32(EID);
            return View();
        }

        public ActionResult BookSeat()
        {
            string passedInfo = "success";
            int EID = Convert.ToInt32(Request.Form["EventID"]);
            string SelectedSeat = Request.Form["SelectedSeat"];
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Bank = Request.Form["Bank"];
            string Account = Request.Form["Account"];
            string[] separater = { "," };
            string[] selectedSeat = SelectedSeat.Split(separater, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            if(selectedSeat.Length > 0)
            {
                foreach (var word in selectedSeat)
                {
                    //In case of two parallel action, check Seat Status (Owner is Sold and User is Book)
                    if (d.getStringByQuery("select * from Seat where EID=" + EID + " and Name='"+selectedSeat[i]+"'", "Status").Equals("Sold"))
                    {
                        i = i + 1;
                        passedInfo = "justSold";
                        continue;
                    }
                    d.ChangeByQuery("update Seat set Status='Book' where EID=" + EID + " and Name='" + selectedSeat[i] + "'");
                    int SeatID = d.getIntByQuery("select * from Seat where EID=" + EID + " and Name='" + selectedSeat[i] + "'", "ID");
                    d.ChangeByQuery("insert into CustomerTicket values(N'" + Name + "',N'" + Phone + "'," + SeatID + ",N'"+Bank+"',N'"+Account+"','"+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")+"')");
                    i = i + 1;
                    passedInfo = "success";
                }
            }
            else
            {
                passedInfo = "noSeat";
            }
            Session["passedInfo"] = passedInfo;
            string url = Session["userurl"].ToString();
            Response.Redirect(url);
            return View();
        }

        public ActionResult AboutCEO()
        {
            return View();
        }
        public ActionResult Construction()
        {
            return View();
        }
    }
}