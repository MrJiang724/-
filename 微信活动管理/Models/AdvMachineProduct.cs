using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 微信活动管理.Models
{
    public class AdvMachineProduct
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string location { get; set; }
        public string adv_location { get; set; }
    }
}