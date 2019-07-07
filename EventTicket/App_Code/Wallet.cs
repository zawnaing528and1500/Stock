using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventTicket.App_Code;

namespace EventTicket.App_Code
{
    public class Wallet
    {
        int ChildID = 0;int ActiveCount = 0;
        public void updateWalletBalance(int MemberID)
        {
            DBBase db = new DBBase();
            TreeLevel3 t = new TreeLevel3();
            Array TotalChildList = t.GetLevel123TotalNodeList(MemberID);
            //Convert Array Object to int Array
            int[] ChildList = TotalChildList.Cast<int>().ToArray();
            for(int i=0; i< ChildList.Length; i++)
            {
                ChildID = ChildList[i];
                //Check ChildID is Active
                if(db.CheckByQuery("select * from Member where ID="+ChildID+" and Active='True'"))
                {
                    ActiveCount++;
                }
            }
            if(ActiveCount> 0)
            {
                int Amount = ActiveCount * db.getIntByQuery("select * from DollarRate where ID=1", "Rate");
                db.ChangeByQuery("update Wallet set Balance = Balance + "+Amount+" where MemberID="+MemberID);
            }
        }
    }
}