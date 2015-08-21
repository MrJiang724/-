using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using 微信活动管理.Models;

namespace 微信活动管理.Controllers
{ 
    public class AdvController : Controller
    {
        private productEntities db = new productEntities();

        //
        // GET: /Adv/

        public ViewResult Index()
        {
            List<adv_machine_product_list> ampl = db.adv_machine_product_list.ToList();
            List<AdvMachineProduct> entities = new List<AdvMachineProduct>();
            foreach (adv_machine_product_list item in ampl)
            {
                AdvMachineProduct entity = new AdvMachineProduct();
                entity.id = item.id;
                entity.product_id = (int)item.product_id;
                entity.product_name = db.tbl_product.Single(m => m.product_id == item.product_id).product_name;
                entity.location = db.yanzhengma.Single(m => m.MachineName == item.machine_name).LocationName;
                entity.adv_location = "滚动位置" + item.location;
                entities.Add(entity);
            }

            return View(entities);
        }

        //
        // GET: /Adv/Details/5

        public ViewResult Details(int id)
        {
            adv_machine_product_list adv_machine_product_list = db.adv_machine_product_list.Single(a => a.id == id);
            tbl_product t = db.tbl_product.Single(a => a.product_id == adv_machine_product_list.product_id);
            string type = db.tbl_product_type.Single(name => name.id == t.product_type).name;
            ProductModel pm = new ProductModel
            {
                product_id = t.product_id,
                product_name = t.product_name,
                create_time = t.create_time,
                description = t.description,
                pic_url = t.pic_url,
                product_type = type,
                content_url = t.content_url,
                push_level = PushLevelModel.GetPushName(t.push_level),
                ticket_url = t.ticket_url,
                short_url = t.short_url
            };

            return View(pm);
        }

        
        //
        // GET: /Adv/Edit/5
 
        public ActionResult Edit(int id)
        {
            adv_machine_product_list adv_machine_product_list = db.adv_machine_product_list.Single(a => a.id == id);

            List<string> tl = new List<string>();
            var types = db.tbl_product_type.ToList();
            foreach (tbl_product_type tpt in types)
            {
                tl.Add(tpt.name);
            }
            List<List<tbl_product>> pl = new List<List<tbl_product>>();
            foreach(string type in tl)
            {
                Dictionary<string, int> p = new Dictionary<string, int>();
                int type_id = db.tbl_product_type.Single(t => t.name == type).id;
                List<tbl_product> products = (from t in db.tbl_product
                                              where t.product_type == type_id
                                              select t).ToList();
                pl.Add(products);

            }

            int pt_id = (int)db.tbl_product.Single(p => p.product_id == adv_machine_product_list.product_id).product_type;
            ViewBag.type = db.tbl_product_type.Single(p => p.id == pt_id).name;
            ViewBag.product_id = adv_machine_product_list.product_id;
            ViewBag.typeList = tl;
            ViewBag.productList = pl;
            return View(adv_machine_product_list);
        }

        //
        // POST: /Adv/Edit/5

        [HttpPost]
        public ActionResult Edit(adv_machine_product_list adv_machine_product_list)
        {
            if (ModelState.IsValid)
            {
                db.adv_machine_product_list.Attach(adv_machine_product_list);
                db.ObjectStateManager.ChangeObjectState(adv_machine_product_list, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adv_machine_product_list);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}