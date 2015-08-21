using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Data;
using System.Drawing;

namespace 微信活动管理.Util
{
    public class SheetHelper
    {
        /// <summary>
        /// 设置创建的图表类型
        /// </summary>
        public enum ChartType { Column, Line, Pie };
        /// <summary>
        /// 设置是否3D展示
        /// </summary>
        public bool Show3DStyle { get; set; }
        /// <summary>
        /// 是否以百分比显示数据
        /// </summary>
        public bool ShowPercentage { get; set; }

        public ChartType SheetType { get; set; }

        public SheetHelper()
        {
        }


        public Chart CreateChart(string title ,  List<string> xData , Dictionary<string ,List<int>>yData ,int width , int height , string xTitle = "null" , string yTitle = "null" ,int backgroundColor = 0x9fc5e8)
        {
            Chart chart = new Chart();
            
            if (!Show3DStyle)
            {
                chart.Width = width;
                chart.Height = height;
                chart.RenderType = RenderType.ImageTag;
                chart.Palette = ChartColorPalette.BrightPastel;
                Title t = new Title(title, Docking.Top, new System.Drawing.Font("Trebuchet MS", 14, System.Drawing.FontStyle.Bold), Color.FromArgb(23,33,24));
                chart.Titles.Add(t);
                chart.ChartAreas.Add("Series 1");
                if (xTitle != "null")
                {
                    chart.ChartAreas["Series 1"].AxisX.Title = xTitle;
                }
                if (yTitle != "null")
                {
                    chart.ChartAreas["Series 1"].AxisY.Title = yTitle;
                }
                chart.ChartAreas["Series 1"].BackColor = Color.FromArgb((backgroundColor+0x64000000));
                chart.ChartAreas["Series 1"].AxisX.MajorGrid.LineWidth = 0;
                foreach(var item in yData)
                {
                    string column = item.Key;
                    chart.Series.Add(column);
                    switch (SheetType)
                    {
                        case ChartType.Column:
                            chart.Series[column].ChartType = SeriesChartType.Column;
                            break;
                        case ChartType.Line:
                            chart.Series[column].ChartType = SeriesChartType.Line;
                            break;
                        case ChartType.Pie:
                            chart.Series[column].ChartType = SeriesChartType.Pie;
                            break;
                        default:
                            break;
                    }
                    chart.Series[column].Points.DataBindXY(xData, item.Value);
                    chart.Series[column].Label = "#VAL";
                }
                chart.BorderSkin.SkinStyle = BorderSkinStyle.None;
                //chart.BorderlineWidth = 2;
                //chart.BorderlineColor = System.Drawing.Color.Gray;
                //chart.BorderColor = System.Drawing.Color.Red;
                chart.BorderlineDashStyle = ChartDashStyle.NotSet;
                chart.BorderWidth = 2;
                chart.Legends.Add("Legend1");

                return chart;
            }
            else
            {
                return chart;
            }
        }


    }

    

}