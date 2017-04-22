using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AutohomeDealerCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            string webAddressTempl = @"http://dealer.autohome.com.cn/china/0_0_0_0_{0}.html";
            string webAddress;
            string resultFile = @"DealerInfo.csv";
            int pageNum = 1;
            int pageNumLmt = 2000;
            bool hasNextPage = false;
            string pattern1 = @"<a target=""_blank"" href=""/\d+/"" js-dname=""(?<DealerShortName>\S+)"" js-dbrand=""(?<BrandList>\S+)"" js-did=""(?<DealerId>\d+)"" js-darea=""(?<CityName>\S+)"" js-haspro=""\d+"">(?<DealerName>\S+)</a>";
            string pattern2=@"<span class=""dealer-api-phone( black)?"">(?<Phone>\S*)</span>(?<IsPhoneVerified>\s*<i class=""icon12 icon12-ok"" title=""\S+[ ]*""></i>)*(\s*<i class=""icon icon-24h1"" title=""\S+[ ]*""></i>)*\s*<i class=""icon icon-(?<SaleArea>sale\S+)"" title=""\S+""></i>\s*</span>\s*</div>\s*<div title=""(?<Address>[^""]*)"">";
            string nextPageDisabled = "page-item-next page-disabled";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (StreamWriter sw = new StreamWriter(resultFile,false,Encoding.GetEncoding("GB2312")))
            {
                sw.WriteLine(@"""DealerId"",""DealerName"",""DealerShortName"",""BrandList"",""CityName"",""Phone"",""IsPhoneVerified"",""SaleArea"",""Address"",""PageNum"",""ItemNum""");
                do
                {
                    webAddress = string.Format(webAddressTempl, pageNum);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webAddress);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string html = string.Empty;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                    {
                        html = sr.ReadToEnd();
                    }
                    List<DealerInfo> DealerList = new List<DealerInfo>();
                    Regex regex1 = new Regex(pattern1, RegexOptions.Multiline);
                    Regex regex2 = new Regex(pattern2, RegexOptions.Multiline);
                    MatchCollection matches1 = regex1.Matches(html);
                    MatchCollection matches2 = regex2.Matches(html);
                    //假设两个集的个数都相等。
                    int count = matches1.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Match match1 = matches1[i];
                        Match match2 = matches2[i];
                        DealerInfo dealerInfo = new DealerInfo();
                        dealerInfo.DealerId = int.Parse(match1.Groups["DealerId"].Value);
                        dealerInfo.DealerName = match1.Groups["DealerName"].Value;
                        dealerInfo.DealerShortName = match1.Groups["DealerShortName"].Value;
                        dealerInfo.BrandList = match1.Groups["BrandList"].Value.Replace("、", ",");
                        dealerInfo.CityName = match1.Groups["CityName"].Value;
                        dealerInfo.Phone = match2.Groups["Phone"].Value;
                        dealerInfo.IsPhoneVerified = string.IsNullOrEmpty(match2.Groups["IsPhoneVerified"].Value) ? 0 : 1;

                        switch (match2.Groups["SaleArea"].Value)
                        {
                            case "salebc":
                                dealerInfo.SaleArea = "售本市";
                                break;
                            case "salebp":
                                dealerInfo.SaleArea = "售本省";
                                break;
                            case "saleqg":
                                dealerInfo.SaleArea = "售全国";
                                break;
                            default:
                                dealerInfo.SaleArea = "售多地";
                                break;
                        }
                        dealerInfo.Address = match2.Groups["Address"].Value;
                        DealerList.Add(dealerInfo);
                        sw.WriteLine(@"""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}"",""{8}"",""{9}"",""{10}"""
                            , dealerInfo.DealerId
                            , dealerInfo.DealerName
                            , dealerInfo.DealerShortName
                            , dealerInfo.BrandList
                            , dealerInfo.CityName
                            , dealerInfo.Phone
                            , dealerInfo.IsPhoneVerified
                            , dealerInfo.SaleArea
                            , dealerInfo.Address
                            , pageNum
                            , i + 1
                            );
                    }
                    hasNextPage = !html.Contains(nextPageDisabled);
                    pageNum++;
                    request = null;
                    response = null;
                } while (hasNextPage && pageNum <= pageNumLmt);
            }
            watch.Stop();
            Console.WriteLine("========================================================");
            Console.WriteLine("Total cost: {0} minutes.", watch.Elapsed.TotalMinutes);
        }
    }
    class DealerInfo
    {
        public int DealerId;
        public string DealerName;
        public string DealerShortName;
        public string CityName;
        public string BrandList;
        public string Phone;
        public int IsPhoneVerified;
        public string SaleArea;
        public string Address;
    }
}
