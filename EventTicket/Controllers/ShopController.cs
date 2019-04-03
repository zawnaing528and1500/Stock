using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;

namespace EventTicket.Controllers
{
    public class ShopController : Controller
    {
        DBBase db = new DBBase();
        // GET: Shop
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CustomerForm()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Login/LoginForm");
            }
            return View();
        }
        public ActionResult ProcessCustomerForm()
        {
            string Name = Request.Form["Name"];
            string Phone = Request.Form["Phone"];
            string Address = Request.Form["Address"];
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            db.ChangeByQuery("insert into Customer values("+ ShopID + ",N'" + Name + "',N'" + Phone + "',N'" + Address + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
            int CustomerID = db.getIntByQuery("select * from Customer where ShopID="+ShopID+" and Name=N'"+Name+"' and Phone=N'"+Phone+"'","ID");
            Session["CustomerID"] = CustomerID;
            return RedirectToAction("HteNumberForm", "Shop");
        }
        public ActionResult HteNumberForm()
        {
            return View();
        }
        public ActionResult ProcessHteNumberForm()
        {
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int CustomerID = Convert.ToInt32(Session["CustomerID"]);
            string No = Request.Form["No"];
            db.ChangeByQuery("insert into HteNumber values(" + ShopID + "," + CustomerID + ",N'" + No + "','" + DateTime.Now.ToString("MM.dd.yyyy") + "')");
            return RedirectToAction("HteNumberForm", "Shop");
        }
        public ActionResult DeleteHteNumber()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from HteNumber where ID="+ID);
            return RedirectToAction("HteNumberForm", "Shop");
        }
        public ActionResult ViewTodayCustomer()
        {
            return View();
        }

        public ActionResult ProcessCustomerHteNumber()
        {
            CustomerHteNumber();
            return View();
        }
        private void CustomerHteNumber()
        {
            string DownloadedDate = "";
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            DataTable dt = db.getAllByQuery("select No,Date from HteNumber where ShopID="+ShopID+ " and MONTH(Date) = MONTH(GetDate())AND YEAR(Date) = YEAR(GetDate())");//your datatable
            string attachment = "attachment; filename=လကုန္​ထိုးထား​ေသာစာရင္​း.txt";

            Response.Buffer = true;
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/plain";

            string tab = "\t";string newline = "\r\n";
            Response.Write("စဥ္​" + tab);
            Response.Write("ထီနံပါတ္" + newline);

            int i=1;
            foreach (DataRow row in dt.Rows)
            {
                tab = "\t";
                Response.Write(i);
                Response.Write(tab);
                Response.Write(row["No"].ToString());
                DownloadedDate = row["Date"].ToString();
                Response.Write(newline);
                i++;
            }
            Response.Write(DownloadedDate);
            Response.End();
        }
        public ActionResult CustomerSuccessNumber()
        {
            return View();
        }
    }
}