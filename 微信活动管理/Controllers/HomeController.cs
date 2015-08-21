using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Data;
using 微信活动管理.Util;

namespace 微信活动管理.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string log)
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.Message = "欢迎使用邮政金融超市管理系统，" + User.Identity.Name + "先生!";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ViewBag.Message = "";
            }
            if (log == "-1")
            {
                ViewBag.error = "log fail";
            }
            return View();
        }


        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public string Upload(HttpPostedFileBase upImg)
        {
            string fileName = System.IO.Path.GetFileName(upImg.FileName);
            string dotName = fileName.Split('.')[fileName.Split('.').Length - 1];
            long timeStamp = DateTime.Now.Ticks;
            fileName = timeStamp + "." + dotName;

            string pic = "", error = "";
            try
            {
                upImg.SaveAs(@"C:\Program Files (x86)\Apache Software Foundation\Tomcat 7.0\webapps\zhmdImageFiles\" + fileName);
                pic = fileName;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return pic;
        }

        [HttpGet]
        public JsonResult Test()
        {
            return Json(new
            {
                val = "abc",
                key = "123"
            });
        }

        //public FileResult GetChart()
        //{
        //    List<string> xData = new List<string>() { "Jan", "Feb", "Mar", "Apr" , "May" , "Jun" };
        //    List<int> yData1 = new List<int>() { 3731 , 6024 , 4935 , 4466 , 5117 , 3546 };
        //    List<int> yData2 = new List<int>() { 4101 , 4324 , 2935 , 5644 , 5671 , 4646 };

        //    SheetHelper sh = new SheetHelper();
        //    sh.SheetType = SheetHelper.ChartType.Column;
        //    sh.Show3DStyle = false;
        //    sh.ShowPercentage = false;

        //    //Chart ch = sh.CreateChart("上半年", xData, yData1, 600, 300,"null","null");
        //    //Chart ch = SheetDrawing.DrawPieChartDemo();
        //    //MemoryStream imageStream = new MemoryStream();
        //    //ch.SaveImage(imageStream, ChartImageFormat.Png);
            
        //    imageStream.Position = 0;
        //    return new FileStreamResult(imageStream, "image/png");
        //}

    }
}
