using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	/// <summary>
	/// 自由時報
	/// </summary>
	public class Newsltn
	{
		/// <summary>
		/// 自由時報網址
		/// </summary>
		private static readonly string DefulUrl = "https://news.ltn.com.tw/";
		/// <summary>
		/// 自由時報-自由財經
		/// </summary>
		private static readonly string DefulStrategyUrl = "https://ec.ltn.com.tw/";
        private static IBrowsingContext _context;

		public async Task<List<News>> GetNews(NewsltnNewsType type)
		{
			string Url = string.Empty;
			if (type == NewsltnNewsType.strategy)
				Url = $"{DefulStrategyUrl}list/strategy";
			else
				Url = $"{DefulUrl}list/breakingnews/{type}";
			string Html = await GetHttpClient.GetHtml(Url);
			var _allNews = new List<News>();
			if (!string.IsNullOrEmpty(Html))
			{
				var config = Configuration.Default;
				 _context = BrowsingContext.New(config);
				var document = _context.OpenAsync(res => res.Content(Html)).Result;
				if (type == NewsltnNewsType.strategy)
				{
                    var List = document.QuerySelector("div.whitecon.boxTitle.boxText[data-desc=列表]");
                    _allNews.AddRange(await GetHtmlNewsStrategyObject(List));
				}
				else
				{
                    var List = document.QuerySelector("ul.list ");
                    _allNews.AddRange(await GetHtmlNewsObject(List));			
				}
			}
			return _allNews;
		}


        private async Task<List<News>> GetHtmlNewsStrategyObject(IElement? _element)
		{
            var _allNews = new List<News>();
            var OneList = _element.QuerySelectorAll(".listphoto");
            Parallel.ForEach(OneList, async (news) =>
            {
                var _news = new News()
                {
                    Title = news.GetAttribute("title") ?? string.Empty,
                    Url = news.GetAttribute("href") ?? string.Empty,
                    UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                    PublishedAt = Convert.ToDateTime(news.QuerySelector(".newstime").InnerHtml),
                    Source = "自由時報"
                };
                await GetNewsStrategyBodyHtml(_news);
                if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
                    _allNews.Add(_news);
            });

            var TwoList = _element.QuerySelectorAll("ul[data-desc=列表] li");
            Parallel.ForEach(TwoList, async (news) =>
            {
                var _news = new News()
                {
                    Title = news.QuerySelector("a").GetAttribute("title") ?? string.Empty,
                    Url = news.QuerySelector("a").GetAttribute("href") ?? string.Empty,
                    UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                    PublishedAt = Convert.ToDateTime(news.QuerySelector(".newstime").InnerHtml),
                    Source = "自由時報"
                };
                await GetNewsStrategyBodyHtml(_news);
                if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
                    _allNews.Add(_news);
            });

            return _allNews;
        }

        private async Task GetNewsStrategyBodyHtml(News _news)
        {
            var OneNewsHtml = await GetHttpClient.GetHtml(_news.Url);
            if (!string.IsNullOrEmpty(OneNewsHtml))
            {
                var Bodydocument = await _context.OpenAsync(res => res.Content(OneNewsHtml));
                var centralContent = Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text");
                #region 刪除無用區塊

                var R2= centralContent.QuerySelector(".before_ir");
                var R3= centralContent.QuerySelector(".after_ir");
                var R4= centralContent.QuerySelector(".appE1121");
                var R5= centralContent.QuerySelector("[id=oneadIRMIRTag]");
                var R6= centralContent.QuerySelector("[id=ad-IR1]");
                var R7= centralContent.QuerySelector("[id=ad-IR2]");
                var R8= centralContent.QuerySelectorAll("script");
                var R9= centralContent.QuerySelector(".suggest");

         
                if (R2 is not null)
                    centralContent.RemoveElement(R2);
                if (R3 is not null)
                    centralContent.RemoveElement(R3);
                if (R4 is not null)
                    centralContent.RemoveElement(R4);
                if (R5 is not null)
                    centralContent.RemoveElement(R5);
                if (R6 is not null)
                    centralContent.RemoveElement(R6);
                if (R7 is not null)
                    centralContent.RemoveElement(R7);
                if (R8 is not null)
                {
                    foreach (var c in R8)
                    {
                        try { centralContent?.RemoveElement(c); }
                        catch { }
                    }
                }
                if (R9 is not null)
                    centralContent.RemoveElement(R9);
                #endregion
                _news.ContentBodyHtml = centralContent.InnerHtml.Trim();
                _news.ContentBody = Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text").TextContent.Trim();
                _news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
                _news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
            }
        }


        private async Task<List<News>> GetHtmlNewsObject(IElement? _element)
        {
            var _allNews = new List<News>();
            var NewsList = _element.QuerySelectorAll("li");
            Parallel.ForEach(NewsList, async (news) =>
            {
                var _news = new News()
                {
                    Title = news.QuerySelector(".title")?.TextContent.Trim(),
                    Url = news.QuerySelector("a")?.GetAttribute("href") ?? string.Empty,
                    UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                    Source = "自由時報"
                };
                await GetNewsBodyHtml(_news);
                if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
                    _allNews.Add(_news);
            });    
            return _allNews;
        }

        private async Task GetNewsBodyHtml(News _news)
        {
            var OneNewsHtml = await GetHttpClient.GetHtml(_news.Url);
            if (!string.IsNullOrEmpty(OneNewsHtml))
            {
                var Bodydocument = await _context.OpenAsync(res => res.Content(OneNewsHtml));
                var centralContent = Bodydocument.QuerySelector(".text.boxTitle.boxText");
                _news.PublishedAt = Convert.ToDateTime(centralContent.QuerySelector(".time")?.TextContent ?? string.Empty);

                #region 刪除無用區塊

                var R1 = centralContent.QuerySelector(".photo");
                var R2 = centralContent.QuerySelector(".time");
                var R3 = centralContent.QuerySelector(".before_ir");
                var R4 = centralContent.QuerySelector("[id=oneadIRMIRTag]");
                var R5 = centralContent.QuerySelector("[id=ad-IR1]");
                var R6 = centralContent.QuerySelectorAll("script");
                var R7 = centralContent.QuerySelector(".suggest");
                var R8 = centralContent.QuerySelector(".appE1121");
            
                if (R1 is not null)
                    centralContent?.RemoveElement(R1);
                if (R2 is not null)
                    centralContent?.RemoveElement(R2);
                if (R3 is not null)
                    centralContent?.RemoveElement(R3);
                if (R4 is not null)
                    centralContent?.RemoveElement(R4);
                if (R5 is not null)
                    centralContent?.RemoveElement(R5);
                if (R6 is not null)
                {
                    foreach(var c in R6)
                    {
                        try{centralContent?.RemoveElement(c);}
                        catch{}
                    }
                }                   
                if (R7 is not null)
                    centralContent?.RemoveElement(R7);
                if (R8 is not null)
                    centralContent?.RemoveElement(R8);

                #endregion

                _news.ContentBodyHtml = centralContent.InnerHtml.Trim();
                _news.ContentBody = centralContent.TextContent.Trim();
                _news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
                _news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
            }
        }




    }

    public enum NewsltnNewsType
    {
        /// <summary>
        /// 政治
        /// </summary>
        politics,
        /// <summary>
        /// 社會
        /// </summary>
        society,
        /// <summary>
        /// 生活
        /// </summary>
        life,
        /// <summary>
        /// 國際
        /// </summary>
        world,
        /// <summary>
        /// 地方
        /// </summary>
        local,
        /// <summary>
        /// 蒐奇
        /// </summary>
        novelty,
        /// <summary>
        /// 財經
        /// </summary>
        strategy,
    }

}
