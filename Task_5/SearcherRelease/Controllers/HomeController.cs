using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearcherRelease.Models;
using PagedList;
using HtmlAgilityPack;
using System.Threading;

namespace SearcherRelease.Controllers
{
    public class HomeController : Controller
    {
        private const int CountOfItem = 18;

        static private List<ParseResult> listResult = new List<ParseResult>();

        static int currC;

        static int flagEnd;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(String input_field)
        {
            //currC = 0;
            flagEnd = 0;
            Session["search_string"] = input_field;
            Thread firstThread = new Thread(parseBelChip);
            Thread secondThread = new Thread(parseChipDip);
            firstThread.Start();
            secondThread.Start();
            while (getCount() < CountOfItem) ;
            return View(listResult.GetRange(0, CountOfItem));
        }

        private int getCount()
        {
            lock (listResult)
            {
                return listResult.Count();
            }
        }

        private void setFlag()
        {
            lock(listResult)
            {
                flagEnd++;
            }
        }

        private int getFlagEnd()
        {
            lock(listResult)
            {
                return flagEnd;
            }
        }

        [HttpGet]
        public JsonResult GetNextPage(int page)
        {
            int currentIndex = page * CountOfItem;
            while (getCount() < currentIndex + CountOfItem)
            {
                if (getFlagEnd() == 2 && currentIndex + CountOfItem > getCount())
                {
                    if (getCount() - currentIndex <= 0)
                        return Json(-1, JsonRequestBehavior.AllowGet);
                    else
                        return Json(listResult.GetRange(currentIndex, getCount() - currentIndex), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(listResult.GetRange(currentIndex, CountOfItem), JsonRequestBehavior.AllowGet);
        }

        private void parseBelChip()
        {
            string request = Session["search_string"]?.ToString();
            var html = new HtmlWeb().Load("http://belchip.by/search/?query=" + request);
            var MainPage = html.DocumentNode.SelectNodes("//div[@class='cat-item']");
            if (MainPage == null)
            {
                setFlag();
                return;
            }
            string Count;
            try
            {
                foreach (var document in MainPage)
                {
                    if (document.SelectSingleNode(".//div[@class='butt-add']/div[@class='getorder']/input[@class='buttonNew buttonNew-grey']") == null)
                        Count = ((double.Parse(document.SelectNodes(".//div[@class='butt-add']/div[@class='getorder']/div[@class='shops']/div")[0].SelectNodes(".//div")[0].InnerText.Split(' ')[0])) +
                                                   (double.Parse(document.SelectNodes(".//div[@class='butt-add']/div[@class='getorder']/div[@class='shops']/div")[1].SelectNodes(".//div")[0].InnerText.Split(' ')[0]))).ToString();
                    else Count = "";
                    string Name = document.SelectSingleNode(".//h3").InnerText;
                    string Image = "http://belchip.by/" + document.SelectSingleNode(".//div[@class='cat-pic']/a").Attributes["href"].Value;
                    string Href = "http://belchip.by/" + document.SelectSingleNode(".//h3/a").Attributes["href"].Value;
                    string Cost = document.SelectSingleNode(".//div[@class='butt-add']/span").InnerText;
                    addToList(Name, Image, Href, Count, Cost);
                }
            }
            catch (NullReferenceException e)
            { }
            setFlag();
            return;
        }

        private void parseChipDip()
        {
            string request = Session["search_string"]?.ToString();
            List<string> CategoryLinks = GetCategoryLinks(request);
            for (int i = 0; i < CategoryLinks.Count; i++)
            {
                var Html = new HtmlWeb().Load(CategoryLinks[i]);
                while (true)
                {
                    var HtmlNode = Html.DocumentNode.SelectNodes("//tr[@class='with-hover']");
                    try
                    {
                        foreach (var value in HtmlNode)
                        {
                            //Image_Link                       
                            string Image_Link = value.SelectSingleNode(".//td[@class='img']/div[@class='img-wrapper']/span[@class='galery']/img") == null ? "/Content/img/i.jpg" :
                                value.SelectSingleNode(".//td[@class='img']/div[@class='img-wrapper']/span[@class='galery']/img").Attributes["src"].Value;
                            //Link
                            string Link = "https://www.ru-chipdip.by" + value.SelectSingleNode(".//td[@class='h_name']/div/a").Attributes["href"].Value;
                            string Count = value.SelectSingleNode(".//div[@class='av_w2']/span[@class='item__avail_order']") == null ? "1" : "";
                            string Name = value.SelectSingleNode(".//div/a").InnerText;
                            string Cost = value.SelectSingleNode(".//span[contains(@class,'price_mr')]").InnerText;
                            addToList(Name, Image_Link, Link, Count, Cost);
                            //Debug.Write(result.Count+"\n");
                        }
                    }
                    catch (NullReferenceException e)
                    { }
                    var NextLinkNode = Html.DocumentNode.SelectSingleNode("//a[@class='link no-visited pager__control pager__next']");
                    if (NextLinkNode == null)
                    {
                        setFlag();
                        return;
                    }
                    var NextLink = "https://www.ru-chipdip.by" + NextLinkNode.Attributes["href"].Value;
                    NextLink = NextLink.Replace("&amp;", "&");
                    Html = new HtmlWeb().Load(NextLink);
                }
            }
            setFlag();
            return;
        }

        private void addToList(string name, string image, string href, string count, string cost)
        {
            lock (listResult)
            {
                listResult.Add(new ParseResult
                {
                    Name = name,
                    Image = image,
                    Href = href,
                    Count = count, //(Convert.ToDouble(minskCount) + Convert.ToDouble(moscowCount)).ToString(),
                    Cost = cost
                });
                //currC++;
            }
        }

        private List<string> GetCategoryLinks(string request)
        {
            var Html = new HtmlWeb().Load("https://www.ru-chipdip.by/search?searchtext=" + request);
            List<string> Links = new List<string>();
            while (true)
            {
                var LinksNodes = Html.DocumentNode.SelectNodes("//td[@class='group-header-wrap']/a");
                if (LinksNodes == null)
                    return Links;
                foreach (var item in LinksNodes)
                {
                    Links.Add("https://www.ru-chipdip.by" + item.Attributes["href"].Value.Replace("/catalog-show/", "/catalog/"));
                }
                var NextLinkNode = Html.DocumentNode.SelectSingleNode("//a[@class='link no-visited pager__control pager__next']");
                if (NextLinkNode == null)
                    break;
                var NextLink = "https://www.ru-chipdip.by" + NextLinkNode.Attributes["href"].Value;
                NextLink = NextLink.Replace("&amp;", "&");
                Html = new HtmlWeb().Load(NextLink);
            }
            return Links;
        }
    }
}