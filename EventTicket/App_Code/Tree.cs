using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
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
    }
}