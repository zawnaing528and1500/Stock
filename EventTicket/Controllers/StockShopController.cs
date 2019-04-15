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
        public ActionResult ReProductUnitForm()
        {
            return View();
        }
        public ActionResult ProcessReProductUnitForm()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            int Quantity = Convert.ToInt32(Request.Form["Quantity"]);
            string ExpiryDate = Request.Form["ExpiryDate"];
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            stock.ChangeByQuery("update Product set Quantity=Quantity+" + Quantity + ", ExpiryDate = '" + ExpiryDate + "',Date='" + DateTime.Now.ToString("MM.dd.yyyy") + "' where ID=" + ID + " and ShopID=" + ShopID);
            return RedirectToAction("ReProductUnitForm", "StockShop");
        }
        #endregion


        #region Out of Packing Product
        public ActionResult OutProductPackingForm()
        {
            return View();
        }
        public ActionResult ProcessOutProductPackingForm()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            int OutQuantity = Convert.ToInt32(Request.Form["Quantity"]);
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int TotalQuantity = stock.getIntByQuery("select * from Product where ID=" + ID, "Quantity");
            if (OutQuantity <= TotalQuantity)
            {
                stock.ChangeByQuery("update Product set Quantity=Quantity-" + OutQuantity + " where ID=" + ID + " and ShopID=" + ShopID);
                string Name = stock.getStringByQuery("select * from Product where ID="+ID, "Name");
                int LeftQuantity = TotalQuantity - OutQuantity;
                Session["OutInfo"] = "["+Name+"] "+OutQuantity+ " (ပါကင္​) ထုတ္​ၿပီးပါၿပီ။ လက္​က်န္​မွာ "+LeftQuantity+ " (ပါကင္)​ျဖစ္​သည္​";
            }
            else
            {
                Session["OutInfo"] = "ပစၥည္​း လက္​က်န္​ မလံု​ေလာက္ပါ။​​";
            }
            return RedirectToAction("OutProductPackingForm", "StockShop");
        }
        #endregion

        #region Out of Unit Product
        public ActionResult OutProductUnitForm()
        {
            return View();
        }
        public ActionResult ProcessOutProductUnitForm()
        {
            int ID = Convert.ToInt32(Request.Form["ID"]);
            int OutQuantity = Convert.ToInt32(Request.Form["Quantity"]);
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int TotalQuantity = stock.getIntByQuery("select * from Product where ID=" + ID, "Quantity");
            if (OutQuantity <= TotalQuantity)
            {
                stock.ChangeByQuery("update Product set Quantity=Quantity-" + OutQuantity + " where ID=" + ID + " and ShopID=" + ShopID);
                string Name = stock.getStringByQuery("select * from Product where ID=" + ID, "Name");
                int LeftQuantity = TotalQuantity - OutQuantity;
                Session["OutInfo"] = "["+Name + " ] " + OutQuantity + " (ခု) ထုတ္​ၿပီးပါၿပီ။ လက္​က်န္​မွာ " + LeftQuantity + " (ခု) ျဖစ္​သည္​​";
            }
            else
            {
                Session["OutInfo"] = "ပစၥည္​း လက္​က်န္​ မလံု​ေလာက္ပါ။​​";
            }
            return RedirectToAction("OutProductUnitForm", "StockShop");
        }
        #endregion

        #region Lack both Packing and Unit Product
        public ActionResult LackPackingProduct()
        {
            return View();
        }
        public ActionResult LackUnitProduct()
        {
            return View();
        }
        #endregion

        public ActionResult ShowAggreement()
        {
            return View();
        }

        #region See all packing product
        public ActionResult AllPackingProduct()
        {
            return View();
        }
        public ActionResult DeletePackingProduct()
        {
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Product where ID=" + ID + " and ShopID=" + ShopID);
            return RedirectToAction("AllPackingProduct", "StockShop");
        }
        #endregion

        #region See all Unit Product
        public ActionResult AllUnitProduct()
        {
            return View();
        }
        public ActionResult DeleteUnitProduct()
        {
            int ShopID = Convert.ToInt32(Session["CurrentUserID"]);
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            stock.ChangeByQuery("delete from Product where ID=" + ID + " and ShopID=" + ShopID);
            return RedirectToAction("AllUnitProduct", "StockShop");
        }
        #endregion

    }
}