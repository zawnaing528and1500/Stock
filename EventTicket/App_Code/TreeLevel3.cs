using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace EventTicket.App_Code
{
    public class TreeLevel3
    {
        DBBase db = new DBBase();
        int[] Child = { }; int[] Level2 = { }; int[] Level3 = { }; int j,i;

        #region Check Root If it is Children
        public Boolean CheckRootIsChilden(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                return true;
                j = 0;i = 0;
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

        #region Add Children to Child Array
        public void AddChildrenToChildArray(int Parent)
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

        #region Add Children to Level2 Array-Repeat Function
        public void AddChildrenToLevel2Array(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                foreach (DataRow rows in dtTree.Rows)
                {
                    int Node = Convert.ToInt32(rows["Child"]);
                    Level2 = Level2.Concat(new int[] { Node }).ToArray();
                }
            }
        }
        #endregion
        #region Add Children to Level3 Array-Repeat Function
        public void AddChildrenToLevel3Array(int Parent)
        {
            DataTable dtTree = db.getAllByQuery("select * from Tree where Parent=" + Parent);
            if (dtTree != null)
            {
                foreach (DataRow rows in dtTree.Rows)
                {
                    int Node = Convert.ToInt32(rows["Child"]);
                    Level3 = Level3.Concat(new int[] { Node }).ToArray();
                }
            }
        }
        #endregion
        #region Add all Children Level2 Array
        public void AddToLevel2ChildList()
        {
            if (j < Child.Length)
            {
                if (CheckIsChilden(Child[j]))
                {
                    AddChildrenToLevel2Array(Child[j]);
                }
                j++;
                AddToLevel2ChildList();
            }
        }
        #endregion

        #region Add all Children Level3 Array
        public void AddToLevel3ChildList()
        {
            if (i < Level2.Length)
            {
                if (CheckIsChilden(Level2[i]))
                {
                    AddChildrenToLevel3Array(Level2[i]);
                }
                i++;
                AddToLevel3ChildList();
            }
        }
        #endregion

        public Array GetLevel2(int Parent)
        {
            AddChildrenToChildArray(Parent);
            AddToLevel2ChildList();
            return Level2;
        }
        public Array GetLevel3(int Parent)
        {
            //Level2
            AddChildrenToChildArray(Parent);
            AddToLevel2ChildList();

            AddToLevel3ChildList();
            return Level3;
        }
        public int GetLevel123TotalNode(int Parent)
        {
            GetLevel3(Parent);
            return Child.Length + Level2.Length + Level3.Length;
        }
    }
}