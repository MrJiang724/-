using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 微信活动管理.Models;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace 微信活动管理.Controllers
{ 
    public class ProductController : Controller
    {
        private productEntities db = new productEntities();

        //
        // GET: /Product/

        public ActionResult Index(int id = 0)
        {
            List<tbl_product> products = null;
            if (id == 0)
            {
                products = (from pro in db.tbl_product
                            where pro.product_status == true
                            select pro).ToList();
            }
            else
            {
                products = (from pro in db.tbl_product
                            where pro.product_type == id &&　pro.product_status == true
                            select pro).ToList();
            }
            List<ProductModel> p = new List<ProductModel>();
            for (int i = products.Count - 1; i >= 0;i--)
            {
                tbl_product item = products[i];
                string type = db.tbl_product_type.Single(name => name.id == item.product_type).name;
                ViewBag.types = db.tbl_product_type.ToList();

                ProductModel pm = new ProductModel()
                {
                    product_id = products[i].product_id,
                    product_name = products[i].product_name,
                    create_time = products[i].create_time,
                    description = products[i].description,
                    pic_url = products[i].pic_url,
                    product_type = type,
                    content_url = products[i].content_url,
                    push_level = PushLevelModel.GetPushName(products[i].push_level),
                    ticket_url = products[i].ticket_url,
                    short_url = products[i].short_url
                };
                p.Add(pm);
            }
            return View(p);
        }

        //
        // GET: /Product/Details/5

        public ViewResult Details(int id)
        {
            tbl_product tbl_product = db.tbl_product.Single(t => t.product_id == id);
            return View(tbl_product);
        }

        //
        // GET: /Product/Create

        public ActionResult Create()
        {
            ViewBag.types = new SelectList( db.tbl_product_type.ToList(),"id","name");
            ViewBag.push_levels = new SelectList(PushLevelModel.GetPushLevelList(), "level", "name");
            return View();
        } 

        //
        // POST: /Product/Create

        [HttpPost]
        public ActionResult Create(tbl_product tbl_product)
        {
            if (ModelState.IsValid)
            {
                tbl_product.product_status = true;
                tbl_product.create_time = DateTime.Now;
                //处理长链接为短连接 获取二维码 并存储
                string webUrl = "";

                webUrl = Config.ConfigManager.getConfig("get-ticket-url");
                webUrl = webUrl.Replace("EVENT_ID", "" + tbl_product.product_id);
                WebRequest webRequest = WebRequest.Create(webUrl);
                HttpWebRequest request = webRequest as HttpWebRequest;
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                String ticketUrl = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                tbl_product.ticket_url = ticketUrl;

                webUrl = Config.ConfigManager.getConfig("get-short-url");
                byte[] dataArray = Encoding.UTF8.GetBytes(tbl_product.content_url);
                webRequest = WebRequest.Create(webUrl);
                request = webRequest as HttpWebRequest;
                request.Method = "POST";
                request.ContentLength = dataArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(dataArray, 0, dataArray.Length);
                dataStream.Close();

                response = request.GetResponse();
                String shortUrl = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                tbl_product.short_url = shortUrl;

                db.tbl_product.AddObject(tbl_product);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(tbl_product);
        }

        
        //
        // GET: /Product/Edit/5
 
        public ActionResult Edit(int id)
        {
            tbl_product tbl_product = db.tbl_product.Single(t => t.product_id == id);
            ViewBag.types = new SelectList(db.tbl_product_type.ToList(), "id", "name");
            ViewBag.push_levels = new SelectList(PushLevelModel.GetPushLevelList(), "level", "name");
            return View(tbl_product);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(tbl_product tbl_product)
        {
            if (ModelState.IsValid)
            {
                db.tbl_product.Attach(tbl_product);
                db.ObjectStateManager.ChangeObjectState(tbl_product, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_product);
        }

        //
        // GET: /Product/Delete/5
 
        public ActionResult Delete(int id)
        {
            tbl_product tbl_product = db.tbl_product.Single(t => t.product_id == id);
            string type = db.tbl_product_type.Single(ty => ty.id == tbl_product.product_type).name;
            ProductModel pm = new ProductModel()
            {
                product_id = tbl_product.product_id,
                product_name = tbl_product.product_name,
                create_time = tbl_product.create_time,
                description = tbl_product.description,
                pic_url = tbl_product.pic_url,
                product_type = type,
                push_level = PushLevelModel.GetPushName(tbl_product.push_level),
                content_url = tbl_product.content_url
            };
            return View(pm);
        }

        //
        // POST: /Product/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            tbl_product temp = db.tbl_product.Single(m => m.product_id == id);
            temp.product_status = false;
            temp.push_level = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        //
        // GET: /Product/Create

        public ActionResult CreateNewType()
        {
            return View();
        }

        //
        // POST: /Product/Create

        [HttpPost]
        public ActionResult CreateNewType(tbl_product_type tbl_product_type)
        {
            if (ModelState.IsValid)
            {
                db.tbl_product_type.AddObject(tbl_product_type);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_product_type);
        }

        public ActionResult CreateTicket(int id)
        {
            tbl_product t = (from product in db.tbl_product
                             where product.product_id == id
                             select product).Single();
            //if (t.ticket_url == null)
            {
                string webUrl = "";
                webUrl = Config.ConfigManager.getConfig("get-ticket-url");
                webUrl = webUrl.Replace("EVENT_ID", t.product_id.ToString());
                WebRequest webRequest = WebRequest.Create(webUrl);
                HttpWebRequest request = webRequest as HttpWebRequest;
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                String ticketUrl = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                t.ticket_url = ticketUrl;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateShortUrl(int id)
        {
            tbl_product t = (from product in db.tbl_product
                             where product.product_id == id
                             select product).Single();
            string webUrl = "";
            webUrl = Config.ConfigManager.getConfig("get-short-url");
            byte[] dataArray = Encoding.UTF8.GetBytes(t.content_url);
            WebRequest webRequest = WebRequest.Create(webUrl);
            HttpWebRequest request = webRequest as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            String shortUrl = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            t.short_url = shortUrl;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult AdvMachineInfo(string machineName)
        {
            List<ProductInfo>[] pl = new List<ProductInfo>[6];
            pl[0] = new List<ProductInfo>();
            for (int i = 1; i < 4; i++)
            {
                adv_machine_product_list amp = (from adv in db.adv_machine_product_list
                                                where adv.machine_name == machineName && adv.location == i
                                                select adv ).Single();
                tbl_product temp = db.tbl_product.Single(m => m.product_id == amp.product_id);
                pl[0].Add(new ProductInfo
                {
                    product_id = temp.product_id,
                    product_name = temp.product_name,
                    short_url = temp.short_url,
                    ticket_url = temp.short_url,
                    pic_url = 微信活动管理.Config.ConfigManager.getConfig("pic-url") + temp.pic_url
                });
            }

            for (int i = 1; i < 6; i++)
            {
                pl[i] = new List<ProductInfo>();
                int type = i;
                List<tbl_product> products = (from pro in db.tbl_product
                                              where (pro.product_type == type && pro.push_level != 0)
                                              orderby pro.product_id descending
                                              select pro).ToList();
                foreach (tbl_product item in products)
                {
                    ProductInfo pi = new ProductInfo();
                    pi.product_id = item.product_id;
                    pi.product_name = item.product_name;
                    pi.short_url = item.short_url;
                    pi.ticket_url = item.ticket_url;
                    pi.pic_url = 微信活动管理.Config.ConfigManager.getConfig("pic-url") + item.pic_url ;
                    pl[i].Add(pi);
                }

            }
            JsonResult res = Json(new
            {
                daping = pl[0],
                ziyouyizu = pl[1],
                youyisi = pl[2],
                youleyouxuan = pl[3],
                liangyouyou = pl[4],
                guoyouyou = pl[5]
            });
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;

        }
        
    }


    public class ProductInfo
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string pic_url { get; set; }
        public string ticket_url { get; set; }
        public string short_url { get; set; }
    }

}