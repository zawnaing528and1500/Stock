using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using EventTicket.App_Code;
using System.Data;
using System.Text;

namespace EventTicket.Controllers
{
    public class TestController : Controller
    {
        public object IO { get; private set; }
        DBBase db = new DBBase();
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

        public void DeleteFileFromFolder(string StrFilename)
        {

            string strPhysicalFolder = Server.MapPath("~/Image/Cover/");

            string strFileFullPath = strPhysicalFolder + StrFilename;

            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
        }

        public ActionResult DeleteImage()
        {
            DeleteFileFromFolder("IPMS Coventry  Warwickshire Show 2018.jpg");
            return View();
        }
        public ActionResult ProcessDataToExcel()
        {
            DataToExcel();
            return View();
        }
        private void DataToExcel()
        {
            DataTable dt = db.getAllByQuery("select ID,No,Date from HteNumber");//your datatable
            string attachment = "attachment; filename=download.xls";

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();

            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            string tab = "";
            foreach (DataColumn dc in dt.Columns)
            {
                Response.Write(tab + dc.ColumnName);
                tab = "\t";
            }
            Response.Write("\r\n");
            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    Response.Write(tab +  dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\r\n");
            }

            Response.Flush();
            Response.End();
        }
        public void GetTotalChildren()
        {
            Tree t = new Tree();
            if (t.CheckRootIsChilden(7))
            {
                Array Child = t.GetTotalNode(7);

                foreach (int i in Child)
                {
                    Debug.WriteLine("{0} ", i);
                }
            }
            else
            {
                Debug.WriteLine("Root has no children");
            }
        }
        public void GetLevel2Children()
        {
            TreeLevel2 t = new TreeLevel2();
            if (t.CheckRootIsChilden(7))
            {
                Array Child = t.GetLevel2(7);
                foreach (int i in Child)
                {
                    Debug.WriteLine("{0} ", i);
                }
            }
            else
            {
                Debug.WriteLine("Root has no children");
            }
        }
        public void GetLevel3Children()
        {
            TreeLevel3 t = new TreeLevel3();
            if (t.CheckRootIsChilden(7))
            {
                Array Child = t.GetLevel3(7);
                foreach (int i in Child)
                {
                    Debug.WriteLine("{0} ", i);
                }
            }
            else
            {
                Debug.WriteLine("Root has no children");
            }
        }
        public ActionResult TestWallet()
        {
            InitialTask i = new InitialTask();
            i.updateWalletBalance(42);
            return View();
        }
        public void SendEmail()
        {
            string email = "zawnaing528and1500@gmail.com";
            if (String.IsNullOrEmpty(email))
                return;
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress("dmgrouponlinehomejobprogram@gmail.com");
                mail.Subject = "sub";

                mail.Body = "I am Testing";

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential("dmgrouponlinehomejobprogram@gmail.com", "Myanmaritstar123"); // ***use valid credentials***
                smtp.Port = 587;

                //Or your Smtp Email ID and Password
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in Sending Mail");
            }
        }
    }
}