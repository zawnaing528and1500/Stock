using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventTicket.App_Code
{
    public class InitialTask
    {
        #region Member

        DBBase db = new DBBase();
        public void updateWalletBalance(int CurrentUserID)
        {
            int ActiveCount = 0, ChildID=0;
            TreeLevel3 t = new TreeLevel3();
            Array TotalChildList = t.GetLevel123TotalNodeList(CurrentUserID);
            //Convert Array Object to int Array
            int[] ChildList = TotalChildList.Cast<int>().ToArray();
            foreach(int i in ChildList)
            {
                ChildID = ChildList[i];
                if (db.CheckByQuery("select * from Member where ID=" + ChildID + " and Active='True'"))
                {
                    ActiveCount++;
                }
            }
            if (ActiveCount > 0)
            {
                int Amount = ActiveCount * db.getIntByQuery("select * from DollarRate where ID=1", "Rate");
                db.ChangeByQuery("update Wallet set Balance = Balance + " + Amount + " where MemberID=" + CurrentUserID);
            }
        }
        #endregion

    }
}