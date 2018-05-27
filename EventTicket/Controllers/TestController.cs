using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using EventTicket.App_Code;

namespace EventTicket.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PrintCharacter()
        {
            int unicode = 65;
            char character = (char)unicode;
            string text = character.ToString();
            Debug.WriteLine(text);
            return View();
        }
        public ActionResult SendSeatInfo()
        {
            MyWebRequest myRequest = new MyWebRequest("http://smsgateway.me/api/v3/messages/send", "POST", "email=inspireeventticket@gmail.com&password=companyproject&device=83001&number=+959257706564&message=Ko Sai! I love you very much!");
            //show the response string on the console screen.
            ViewData["status"] = myRequest.GetResponse();
            return View();
        }
    }
}