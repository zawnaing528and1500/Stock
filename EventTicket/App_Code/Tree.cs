using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Net.Mail;
using System.Diagnostics;
namespace EventTicket.App_Code
{
    public class Tree
    {
        DBBase db = new DBBase();
        int[] Child = {}; int[] Level2 = { }; int[] Level3 = { }; int j;
       
        #region Check Root If it is Children
        public Boolean CheckRootIsChilden(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                j = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Check node if it is children
        public Boolean CheckIsChilden(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Add Child to Array
        public void AddChildrenToArray(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                foreach (DataRow rows in dtTree.Rows)
                {
                    int Node = Convert.ToInt32(rows["Child"]);
                    Child = Child.Concat(new int[] { Node }).ToArray();
                }
            }
        }
        #endregion

        #region Add Total Children Array
        public void AddAllToChildList()
        {
            if (j < Child.Length)
            {
                if (CheckIsChilden(Child[j]))
                {
                    AddChildrenToArray(Child[j]);
                }
                j++;
                AddAllToChildList();
            }
        }
        #endregion

        public int GetTotalChildren()
        {
            return j;
        }
        public Array GetTotalNode(int Parent)
        {
            AddChildrenToArray(Parent);
            AddAllToChildList();
            return Child;
        }
        public Array GetLevel1(int Parent)
        {
            AddChildrenToArray(Parent);
            return Child;
        }

        #region Wallet Section

        public void updateWalletBalance(int CurrentUserID)
        {
            int ActiveCount = 0, ChildID = 0;int Period = 0;
            int day = DateTime.Now.Day;
            if (day > 14 || day < 29)
            {
                Period = 1;
            }
            if (day == 30 || day == 31 || day < 14)
            {
                Period = 2;
            }
            TreeLevel3 t = new TreeLevel3();
            Array TotalChildList = t.GetLevel123TotalNodeList(CurrentUserID);
            //Convert Array Object to int Array
            int[] ChildList = TotalChildList.Cast<int>().ToArray();
            for(int i=0; i< ChildList.Length; i++)
            {
                ChildID = ChildList[i];
                if (db.CheckByQuery("select * from Member where ID=" + ChildID + " and Active='True'") && !(db.CheckByQuery("select * from Payment where MemberID="+CurrentUserID+" and Child="+ChildID+" and Period="+Period)))
                {
                    db.ChangeByQuery("insert into Payment values("+CurrentUserID+","+ChildID+","+Period+")");
                    ActiveCount++;
                }
            }
            if (ActiveCount > 0)
            {
                int Amount = ActiveCount * db.getIntByQuery("select * from DollarRate where ID=1", "Rate");
                db.ChangeByQuery("update Wallet set Balance = Balance + " + Amount + " where MemberID=" + CurrentUserID);
            }
            if (day == 14 || day == 29)
            {
                db.ChangeByQuery("delete From Payment");
            }
        }
        public void TransferMoney(string OwnWallet,string WalletToTransfer,int AmountToTransfer)
        {
            db.ChangeByQuery("update Wallet set Balance=Balance-"+AmountToTransfer+" where WalletNumber=N'"+OwnWallet+"'");
            db.ChangeByQuery("update Wallet set Balance=Balance+" + AmountToTransfer + " where WalletNumber=N'" + WalletToTransfer + "'");
        }

        #endregion

        #region Sending Email
        public void SendEmail(string Subject, string Body, string To)
        {
            if (String.IsNullOrEmpty(To))
                return;
            try
            {
                //dmgrouponlinehomejobprogram@gmail.com, Myanmaritstar123
                //dmgrouponlinehomejobsprogram@gmail.com, *Dmgroup* 
                MailMessage mail = new MailMessage();
                mail.To.Add(To);
                mail.From = new MailAddress("myanmaritstar.homejob@gmail.com");
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new System.Net.NetworkCredential("myanmaritstar.com@gmail.com", "Zawnaing12"); // ***use valid credentials***
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in Sending Mail");
            }
        }
        #endregion

        #region Inactive Count
        public void InactiveCount()
        {
            DataTable Member = db.getAllByQuery("select * from Member where Active='False' and ID!=80 and ID!=1");
            foreach(DataRow row in Member.Rows)
            {
                int MemberID = Convert.ToInt32(row["ID"]);

                int InActiveCount = db.getCountByQuery("select * from InactiveCount where MemberID=" + MemberID);
                if(InActiveCount == 2)
                {
                    int Parent = db.getIntByQuery("select * from Tree where Child=" + MemberID, "Parent");
                    if (Parent == 0) { Parent = 80; }
                    db.ChangeByQuery("update Tree set Parent= " + Parent + " where Parent=" + MemberID);

                    db.ChangeByQuery("delete from Tree where Parent=" + MemberID);
                    db.ChangeByQuery("delete from Tree where Child=" + MemberID);

                    db.ChangeByQuery("delete from InactiveCount where MemberID="+MemberID);
                    db.ChangeByQuery("delete from TransferHistory where SenderID=" + MemberID+" or ReceiverID="+MemberID);
                    db.ChangeByQuery("delete from RequestActiveDepositHistory where MemberID=" + MemberID);
                    db.ChangeByQuery("delete from RequestActive where MemberID=" + MemberID);
                    db.ChangeByQuery("delete from Payment where MemberID=" + MemberID);
                    db.ChangeByQuery("delete from MemberCaptchaEmail where MemberID=" + MemberID);
                    db.ChangeByQuery("delete from MemberBank where MemberID=" + MemberID);
                    db.ChangeByQuery("delete from Login where AllID=" + MemberID + " and AccessLevel = 2");
                    db.ChangeByQuery("delete from Member where ID=" + MemberID);
                }
                else
                {
                    db.ChangeByQuery("insert into InactiveCount values("+MemberID+")");
                }
            }
        }
        #endregion
    }
}