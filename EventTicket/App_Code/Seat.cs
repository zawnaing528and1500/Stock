using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventTicket.App_Code
{
    public class Seat
    {
        int TotalTicket; int EID;
        string Name;
        int Row; int Price;
        string Status="Not Set";
        int Character = 65;

        public void setTotalTicket(int totalTicket, int EventID)
        {
            TotalTicket = totalTicket;
            EID = EventID;
        }
        public void setFirstTimeSeat()
        {
            int NoOfRow = TotalTicket / 10;
            int ExtraInsert = TotalTicket % 10;
            DBBase d = new DBBase();
            for(int i=1; i <= NoOfRow; i++)
            {
                for(int j=1; j <= 10; j++)
                {
                    char character = (char)Character;
                    string text = character.ToString();
                    Name = text + j.ToString();//A1,A2,A3,A4,..
                    Row = i;
                    Price = 5000;
                    d.ChangeByQuery("insert into Seat values("+EID+",'"+Name+"',"+Row+","+Price+",'"+Status+"')");
                }
                Character = Character + 1;//Increase A to B
            }
            for(int i=1; i <= ExtraInsert; i++)
            {
                char character = (char)Character;
                string text = character.ToString();
                //Current row is NoOfRow+1
                Name = text + i.ToString();
                Row = NoOfRow + 1;
                Price = 5000;
                //Status is not set
                d.ChangeByQuery("insert into Seat values(" + EID + ",'" + Name + "'," + Row + "," + Price + ",'" + Status + "')");
            }
        }
    }
}