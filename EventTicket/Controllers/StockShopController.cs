using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventTicket.App_Code;
using System.Data;

namespace EventTicket.Controllers
{
    public class StockShopController : Controller
    {
        StockDBBase stock = new StockDBBase();

        // GET: StockShop
        public ActionResult Index()
        {
            return View();
        }

        #region Pack Product
        public ActionResult NewProductPackingForm()
        {
            return View();
        }
        public ActionResult ProcessNewProductPackingForm()
        {
            string Name = Request.Form["Name"];
            int Quantity = Convert.ToInt32(Request.Form["Quantity"]);
            int Minimum = Convert.ToInt32(Request.Form["Minimium"]);
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            string ExpiryDate = Request.Form["ExpiryDate"];
            if (ExpiryDate == null)
            {
                stock.ChangeByQuery("insert into Product values(" + ShopID + ",N'" + Name + "'," + Quantity + "," + Minimum + ",'True','" + DateTime.Now.ToString("MM.dd.yyyy") + "',NULL)");

            }
            else
            {
                stock.ChangeByQuery("insert into Product values(" + ShopID + ",N'" + Name + "'," + Quantity + "," + Minimum + ",'True','" + DateTime.Now.ToString("MM.dd.yyyy") + "','"+ExpiryDate+"')");
            }
            return RedirectToAction("NewProductPackingForm", "StockShop");
        }
        public ActionResult DeleteProductPacking()
        {
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Product where ID=" + ID + " and ShopID=" + ShopID);
            return RedirectToAction("NewProductPackingForm", "StockShop");
        }
        #endregion

        #region Unit Product
        public ActionResult NewProductUnitForm()
        {
            return View();
        }
        public ActionResult ProcessNewProductUnitForm()
        {
            string Name = Request.Form["Name"];
            int Quantity = Convert.ToInt32(Request.Form["Quantity"]);
            int Minimum = Convert.ToInt32(Request.Form["Minimium"]);
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);

            string ExpiryDate = Request.Form["ExpiryDate"];
            if (ExpiryDate == null)
            {
                stock.ChangeByQuery("insert into Product values(" + ShopID + ",N'" + Name + "'," + Quantity + "," + Minimum + ",'False','" + DateTime.Now.ToString("MM.dd.yyyy") + "',NULL)");

            }
            else
            {
                stock.ChangeByQuery("insert into Product values(" + ShopID + ",N'" + Name + "'," + Quantity + "," + Minimum + ",'False','" + DateTime.Now.ToString("MM.dd.yyyy") + "','" + ExpiryDate + "')");
            }
            return RedirectToAction("NewProductUnitForm", "StockShop");
        }
        public ActionResult DeleteProductUnit()
        {
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Product where ID=" + ID + " and ShopID=" + ShopID);
            return RedirectToAction("NewProductUnitForm", "StockShop");
        }
        #endregion

        #region Re Pack Product
        
        public ActionResult ReProductPackingForm()
        {
            return View();
        }
        public  ActionResult ProcessReProductPackingForm()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            int Quantity = Convert.ToInt32(Request.Form["Quantity"]);
            string ExpiryDate = Request.Form["ExpiryDate"];
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            stock.ChangeByQuery("update Product set Quantity=Quantity+" + Quantity+", ExpiryDate = '"+ExpiryDate+"',Date='"+ DateTime.Now.ToString("MM.dd.yyyy") + "' where ID="+ID+" and ShopID="+ShopID);
            return RedirectToAction("ReProductPackingForm", "StockShop");
        }

        #endregion

        #region Re Unit Product
        #endregion


        public ActionResult OutStockForm()
        {
            return View();
        }
        public ActionResult InStockForm()
        {
            return View();
        }

    }
}