using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCrawler11
{
    class VehicleStyle
    {
        public string VehicleStyleKey;
        public string Price_Prom;
        public string Price2_Mall;
    }
    class Program
    {
        static private List<string> addrList = new List<string>();
        static private List<VehicleStyle> VehicleList = new List<VehicleStyle>();
        static void Main(string[] args)
        {
            //string webAddress = @"http://www.yichemall.com/shop/shanghaivw/?source=yc_11_10";
            //string webAddress = @"http://11.yiche.com/List/";
            string webAddress = @"http://11.yiche.com";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string pattern = @"<a\s*href=""\s*(?<Addr>\S+)\s*""\s*target=""\S+""( class=""car-name"")?>(\s*<img\s*src=""\S+""\s*class=""\S+""\s*alt=""\S*""\s*/>)?\s*<dl>\s*<dt>(?<Name>[ \S]+)</dt>\s+<dd><strong>\s*￥(?<Price>\S+)</strong>\s*万起</dd>";
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                html = sr.ReadToEnd();
                html = html.Substring(html.IndexOf("<!-- 商城 start -->"), html.IndexOf("<!-- 商城 end -->") - html.IndexOf("<!-- 商城 start -->"));
                Regex regex1 = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection matches1 = regex1.Matches(html);
                int count = matches1.Count;
                for (int i = 0; i < count; i++)
                {
                    Match match1 = matches1[i];
                    string address = match1.Groups["Addr"].Value;
                    GetPrice(address);
                }

            }
        }

        private static void GetPrice(string address)
        {
            string webAddress = address;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string pattern = @"<a\s*href=""\s*(?<Addr>\S+)\s*""\s*target=""\S+""( class=""car-name"")?>(\s*<img\s*src=""\S+""\s*class=""\S+""\s*alt=""\S*""\s*/>)?\s*<dl>\s*<dt>(?<Name>[ \S]+)</dt>\s+<dd><strong>\s*￥(?<Price>\S+)</strong>\s*万起</dd>";
            string html = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                html = sr.ReadToEnd();
                html = html.Substring(html.IndexOf("<!-- 商城 start -->"), html.IndexOf("<!-- 商城 end -->") - html.IndexOf("<!-- 商城 start -->"));
                Regex regex1 = new Regex(pattern, RegexOptions.Multiline);
                MatchCollection matches1 = regex1.Matches(html);
                int count = matches1.Count;
                for (int i = 0; i < count; i++)
                {
                    Match match1 = matches1[i];
                    string address = match1.Groups["Addr"].Value;
                    GetPrice(address);
                }

            }
        }
    }
}
