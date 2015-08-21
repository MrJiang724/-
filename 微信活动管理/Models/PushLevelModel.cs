using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 微信活动管理.Models
{
    public class PushLevelModel
    {
        public sbyte level { get; set; }
        public string name { get; set; }

        public static List<PushLevelModel> GetPushLevelList()
        {
            List<PushLevelModel> levels = new List<PushLevelModel>();
            levels.Add(new PushLevelModel()
            {
                level = 0,
                name = "不推送"
            });
            for (int i = 1; i < 9; i++)
            {
                levels.Add(new PushLevelModel()
                {
                    level = (sbyte)i,
                    name = "等级" + i
                });
            }

            return levels;
        }

        public static string GetPushName(sbyte level)
        {
            string levelName = "";
            switch (level)
            {
                case 0:
                    levelName = "不推送";
                    break;
                default:
                    levelName = "等级" + level;
                    break;
            }
            return levelName;
        }
    }
}