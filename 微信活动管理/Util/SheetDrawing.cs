using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Data;

namespace 微信活动管理.Util
{
    public class SheetDrawing
    {
        //折线图示例
        public static Chart DrawLineChartDemo()
        {
            List<int> data = Models.StaticModel.createStaticData();
            System.Web.UI.DataVisualization.Charting.Chart Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();
            Chart2.Width = 600;
            Chart2.Height = 300;
            Chart2.RenderType = System.Web.UI.DataVisualization.Charting.RenderType.ImageTag;
            Chart2.Palette = ChartColorPalette.BrightPastel;
            Title t = new Title("表格", Docking.Top, new System.Drawing.Font("Trebuchet MS", 14, System.Drawing.FontStyle.Bold), System.Drawing.Color.FromArgb(26, 59, 105));
            Chart2.Titles.Add(t);
            Chart2.ChartAreas.Add("Series 1");
            // create a couple of series  
            Chart2.Series.Add("Series 1");
            Chart2.Series.Add("Series 2");
            Chart2.Series["Series 2"].ChartType = SeriesChartType.Line;
            DataTable dt = new DataTable();

            //Add three columns to the DataTable
            dt.Columns.Add("Date");
            dt.Columns.Add("Volume1");
            dt.Columns.Add("Volume2");

            DataRow dr;

            //Add rows to the table which contains some random data for demonstration
            dr = dt.NewRow();
            dr["Date"] = "Jan";
            dr["Volume1"] = 3731;
            dr["Volume2"] = 4101;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Feb";
            dr["Volume1"] = 6024;
            dr["Volume2"] = 4324;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Mar";
            dr["Volume1"] = 4935;
            dr["Volume2"] = 2935;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Apr";
            dr["Volume1"] = 4466;
            dr["Volume2"] = 5644;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "May";
            dr["Volume1"] = 5117;
            dr["Volume2"] = 5671;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Date"] = "Jun";
            dr["Volume1"] = 3546;
            dr["Volume2"] = 4646;
            dt.Rows.Add(dr);

            //设置图表的数据源
            Chart2.DataSource = dt;

            //设置图表Y轴对应项
            Chart2.Series["Series 1"].YValueMembers = "Volume1";
            Chart2.Series["Series 2"].YValueMembers = "Volume2";

            //设置图表X轴对应项
            Chart2.Series["Series 1"].XValueMember = "Date";

            //绑定数据
            Chart2.DataBind();


            //// add points to series 1  
            //foreach (int value in data)
            //{
            //    Chart2.Series["Series 1"].Points.AddY(value);
            //}
            //// add points to series 2  
            //foreach (int value in data)
            //{
            //    Chart2.Series["Series 2"].Points.AddY(value + 1);
            //}
            Chart2.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            Chart2.BorderlineWidth = 2;
            Chart2.BorderColor = System.Drawing.Color.Black;
            Chart2.BorderlineDashStyle = ChartDashStyle.Solid;
            Chart2.BorderWidth = 2;
            Chart2.Legends.Add("Legend1");

            return Chart2;
        }

        //饼状图示例
        public static Chart DrawPieChartDemo()
        {
            List<int> data = Models.StaticModel.createStaticData();
            System.Web.UI.DataVisualization.Charting.Chart Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();
            Chart2.Width = 600;
            Chart2.Height = 300;
            Chart2.RenderType = System.Web.UI.DataVisualization.Charting.RenderType.ImageTag;
            Chart2.Palette = ChartColorPalette.BrightPastel;
            Title t = new Title("表格", Docking.Top, new System.Drawing.Font("Trebuchet MS", 14, System.Drawing.FontStyle.Bold), System.Drawing.Color.FromArgb(26, 59, 105));
            Chart2.Titles.Add(t);
            Chart2.ChartAreas.Add("Series 1");
            // create a couple of series  
            Chart2.Series.Add("Series 1");
            //Chart2.Series.Add("Series 2");
            Chart2.Series["Series 1"].ChartType = SeriesChartType.Pie;
            Chart2.Series["Series 1"].IsValueShownAsLabel = true;
            Chart2.Series["Series 1"].Label = "#PERCENT{P2}";
            Chart2.Series["Series 1"].LegendText = "#VALX";
            DataTable dt = new DataTable();

            //Add three columns to the DataTable
            dt.Columns.Add("Date");
            dt.Columns.Add("Volume1");
            dt.Columns.Add("Volume2");

            //设置数据
            List<string> xData = new List<string>() { "Jan", "Feb", "Mar", "Apr" , "May" , "Jun" };
            List<int> yData1 = new List<int>() { 3731 , 6024 , 4935 , 4466 , 5117 , 3546 };
            List<int> yData2 = new List<int>() { 4101 , 4324 , 2935 , 5644 , 5671 , 4646 };

            //设置图表X、Y轴对应项
            Chart2.Series["Series 1"].Points.DataBindXY(xData, yData1);


            //绑定数据
            Chart2.DataBind();

            Chart2.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            Chart2.BorderlineWidth = 2;
            Chart2.BorderColor = System.Drawing.Color.Black;
            Chart2.BorderlineDashStyle = ChartDashStyle.Solid;
            Chart2.BorderWidth = 2;
            Chart2.Legends.Add("Legend1");

            return Chart2;
        }





    }
}