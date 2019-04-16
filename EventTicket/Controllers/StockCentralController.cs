using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventTicket.Controllers
{
    public class StockCentralController : Controller
    {
        // GET: StockCentral
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowBranch()
        {
            return View();
        }
        public ActionResult ShowLackPackingProduct()
        {
            return View();
        }
        public ActionResult ShowExpiryPackingProduct()
        {
            return View();
        }
    }
}