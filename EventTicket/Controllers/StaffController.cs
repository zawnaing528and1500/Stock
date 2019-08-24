using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Management;
using EventTicket.App_Code;
using System.Net.NetworkInformation;
using System.Diagnostics;
namespace EventTicket.Controllers
{
    public class StaffController : Controller
    {
        DBBase db = new DBBase();
        Tree t = new Tree();
        // GET: Staff

        #region All Functions Called by Actions
        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        #endregion

        #region Add Post : Image and Description
        public ActionResult AllPost()
        {
            return View();
        }
        public ActionResult PostDescription()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            ViewBag.ID = ID;
            return View();
        }
        public ActionResult DeletePost()
        {
            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            string Cover = db.getStringByQuery("select * from Post where ID=" + ID, "ImageName");
            DeleteCover(Cover);
            db.ChangeByQuery("delete from Post where ID=" + ID);
            return RedirectToAction("AllPost");
        }
        public void DeleteCover(string StrFilename)
        {
            string strPhysicalFolder = Server.MapPath("~/Image/Cover/");
            string strFileFullPath = strPhysicalFolder + StrFilename;
            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
        }
        public ActionResult AddPost()
        {
            return View();
        }
        public ActionResult ProcessAddPost(HttpPostedFileBase file)
        {
            string ImageName = "";
            if (file.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                ImageName = fileName;
                var path = System.IO.Path.Combine(Server.MapPath("~/Image/Cover"), fileName);
                file.SaveAs(path);
            }
            string Name = Request.Form["Name"];
            int CategoryID = Convert.ToInt32(Request.Form["CategoryID"]);
            int StaffID = Convert.ToInt32(Request.Form["StaffID"]);
            string Description = Request.Form["Description"];
            string Reference = Request.Form["Reference"];
            Description = Description.Replace("\'", string.Empty);
            db.ChangeByQuery("insert into Post values(N'"+Name+"',"+CategoryID+",N'"+Description+"',N'"+ImageName+"','"+ DateTime.Now.ToString("MM.dd.yyyy")+ "',"+StaffID+",0,'"+Reference+"')");
            return RedirectToAction("AddPost");
        }
        #endregion

        public ActionResult Dashboard()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            return View();
        }
        public ActionResult DeleteMember()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            int MemberID = Convert.ToInt32(Request.QueryString["ID"]);
            db.ChangeByQuery("delete from WithdrawHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Wallet where MemberID=" + MemberID);

            //Delete Node
            int Parent = db.getIntByQuery("select * from Tree where Child=" + MemberID, "Parent");
            if(Parent == 0) { Parent = 80; }
            db.ChangeByQuery("update Tree set Parent= "+Parent+" where Parent=" + MemberID);

            db.ChangeByQuery("delete from Tree where Parent=" + MemberID);
            db.ChangeByQuery("delete from Tree where Child=" + MemberID);

            db.ChangeByQuery("delete from TransferHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from RequestActiveDepositHistory where MemberID=" + MemberID);
            db.ChangeByQuery("delete from RequestActive where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Payment where MemberID=" + MemberID);
            db.ChangeByQuery("delete from MemberCaptchaEmail where MemberID=" + MemberID);
            db.ChangeByQuery("delete from MemberBank where MemberID=" + MemberID);
            db.ChangeByQuery("delete from Login where AllID=" + MemberID+" and AccessLevel = 2");
            db.ChangeByQuery("delete from Member where ID=" + MemberID);
            return RedirectToAction("ViewAllMembers");
        }
        public ActionResult ViewAllMembers()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }
            if (TempData["From"] != null)
            {
                ViewBag.From = TempData["From"].ToString();
                ViewBag.To = TempData["To"].ToString();
            }
            else
            {
                ViewBag.From = 0;
                ViewBag.To = 10;
            }
            return View();
        }
        public ActionResult ProcessViewAllMembers()
        {
            if (Session["CurrentUserID"] == null)
            {
                Response.Redirect("~/Account/LoginForm");
            }

            string From = Request.Form["From"];
            string To = Request.Form["To"];
            TempData["From"] = From;
            TempData["To"] = To;

            return RedirectToAction("ViewAllMembers", "Staff");
        }
        public string ChangeParent()
        {
            int MemberID = Convert.ToInt32(Request.QueryString["MemberID"]);
            int NewParentID = Convert.ToInt32(Request.QueryString["NewParentID"]);
            db.ChangeByQuery("update Tree set Parent="+NewParentID+" where Child="+MemberID);
            //int Parent = db.getIntByQuery("select * from Tree where Child=" + MemberID, "Parent");
            //if (Parent == 0) { Parent = 80; }
            //db.ChangeByQuery("update Tree set Parent= " + Parent + " where Parent=" + MemberID);
            return "Change ParentID of "+MemberID+" Successfully. New ParentID is "+NewParentID;
        }
        
    }
}