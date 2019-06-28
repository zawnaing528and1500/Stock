using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventTicket.App_Code
{
    public class Node
    {
        #region Get Member List of Level 1, 2, 3 - Level 0 is total
        public Array GetTotalNodeList(int Parent, int level)
        {
            Tree t = new Tree();
            Array Result = new Array[0];
            if (level == 0)
            {
                if (t.CheckRootIsChilden(Parent))
                {
                    Result = t.GetTotalNode(Parent);
                }
            }
            else if (level == 1)
            {
                if (t.CheckRootIsChilden(Parent))
                {
                    Result = t.GetLevel1(Parent);
                }
            }
            else if (level == 2)
            {
                TreeLevel2 t1 = new TreeLevel2();
                if (t1.CheckRootIsChilden(Parent))
                {
                    Result = t1.GetLevel2(Parent);
                }
            }
            else if (level == 3)
            {
                TreeLevel3 t2 = new TreeLevel3();
                if (t.CheckRootIsChilden(Parent))
                {
                    Result = t2.GetLevel3(Parent);
                }
            }
            return Result;
        }
        #endregion


        //Level 0 is Total. Level 123
        public int GetTotalNode(int Parent, int level)
        {
            Tree t = new Tree();
            int result = 0;
            if (level == 0)
            {
                if (t.CheckRootIsChilden(Parent))
                {
                    Array Child = t.GetTotalNode(Parent);
                    result= Child.Length;
                }
            }
            else if(level == 1)
            {
                if (t.CheckRootIsChilden(Parent))
                {
                    Array Child = t.GetLevel1(Parent);
                    result = Child.Length;
                }
            }
            else if(level == 2)
            {
                TreeLevel2 t1 = new TreeLevel2();
                if (t1.CheckRootIsChilden(Parent))
                {
                    Array Child = t1.GetLevel2(Parent);
                    foreach (int i in Child)
                    {
                        result = Child.Length;
                    }
                }
            }
            else if (level == 3)
            {
                TreeLevel3 t2 = new TreeLevel3();
                if (t.CheckRootIsChilden(Parent))
                {
                    Array Child = t2.GetLevel3(Parent);
                    foreach (int i in Child)
                    {
                        result = Child.Length;
                    }
                }
            }
            else if (level == 123)
            {
                TreeLevel3 t2 = new TreeLevel3();
                if (t.CheckRootIsChilden(Parent))
                {
                    result = t2.GetLevel123TotalNode(Parent);
                }
            }
            return result;
        }
    }
}