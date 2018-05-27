using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventTicket.App_Code
{
    public class Row
    {
        DBBase d = new DBBase();
        public void set(int EID, int Row)
        {
            for(int i=1; i<= Row; i++)
            {
                d.ChangeByQuery("insert into Row values("+EID+","+i+","+0+",'Not Set',"+0+")");
            }
        }
    }
}