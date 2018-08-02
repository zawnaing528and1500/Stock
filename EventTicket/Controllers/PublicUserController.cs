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
            int TotalCost = 0;String TransferBill = "";string SeatList = "";
            string passedInfo = "success";
            int EID = Convert.ToInt32(Request.Form["EventID"]);
            string EOrgPhone = d.getStringByQuery("select * from Event where ID=" + EID, "Phone");
            string SelectedSeat = Request.Form["SelectedSeat"];
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Operator = Request.Form["Operator"];
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
                    d.ChangeByQuery("insert into CustomerTicket(Name, Phone, SeatID, InsertedDate) values(N'" + Name + "',N'" + Phone + "'," + SeatID + ",'"+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")+"')");
                    TotalCost = TotalCost + d.getIntByQuery("select * from Seat where ID=" + SeatID,"Price");
                    SeatList = SeatList + selectedSeat[i]+"/";
                    i = i + 1;
                    passedInfo = "success";
                }
            }
            else
            {
                passedInfo = "noSeat";
            }


            //Get Phone 
            //Get Operator 
            //amount - Total Price
            /*
             MPT - *223*amount*number#
             Ooredoo - *155*amount*number#
             MEC - *110*amount*number#
             */

            //Check Operator for tranferring phone bill
            if (passedInfo.Equals("success"))
            {
                if (Operator.Equals("mpt"))
                {
                    TransferBill = "*223*" + TotalCost + "*" + EOrgPhone + "#";
                }
                else if (Operator.Equals("ooredoo"))
                {
                    TransferBill = "*155*" + TotalCost + "*" + EOrgPhone + "#";
                }
                else if (Operator.Equals("mec"))
                {
                    TransferBill = "*110*" + TotalCost + "*" + EOrgPhone + "#";
                }

                Session["passedInfo"] = passedInfo;
                string url = Session["userurl"].ToString();

                ViewBag.TransferBill = TransferBill;
                ViewBag.FromPhone = Phone;
                ViewBag.ToPhone = EOrgPhone;
                ViewBag.TotalCost = TotalCost;
                ViewBag.SeatList = SeatList;
                return View();
            }
       else
            {
                Session["passedInfo"] = passedInfo;
                string url = Session["userurl"].ToString();
                Response.Redirect(url);
                return View();
            }

            
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