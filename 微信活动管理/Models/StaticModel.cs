using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 微信活动管理.Models
{
    public class StaticModel
    {
        public static List<int> createStaticData()
        {
            List<int> c_data = new List<int>();
            c_data.Add(1);
            c_data.Add(6);
            c_data.Add(4);
            c_data.Add(3);
            return c_data;
        }
    }
}