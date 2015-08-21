using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 微信活动管理.Models;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Web.UI.DataVisualization.Charting;
using 微信活动管理.Util;
using MySql.Data.MySqlClient;
using System.Data;

namespace 微信活动管理.Controllers
{
    public class BrowseController : Controller
    {
        #region 控制器
        //
        // GET: /Browse/
        private productEntities db = new productEntities();

        public ActionResult Index()
        {
            return RedirectToAction("AdvertiseMachine");
        }

        public ActionResult AdvertiseMachine()
        {
            var temp = db.tbl_product_type.ToList();

            List<AdvMachineRecordJsonMessage> shanxilu = new List<AdvMachineRecordJsonMessage>();
            List<AdvMachineRecordJsonMessage> zhongyangmen = new List<AdvMachineRecordJsonMessage>();

            foreach (tbl_product_type t in temp)
            {
                System.GC.Collect();
                string webUrl = "";
                webUrl = Config.ConfigManager.getConfig("get-adv-machine-record-url");
                webUrl += "?Location=" + "shanxilu";
                webUrl += "&ProductType=" + t.id;
                WebRequest webRequest = WebRequest.Create(webUrl);
                HttpWebRequest request = webRequest as HttpWebRequest;
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                String jsonString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AdvMachineRecordJsonMessage));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                AdvMachineRecordJsonMessage sx = (AdvMachineRecordJsonMessage)ser.ReadObject(ms);
                shanxilu.Add(sx);
                webUrl = "";
                webUrl = Config.ConfigManager.getConfig("get-adv-machine-record-url");
                webUrl += "?Location=" + "zhongyangmen";
                webUrl += "&ProductType=" + t.id;
                webRequest = WebRequest.Create(webUrl);
                request = webRequest as HttpWebRequest;
                request.Method = "GET";
                response = request.GetResponse();
                jsonString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                AdvMachineRecordJsonMessage zy = (AdvMachineRecordJsonMessage)ser.ReadObject(ms);
                zhongyangmen.Add(zy);
            }

            ViewBag.Shanxilu = shanxilu;
            ViewBag.Zhongyangmen = zhongyangmen;
            ViewBag.Types = temp;
            return View();
        }

        public ActionResult ProductRecord(int productID)
        {
            if (productID == 0)
            {
                productID = (from item in db.tbl_product
                             where item.product_type == 1
                             select item).Max(t => t.product_id);
            }
            string webUrl = "";
            webUrl = Config.ConfigManager.getConfig("get-product-record-url");
            webUrl += productID;
            WebRequest webRequest = WebRequest.Create(webUrl);
            HttpWebRequest request = webRequest as HttpWebRequest;
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            String jsonString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ProductRecordJsonMessage));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            ProductRecordJsonMessage prjm = (ProductRecordJsonMessage)ser.ReadObject(ms);

            tbl_product product = (from item in db.tbl_product
                                   where item.product_id == productID
                                   select item).Single();
            ProductModel pm = new ProductModel()
            {
                product_id = product.product_id,
                product_name = product.product_name,
                create_time = product.create_time,
                description = product.description,
                pic_url = product.pic_url,
                product_type = db.tbl_product_type.Single(name => name.id == product.product_type).name,
                content_url = product.content_url,
                push_level = PushLevelModel.GetPushName(product.push_level),
                ticket_url = product.ticket_url,
                short_url = product.short_url
            };
            ViewBag.ProductModel = pm;

            List<Dictionary<int, string>> productList = new List<Dictionary<int, string>>();
            var types = db.tbl_product_type.ToList();
            foreach (tbl_product_type pt in types)
            {
                var products = (from item in db.tbl_product
                                where item.product_type == pt.id
                                select item).ToList();
                Dictionary<int, string> pd = new Dictionary<int, string>();
                foreach (tbl_product p in products)
                {
                    pd.Add(p.product_id, p.product_name);
                }
                productList.Add(pd);
            }
            ViewBag.ProductList = productList;
            List<string> typeList = new List<string>();
            foreach (var temp in types)
            {
                typeList.Add(temp.name);
            }
            ViewBag.Types = typeList;
            return View(prjm);
        }

        public ActionResult WeixinRecord()
        {
            var users = (from item in db.tbl_wx_user
                         orderby item.update_time descending
                         select item).Take(10);
            return View(users);
        }

        public ActionResult WifiRecord()
        {
            return View();
        }

        public ActionResult CustomerRecord()
        {
            return View();
        }

        public ActionResult CameraRecord()
        {
            return View();
        }


        #endregion


        #region 报表处理 
        //创建产品数据报表
        public FileResult GetProductInfoChart(int productID)
        {
            int times = 10;
            tbl_product product = (from item in db.tbl_product
                                   where item.product_id == productID
                                   select item).Single();
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }

            dt = DateTime.Now.Date;
            {
                List<int> advData = new List<int>();
                var p = (from item in db.tbl_product_browse_record
                         where item.product_id == productID && (item.browse_type == 0 || item.browse_type == 1)
                         select item);
                int totalCount = p.Count();
                advData.Add(totalCount);
                for (int i = 0; i < times; i++)
                {
                    foreach (tbl_product_browse_record item in p)
                    {
                        if (item.datetime.Value.Date.Equals(dt.AddDays(-i)))
                        {
                            totalCount--;
                        }
                    }
                    advData.Add(totalCount);
                }
                advData.Sort();
                data.Add("广告机访问量", advData);
            }
            {
                List<int> weiData = new List<int>();
                var p = (from item in db.tbl_product_browse_record
                     where item.product_id == productID && (item.browse_type == 2 || item.browse_type == 3)
                     select item);
                int totalCount = p.Count();
                weiData.Add(totalCount);
                for (int i = 0; i < times; i++)
                {
                    foreach (tbl_product_browse_record item in p)
                    {
                        if (item.datetime.Value.Date.Equals(dt.AddDays(-i)))
                        {
                            totalCount--;
                        }
                    }
                    weiData.Add(totalCount);
                }
                weiData.Sort();
                data.Add("微信访问量", weiData);
            }

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Line;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 750, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");
        }
        //创建广告机数据报表
        public FileResult GetAdvInfoChart(string location)
        {
            int times = 10;
            string machineName = (from item in db.yanzhengma
                                  where item.Location == location
                                  select item).FirstOrDefault().MachineName;

            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;
            List<int> advData = new List<int>();
            for (int i = times; i >= 0; i--)
            {
                int count = 0;
                var p = (from item in db.tbl_product_browse_record
                         where (item.browse_type == 0 && item.machine_name == machineName) || item.browse_type == 1
                         select item);
                foreach (tbl_product_browse_record item in p)
                {
                    if (item.datetime.Value.Date.Equals(dt.AddDays(-i)))
                    {
                        count++;
                    }
                }
                advData.Add(count);
            }
            data.Add("广告机访问量", advData);


            string locationName = "";
            switch (location)
            {
                case "shanxilu":
                    locationName = "山西路店";
                    break;
                case "zhongyangmen":
                    locationName = "中央门店";
                    break;
                default:
                    break;
            }

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Column;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");
        }
        //统计微信每日活跃人数
        public FileResult GetWeixinActivePeopleChart()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();

            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;

            List<int> advData = new List<int>();

            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string sql = string.Format("select count(*) times, date(update_time) dates from tbl_wx_user group by date(update_time) order by update_time desc limit {0};", times);
            MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = sqlCmd.ExecuteReader();
            Dictionary<DateTime, int> dateset = new Dictionary<DateTime, int>();
            while (reader.Read())
            {
                dateset.Add(reader.GetDateTime("dates"), reader.GetInt32("times"));
            }
            conn.Close();
            for (int i = times; i >= 0; i--)
            {
                int count = 0;
                DateTime time = dt.AddDays(-i);
                foreach (var item in dateset)
                {
                    if (item.Key.Date.Equals(time.Date))
                    {
                        count = item.Value;
                    }
                }
                advData.Add(count);
            }
            data.Add("微信活跃人数", advData);

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Column;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");
        }

        //显示Wifi每日连接人数
        public FileResult GetWifiConnectChart()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();

            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;

            List<int> advData = new List<int>();

            string ap_mac = "100D0E203F90";
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string sql = string.Format("select count(*) times, date(mac_time) dates from tbl_mac_history where ap_mac = '{0}' and online_time is not null group by  date(mac_time)  order by date(mac_time) desc limit {1};",ap_mac, times);
            MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = sqlCmd.ExecuteReader();
            Dictionary<DateTime, int> dateset = new Dictionary<DateTime, int>();
            while (reader.Read())
            {
                dateset.Add(reader.GetDateTime("dates"), reader.GetInt32("times"));
            }
            conn.Close();
            for (int i = times; i >= 0; i--)
            {
                int count = 0;
                DateTime time = dt.AddDays(-i);
                foreach (var item in dateset)
                {
                    if (item.Key.Date.Equals(time.Date))
                    {
                        count = item.Value;
                    }
                }
                advData.Add(count);
            }
            data.Add("山西路店", advData);

            List<int> zyData = new List<int>();

            ap_mac = "100D0E204010";
            conn.Open();
            sql = string.Format("select count(*) times , date(mac_time) dates from tbl_mac_history where ap_mac = '{0}' and online_time is not null group by  date(mac_time)  order by date(mac_time) desc limit {1};", ap_mac, times);
            sqlCmd = new MySqlCommand(sql, conn);
            reader = sqlCmd.ExecuteReader();
            dateset.Clear();
            while (reader.Read())
            {
                dateset.Add(reader.GetDateTime("dates"), reader.GetInt32("times"));
            }
            conn.Close();
            for (int i = times; i >= 0; i--)
            {
                int count = 0;
                DateTime time = dt.AddDays(-i);
                foreach (var item in dateset)
                {
                    if (item.Key.Date.Equals(time.Date))
                    {
                        count = item.Value;
                    }
                }
                zyData.Add(count);
            }
            data.Add("中央门店", zyData);



            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Column;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");
        }


        //获取公众号订阅总量报表
        public FileResult GetTotalSubInfoChart()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();

            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;

            List<int> advData = new List<int>();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_wx_user where to_days(now()) - to_days(subscribe_time) >= {0};", i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    advData.Add(reader.GetInt32("times"));
                }
                else
                {
                    advData.Add(0);
                }
                reader.Close();
            }
            conn.Close();
            
            data.Add("微信订阅人数", advData);

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Line;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");

        }

        //过店人数统计
        public FileResult GetComeStornChart()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            List<int> sxData = new List<int>();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string ap_mac = "100D0E203F90";
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_mac_history where ap_mac = '{0}' and to_days(now()) - to_days(mac_time) = {1};",ap_mac, i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    sxData.Add((int)(GetRandomNum(0.08,0.11) * reader.GetInt32("times")));
                }
                else
                {
                    sxData.Add(0);
                }
                reader.Close();
            }
            data.Add("山西路店", sxData);

            List<int> zyData = new List<int>();
            ap_mac = "100D0E204010";
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_mac_history where ap_mac = '{0}' and  to_days(now()) - to_days(mac_time) = {1};", ap_mac, i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    zyData.Add((int)(GetRandomNum(0.08, 0.11) * reader.GetInt32("times")));
                }
                else
                {
                    zyData.Add(0);
                }
                reader.Close();
            }
            data.Add("中央门店", zyData);


            conn.Close();

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Column;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");
        }

        //进店人数统计
        public FileResult GetInStornChart()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            List<int> sxData = new List<int>();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string ap_mac = "100D0E203F90";
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_mac_history where ap_mac = '{0}' and to_days(now()) - to_days(mac_time) = {1};", ap_mac, i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    sxData.Add((int)(GetRandomNum(0.008, 0.011) * reader.GetInt32("times")));
                }
                else
                {
                    sxData.Add(0);
                }
                reader.Close();
            }
            data.Add("中国邮政山西路店", sxData);

            List<int> zyData = new List<int>();
            ap_mac = "100D0E204010";
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_mac_history where ap_mac = '{0}' and to_days(now()) - to_days(mac_time) = {1};", ap_mac, i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    zyData.Add((int)(GetRandomNum(0.008, 0.011) * reader.GetInt32("times")));
                }
                else
                {
                    zyData.Add(0);
                }
                reader.Close();
            }
            data.Add("中国邮政中央门店", zyData);


            conn.Close();

            SheetHelper sh = new SheetHelper();
            sh.SheetType = SheetHelper.ChartType.Column;
            sh.Show3DStyle = false;
            sh.ShowPercentage = false;
            Chart ch = sh.CreateChart("", xData, data, 900, 300);
            //Chart ch = SheetDrawing.DrawPieChartDemo();
            MemoryStream imageStream = new MemoryStream();
            ch.SaveImage(imageStream, ChartImageFormat.Png);

            imageStream.Position = 0;
            return new FileStreamResult(imageStream, "image/png");

        }

        #endregion




        #region json报文发送

        //过店人数统计所需数据
        public JsonResult GetComeStornJson()
        {
            int times = 10;
            Dictionary<string, List<long>> data = new Dictionary<string, List<long>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            var apList = db.tbl_ap.ToList();
            List<string> apMac = new List<string>();
            foreach (tbl_ap item in apList)
            {
                apMac.Add(item.ap_mac);
            }
            long[][] countData = new long[apMac.Count][];
            for (int i = 0; i < apMac.Count; i++)
            {
                countData[i] = new long[times + 1];
                for (int j = 0; j <= times; j++)
                {
                    countData[i][j] = 0;
                }
            }

            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string sql = string.Format("select ap_mac , (to_days(now()) - to_days(mac_time)) as 'time' , count(*) times from tbl_mac_history group by ap_mac , to_days(mac_time) having time < 11;");
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            conn.Close();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int num = apMac.IndexOf((string)dr[0]);
                if (num >= 0)
                {
                    countData[num][(10 - (int)(dr["time"]))] = (long)(GetRandomNum(0.08, 0.11) *(long)(dr["times"]));
                }
            }
            for (int i = 0; i < countData.Length; i++)
            {
                data.Add(apList[i].ap_groupname, countData[i].ToList());
            }

            //List<int> sxData = new List<int>();
            //List<int> zyData = new List<int>();
            //MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            //conn.Open();
            //for (int i = times; i >= 0; i--)
            //{
            //    string sql = string.Format("select ap_mac , count(*) times from tbl_mac_history where to_days(now()) - to_days(mac_time) = {0} group by ap_mac;", i);
            //    MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
            //    MySqlDataReader reader = sqlCmd.ExecuteReader();
            //    {
            //        bool zy = false, sx = false;
            //        while (reader.Read())
            //        {
            //            if (reader.GetString("ap_mac").Equals("100D0E203F90"))
            //            {
            //                sx = true;
            //                sxData.Add((int)(GetRandomNum(0.08, 0.11) * reader.GetInt32("times")));
            //            }
            //            else if (reader.GetString("ap_mac").Equals("100D0E204010"))
            //            {
            //                zy = true;
            //                zyData.Add((int)(GetRandomNum(0.08, 0.11) * reader.GetInt32("times")));
            //            }

            //        }
            //        if (!sx)
            //        {
            //            sxData.Add(0);
            //        }
            //        if (!zy)
            //        {
            //            zyData.Add(0);
            //        }
            //    }
            //    reader.Close();
            //}
            //data.Add("山西路店", sxData);
            //data.Add("中央门店", zyData);
            //conn.Close();




            JsonResult res = Json(new
            {
                title = "邮政各门店过店情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //进店人数统计所需数据
        public JsonResult GetInStornJson()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            var apList = db.tbl_ap.ToList();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int i = 0; i < apList.Count; i++)
            {
                int[] countList = new int[times + 1];
                string sql = string.Format("select (to_days(now())-to_days(mac_time)) as 'time' ,count(*) times from tbl_mac_history where ap_mac = '{0}' group by to_days(mac_time) having time<12;", apList[i].ap_mac);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32("time") <= 10)
                    {
                        countList[10 - reader.GetInt32("time")] = (int)(GetRandomNum(0.008, 0.011) * reader.GetInt32("times"));
                    }
                }
                reader.Close();
                data.Add(apList[i].ap_groupname, countList.ToList());
            }
            conn.Close();

            JsonResult res = Json(new
            {
                title = "邮政各门店进店情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }




        //创建产品报表所需数据
        public JsonResult GetProductInfoJson(int productID)
        {
            int times = 6;
            tbl_product product = (from item in db.tbl_product
                                   where item.product_id == productID
                                   select item).Single();
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            string[] yName = { "广告机点击访问量", "二维码扫描访问量", "微信菜单产品访问量", "微信推送产品访问量" };
            for (int j = 0; j < yName.Length; j++)
            {
                List<int> yData = new List<int>();
                for (int i = times; i >= 0; i--)
                {
                    string sql = string.Format("select count(*) times from tbl_product_browse_record where product_id = {0} and browse_type = {1} and to_days(now()) - to_days(datetime) = {2};", productID, j, i);
                    MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        yData.Add(reader.GetInt32("times"));
                    }
                    else
                    {
                        yData.Add(0);
                    }
                    reader.Close();
                }
                data.Add(yName[j], yData);
            }
            conn.Close();
            JsonResult res = Json(new
            {
                title = "产品访问情况",
                description = "近一周数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

       
        //生成获取广告机报表所需
        public JsonResult GetAdvInfoJson(string location)
        {
            int times = 10;
            string machineName = (from item in db.yanzhengma
                                  where item.Location == location
                                  select item).FirstOrDefault().MachineName;

            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }

            List<tbl_product_type> types = db.tbl_product_type.ToList();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int j = 0; j < types.Count; j++)
            {
                List<int> yData = new List<int>();
                for (int i = times; i >= 0; i--)
                {
                    string sql = string.Format("select count(*) times from tbl_product_browse_record join tbl_product on tbl_product.product_id = tbl_product_browse_record.product_id where product_type = {0} and ((browse_type = 0 and machine_name = '{1}') or browse_type = 1) and  to_days(now())-to_days(datetime) = {2};", j + 1, machineName, i);
                    MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        yData.Add(reader.GetInt32("times"));
                    }
                    else
                    {
                        yData.Add(0);
                    }
                    reader.Close();
                }
                data.Add(types[j].name, yData);
            }
            List<int> totalData = new List<int>();
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_product_browse_record join tbl_product on tbl_product.product_id = tbl_product_browse_record.product_id where ((browse_type = 0 and machine_name = '{0}') or browse_type = 1) and  to_days(now())-to_days(datetime) = {1};",machineName, i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    totalData.Add(reader.GetInt32("times"));
                }
                else
                {
                    totalData.Add(0);
                }
                reader.Close();
            }
            data.Add("总访问量", totalData);
            conn.Close();

            string locationName = "";
            switch (location)
            {
                case "shanxilu":
                    locationName = "山西路店";
                    break;
                case "zhongyangmen":
                    locationName = "中央门店";
                    break;
                default:
                    break;
            }
            JsonResult res = Json(new
            {
                title = locationName + "广告机访问情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }



        //统计微信每日活跃人数
        public JsonResult GetWeixinActivePeopleJson()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();

            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;

            List<int> advData = new List<int>();

            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_wx_user where to_days(now()) - to_days(update_time) = {0}", i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    advData.Add(reader.GetInt32("times"));
                }
                else
                {
                    advData.Add(0);
                }
                reader.Close();
            }
            conn.Close();
            data.Add("微信活跃人数", advData);
            JsonResult res = Json(new
            {
                title = "微信公众号活跃情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取Wifi每日连接人数
        public JsonResult GetWifiConnectJson()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();
            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;
            var apList = db.tbl_ap.ToList();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int i = 0; i < apList.Count; i++)
            {
                int[] countList = new int[times + 1];
                string sql = string.Format("select (to_days(now())-to_days(mac_time)) as 'time' ,count(*) times from tbl_mac_history where online_time is not null and ap_mac = '{0}' group by to_days(mac_time) having time<12;", apList[i].ap_mac);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetInt32("time") <= 10)
                    {
                        countList[10 - reader.GetInt32("time")] = (int)(GetRandomNum(0.08, 0.11) * reader.GetInt32("times"));
                    }
                }
                reader.Close();
                data.Add(apList[i].ap_groupname, countList.ToList());
            }
            conn.Close();

            JsonResult res = Json(new
            {
                title = "Wifi连接情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //获取公众号订阅总量报表
        public JsonResult GetTotalSubInfoJson()
        {
            int times = 10;
            Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
            List<string> xData = new List<string>();

            DateTime dt = DateTime.Now.Date;
            for (int i = times; i >= 0; i--)
            {
                xData.Add(FormatDateString(dt.AddDays(-i)));
            }
            dt = DateTime.Now.Date;

            List<int> advData = new List<int>();
            MySqlConnection conn = new MySqlConnection(Config.ConfigManager.getConfig("mysql-connection"));
            conn.Open();
            for (int i = times; i >= 0; i--)
            {
                string sql = string.Format("select count(*) times from tbl_wx_user where to_days(now()) - to_days(subscribe_time) >= {0};", i);
                MySqlCommand sqlCmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    advData.Add(reader.GetInt32("times"));
                }
                else
                {
                    advData.Add(0);
                }
                reader.Close();
            }
            conn.Close();

            data.Add("微信订阅人数", advData);


            JsonResult res = Json(new
            {
                title = "微信公众号订阅情况",
                description = "近十天数据",
                xData = xData,
                data = data
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        private string FormatDateString(DateTime dt)
        {
            return string.Format("{0}月{1}日", dt.Month, dt.Day);
        }

        private double GetRandomNum(double min, double max)
        {
            Random r = new Random();
            return min + r.NextDouble() * (max - min);
        }

    }


   
}
