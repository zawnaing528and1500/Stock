using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;
namespace EventTicket.Controllers
{
    public class EOrgController : Controller
    {
        int EOrgID = 1;//Convert.ToInt32(Session["CurrentUserID"]);

        DecryptParameter dp = new DecryptParameter();
        DBBase d = new DBBase();
        public ActionResult Index()
        {
            return View();
        }

        //CreateEvent, AddEventData, DeleteEvent, Manage, NotFound
        #region Event
        public ActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEventData(HttpPostedFileBase file, HttpPostedFileBase fileMap)
        {
            EOrgID = Convert.ToInt32(Session["CurrentUserID"]);
            string ImageName = "";
            String ImageNameMap = "";
            if (file.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                ImageName = fileName;
                var path = System.IO.Path.Combine(Server.MapPath("~/Image/Cover"), fileName);
                file.SaveAs(path);
            }
            if (fileMap.ContentLength > 0)
            {
                var fileNameMap = System.IO.Path.GetFileName(fileMap.FileName);
                ImageNameMap = fileNameMap;
                var path = System.IO.Path.Combine(Server.MapPath("~/Image/Seat_Map"), fileNameMap);
                fileMap.SaveAs(path);
            }
            string Name = Request.Form["Name"];
            string Category = Request.Form["Category"];
            string Date = Request.Form["Date"];
            string Time = Request.Form["Time"];
            string Place = Request.Form["Place"];
            string Email = Request.Form["Email"];
            string Phone = Request.Form["Phone"];
            string IsFree = Request.Form["isFree"];
            int Row = Convert.ToInt16(Request["Row"]);
            int TotalTicket = 0;//Convert.ToInt32(Request.Form["TotalTicket"]);
            string Description = Request.Form["Description"];
            //Get EOrgID. Set to 1 in unit testing
            int ECategoryID = Convert.ToInt32(Category);
            DateTime EDate = Convert.ToDateTime(Date);
            d.ChangeByQuery("insert into Event(EOrgID,ECategoryID,Name,ImageName,Place,EDate,Email,Phone,TotalTicket,IsFree,Description,Row,SeatMap,Time) values(" + EOrgID + "," + ECategoryID + ",'" + Name + "','" + ImageName + "','" + Place + "','" + EDate + "','" + Email + "','" + Phone + "','" + TotalTicket + "','" + IsFree + "','" + Description + "'," + Row + ",'"+ ImageNameMap + "','"+Time+"')");
            int EID = d.getIntByQuery("select top 1 * From Event where EOrgID=" + EOrgID + " order by ID desc", "ID");
            Row r = new Row();
            r.set(EID, Row);
            //Seat s = new Seat();
            //s.setTotalTicket(TotalTicket, 1);
            //s.setFirstTimeSeat();
            return RedirectToAction("Manage");
        }
        public ActionResult DeleteEvent()
        {
            string E_EID = Request.QueryString["E_EID"];
            E_EID = E_EID.Replace(" ", "+");
            String EID = dp.Decrypt(E_EID, "ETicket");
            int ID = Convert.ToInt32(EID);
            //Delete Cover and SeatMap
            string Cover = d.getStringByQuery("select * from Event where ID=" + ID, "ImageName");
            string SeatMap = d.getStringByQuery("select * from Event where ID=" + ID, "SeatMap");
            DeleteCover(Cover);DeleteSeatMap(SeatMap);
            //(1) Delete Event, Row, Seat, CustomerTicket
            d.ChangeByQuery("delete from CustomerTicket where SeatID in(select ID from Seat where EID=" + ID + ")");
            d.ChangeByQuery("delete from Seat where EID=" + ID);
            d.ChangeByQuery("delete from Row where EID=" + ID);
            d.ChangeByQuery("delete from Event where ID=" + ID);
            return RedirectToAction("Manage");
        }
        public ActionResult Manage()
        {
            EOrgID = Convert.ToInt32(Session["CurrentUserID"]);
            //Check if there is event or not. Retrieve EOrgID from Session. 
            if (d.CheckByQuery("select * from Event where EOrgID=" + EOrgID) == false)
            {
                return RedirectToAction("NotFound");
            }

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        #endregion

        //SetRowDetail, InsertRowDetail, SetSeatStatus, InsertSeatStatus, SellSeatManually
        #region Seat
        public ActionResult SetRowDetail()
        {
            string E_EID = Request.QueryString["E_EID"];
            E_EID = E_EID.Replace(" ", "+");
            String EID = dp.Decrypt(E_EID, "ETicket");
            ViewBag.EID = Convert.ToInt32(EID);
            return View();
        }
        public ActionResult InsertRowDetail()
        {
            int EID = Convert.ToInt32(Request.Form["EID"]);
            int Row = Convert.ToInt32(Request.Form["Row"]);
            int NumberOfSeat = Convert.ToInt32(Request.Form["NumberOfSeat"]);
            string StartingCharacter = Request.Form["StartingCharacter"];
            int Price = Convert.ToInt32(Request.Form["Price"]);
            //When a row is updated, Row/Seat/CustomerTicket are deleted.
            d.ChangeByQuery("update Row set NumberOFSeat=" + NumberOfSeat + ",StartingCharacter='" + StartingCharacter + "',Price=" + Price + " where EID=" + EID + " and Row=" + Row);
            d.ChangeByQuery("delete from CustomerTicket where SeatID in(select ID from Seat where EID=" + EID + " and Row=" + Row + ")");
            d.ChangeByQuery("delete from Seat where EID=" + EID + " and Row=" + Row);
            for (int i = 1; i <= NumberOfSeat; i++)
            {
                string Name = StartingCharacter + i;
                d.ChangeByQuery("insert into Seat values(" + EID + ",'" + Name + "'," + Row + "," + Price + ",'Free')");
            }
            string url = Session["url"].ToString();
            Response.Redirect(url);
            return View();
        }
        public ActionResult SetSeatStatus()
        {
            string E_EID = Request.QueryString["E_EID"];
            E_EID = E_EID.Replace(" ", "+");
            String EID = dp.Decrypt(E_EID, "ETicket");
            ViewBag.EID = Convert.ToInt32(EID);
            return View();
        }
        public ActionResult InsertSeatStatus()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            string Status = Request.Form["Status"];
            if (Status.Equals("Free"))
            {
                //Delete Book Detail first. Then update seat status
                d.ChangeByQuery("delete from CustomerTicket where SeatID=" + ID);
            }
            d.ChangeByQuery("update Seat set Status='" + Status + "' where ID=" + ID);
            string url = Session["url"].ToString();
            Response.Redirect(url);
            return View();
        }
        public ActionResult SellSeatManually()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            d.ChangeByQuery("insert into CustomerTicket(Name,Phone,SeatID) values(N'" + Name + "',N'" + Phone + "'," + ID + ")");
            d.ChangeByQuery("update Seat set Status='Sold' where ID=" + ID);
            string url = Session["url"].ToString();
            Response.Redirect(url);
            return View();
        }
        #endregion

        #region Delete Cover and SeatMap
        public void DeleteCover(string StrFilename)
        {
            string strPhysicalFolder = Server.MapPath("~/Image/Cover/");
            string strFileFullPath = strPhysicalFolder + StrFilename;
            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
        }
        public void DeleteSeatMap(string StrFilename)
        {
            string strPhysicalFolder = Server.MapPath("~/Image/Seat_Map/");
            string strFileFullPath = strPhysicalFolder + StrFilename;
            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
        }

        #endregion

        //AddSMSGate, SendSeatInfo
        #region SMSGate
        public ActionResult AddSMSGate()
        {
            return View();
        }
        public ActionResult SendSeatInfo()
        {

            if (d.CheckByQuery("select * from SMSGate where EOrgID=" + EOrgID) == true)
            {
                DataTable dt = d.getAllByQuery("select * from SMSGate where EOrgID=" + EOrgID);
                foreach (DataRow row in dt.Rows)
                {
                    string Email = row["Email"].ToString();
                    string Password = row["Password"].ToString();
                    int Device = Convert.ToInt32(row["Device"]);
                    string Number = "09257706564";
                    Number = "+95" + Number.Remove(0, 1);
                    string Message = "Name:ေဇာ္နိုင္/Seat:A5";
                    string all = "email=" + Email + "&password=" + Password + "&device=" + Device + "&number=" + Number + "&message=" + Message;
                    MyWebRequest myRequest = new MyWebRequest("http://smsgateway.me/api/v3/messages/send", "POST", all);
                    //show the response string on the console screen. 
                    try
                    {
                        ViewData["status"] = myRequest.GetResponse();
                        if (ViewData["status"].ToString() == "OK")
                        {
                            break;
                        }
                    }
                    catch
                    {
                        ViewData["status"] = "InvalidSMSGate";
                        continue;
                    }
                }

            }
            else
            {
                ViewData["status"] = "NoSMSGate";
            }
            return View();
        }
        public ActionResult InsertSMSGate()
        {
            string  Email = Request.Form["Email"];
            string Password = Request.Form["Password"];
            int Device = Convert.ToInt32(Request.Form["Device"]);
            string Phone = Request.Form["Phone"];
            d.ChangeByQuery("insert into SMSGate values("+EOrgID+",'"+Email+"','"+Password+"',"+Device+",'"+Phone+"')");
            return RedirectToAction("AddSMSGate");
        }
        public ActionResult DeleteSMSGate()
        {
            int ID = Convert.ToInt32(Request.QueryString["SMSGateID"]);
            d.ChangeByQuery("delete from SMSGate where ID=" + ID + " and EOrgID=" + EOrgID);
            return RedirectToAction("AddSMSGate");
        }
        #endregion





    }
}