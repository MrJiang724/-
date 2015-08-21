using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Text;

namespace 微信活动管理.Config
{
    public class ConfigManager
    {
        private static Dictionary<string, string> dic;
        static ConfigManager()
        {
            XmlDocument doc = new XmlDocument();
            dic = new Dictionary<string, string>();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;

            string config = new StreamReader(new FileStream(path + "\\Config\\Config.xml", FileMode.Open), Encoding.UTF8).ReadToEnd();
            doc.LoadXml(config);
            XmlNode root = (XmlNode)doc.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                dic.Add(node.Name, node.InnerText);
            }
        }
        public static string getConfig(string key)
        {
            return dic[key];
        }
    }
}