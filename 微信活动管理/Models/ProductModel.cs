using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 微信活动管理.Models
{
    public class ProductModel
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public DateTime create_time { get; set; }
        public string description { get; set; }
        public string pic_url { get; set; }
        public string product_type { get; set; }
        public string push_level { get; set; }
        public string content_url { get; set; }
        public string short_url { get; set; }
        public string ticket_url { get; set; }

    }
}