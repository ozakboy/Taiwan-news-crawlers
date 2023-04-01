using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Taiwan_news_crawlers
{
    /// <summary>
    /// 中央通訊社
    /// </summary>
    public class Cna
    {
        /// <summary>
        /// 中央通訊社網址
        /// </summary>
        private static readonly string DefulUrl = "https://www.cna.com.tw/";

        private static IBrowsingContext _context;
        /// <summary>
        /// 取得新聞
        /// </summary>
        /// <returns></returns>
        public async Task<List<News>> GetNews(CnaType _cnaType)
        {
            string Url = DefulUrl + $"list/{_cnaType}.aspx";
            string Html = await GetHttpClient.GetHtml(Url);
            var _allNews = new List<News>();
            if (!string.IsNullOrEmpty(Html))
            {
                var config = Configuration.Default;
                _context = BrowsingContext.New(config);
                var document = await _context.OpenAsync(res => res.Content(Html));
                var allList = document.QuerySelector(".statement");
                var MainList = allList.QuerySelector(".mainList");
                if (MainList is not null)                
                    _allNews.AddRange(await GetHtmlNewsObject(MainList));                
                var jsMainList = allList.QuerySelector(".mainList.imgModule");
                if (jsMainList is not null)
                    _allNews.AddRange(await GetHtmlNewsObject(jsMainList));
            }
            return _allNews;
        }

        private async Task<List<News>> GetHtmlNewsObject(IElement? _element)
        {
            var _allNews = new List<News>();

            var newsHtml = _element.QuerySelectorAll("li");

            Parallel.ForEach(newsHtml, async (element) =>
            {
                try
                {
					var _news = new News()
					{
						Title = element.QuerySelector("span").TextContent.Trim(),
						Url = element.QuerySelector("a").GetAttribute("href") ?? string.Empty,
						PublishedAt = Convert.ToDateTime(element.QuerySelector(".date")?.TextContent ?? DateTime.Now.ToString()),
						UrlToImage = string.Empty,
						Source = "中央通訊社"
					};
					await GetNewsBodyHtml(_news);
					if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
						_allNews.Add(_news);
				}
                catch{}

            });
            return _allNews;
        }

        private async Task GetNewsBodyHtml(News _news)
        {
            var OneNewsHtml = await GetHttpClient.GetHtml(_news.Url);
            if (!string.IsNullOrEmpty(OneNewsHtml))
            {
                var Bodydocument = await _context.OpenAsync(res => res.Content(OneNewsHtml));
                var centralContent = Bodydocument.QuerySelector(".centralContent");
                var R1 = centralContent?.QuerySelector(".paragraph")?.QuerySelectorAll(".paragraph.moreArticle.flexhalf") ?? default;
                var R2 = centralContent?.QuerySelector(".paragraph")?.QuerySelector(".paragraph.bottomBox") ?? default;
                var ContentBodyHtml = centralContent?.QuerySelector(".paragraph") ?? default;
                if (ContentBodyHtml is null)
                    return;
                if (R1 is not null)
                {
                    foreach (var c in R1)
                        ContentBodyHtml?.RemoveElement(c);
                }
                if (R2 is not null)
                    ContentBodyHtml?.RemoveElement(R2);
                _news.ContentBodyHtml = ContentBodyHtml.InnerHtml;

                _news.ContentBody = ContentBodyHtml.TextContent.Trim();

                _news.UrlToImage = Bodydocument.QuerySelector(".fullPic")?.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty;
                _news.Author = Bodydocument.QuerySelector("head meta[itemprop=author]").GetAttribute("content") ?? string.Empty;
                _news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
            }
        }             
    }

	public enum CnaType
	{
		/// <summary>
		/// 政治
		/// </summary>
		aipl,
		/// <summary>
		/// 國際
		/// </summary>
		aopl,
		/// <summary>
		/// 兩岸
		/// </summary>
		acn,
		/// <summary>
		/// 產經
		/// </summary>
		aie,
		/// <summary>
		/// 證券
		/// </summary>
		asc,
		/// <summary>
		/// 科技
		/// </summary>
		ait,
		/// <summary>
		/// 生活
		/// </summary>
		ahel,
		/// <summary>
		/// 社會
		/// </summary>
		asoc,
		/// <summary>
		/// 地方
		/// </summary>
		aloc,
		/// <summary>
		/// 文化
		/// </summary>
		acul,
		/// <summary>
		/// 運動
		/// </summary>
		aspt,
		/// <summary>
		/// 娛樂
		/// </summary>
		amov
	}
}
