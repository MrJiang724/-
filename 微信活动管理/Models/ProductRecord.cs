using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 微信活动管理.Models
{
    public class ProductRecord
    {

    }

    public class RecordData
    {
        public int weekCount { get; set; }
        public int monthCount { get; set; }
        public int totalCount { get; set; }
    }

    public class ProductRecordData
    {
        public RecordData weixinReport { get; set; }
        public RecordData advertReport { get; set; }
    }

    public class ProductRecordJsonMessage
    {
        public ProductRecordData data { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class AdvMachineRecordData
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int todayClickTime { get; set; }
        public int weekClickTime { get; set; }
        public int monthClickTime { get; set; }
        public int totalClickTime { get; set; }
        public int todayScanTime { get; set; }
        public int weekScanTime { get; set; }
        public int monthScanTime { get; set; }
        public int totalScanTime { get; set; }
    }

    public class AdvMachineRecordJsonMessage
    {
        public List<AdvMachineRecordData> data { get; set; }
        public string errmsg { get; set; }
        public int errcode { get; set; }
    }
}