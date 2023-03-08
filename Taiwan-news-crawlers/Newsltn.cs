using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private static readonly string DefulUrl = "https://www.ltn.com.tw/";
		/// <summary>
		/// 自由時報-自由財經
		/// </summary>
		private static readonly string DefulStrategyUrl = "https://ec.ltn.com.tw/";
		private Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
		public async Task<List<News>> GetNews(NewsltnNewsType type)
		{
			string Url = string.Empty;
			if (type == NewsltnNewsType.strategy)
				Url = "https://ec.ltn.com.tw/list/strategy";
			else
				Url = $"https://news.ltn.com.tw/list/breakingnews/{type}";
			string Html = await GetHttpClient.GetHtml(Url);
			var _allNews = new List<News>();
			if (!string.IsNullOrEmpty(Html))
			{
				var config = Configuration.Default;
				var context = BrowsingContext.New(config);
				var document = context.OpenAsync(res => res.Content(Html)).Result;
				if (type == NewsltnNewsType.strategy)
				{
					var List = document.QuerySelector("div.whitecon.boxTitle.boxText[data-desc=列表]");
					var OneList = List.QuerySelectorAll(".listphoto a");
					Parallel.ForEach(OneList, async (news) =>
					{
						var _news = new News()
						{
							Title = news.GetAttribute("title") ?? string.Empty,
							Url = news.GetAttribute("href") ?? string.Empty,
							UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
							PublishedAt = Convert.ToDateTime(news.QuerySelector(".newstime").InnerHtml)
						};					
						Html = await GetHttpClient.GetHtml(_news.Url);
						if (!string.IsNullOrEmpty(Html))
						{
							var Bodydocument = context.OpenAsync(res => res.Content(Html)).Result;
							_news.ContentBodyHtml = rRemScript.Replace(Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text").InnerHtml.Trim(), "");
							_news.ContentBody = Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text").TextContent.Trim();
							_news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
							_news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
							_allNews.Add(_news);
						}
					});
					var TwoList = List.QuerySelectorAll("ul[data-desc=列表] li");
					Parallel.ForEach(TwoList, async (news) =>
					{
						var _news = new News()
						{
							Title = news.QuerySelector("a").GetAttribute("title") ?? string.Empty,
							Url = news.QuerySelector("a").GetAttribute("href") ?? string.Empty,
							UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
							PublishedAt = Convert.ToDateTime(news.QuerySelector(".newstime").InnerHtml)
						};
						Html = await GetHttpClient.GetHtml(_news.Url);
						if (!string.IsNullOrEmpty(Html))
						{
							var Bodydocument = context.OpenAsync(res => res.Content(Html)).Result;
							_news.ContentBodyHtml = rRemScript.Replace(Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text").InnerHtml.Trim(), "");
							_news.ContentBody = Bodydocument.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text").TextContent.Trim();
							_news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
							_news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
							_allNews.Add(_news);
						}
					});
				}
				else
				{
					var NewsList = document.QuerySelectorAll("ul.list li");
					Parallel.ForEach(NewsList, async (news) =>
				   {
					   var _news = new News()
					   {
						   Title = news.QuerySelector(".title")?.TextContent.Trim(),
						   Url = news.QuerySelector("a")?.GetAttribute("href") ?? string.Empty,
						   UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
					   };
					   Html = await GetHttpClient.GetHtml(_news.Url);
					   if (!string.IsNullOrEmpty(Html))
					   {
						   var Bodydocument = context.OpenAsync(res => res.Content(Html)).Result;
						   _news.PublishedAt = Convert.ToDateTime(Bodydocument.QuerySelector(".time")?.TextContent ?? string.Empty);
						   _news.ContentBodyHtml = rRemScript.Replace(Bodydocument.QuerySelector(".text.boxTitle.boxText").InnerHtml.Trim(), "");
						   _news.ContentBody = Bodydocument.QuerySelector(".text.boxTitle.boxText").TextContent.Trim();
						   _news.Description = Bodydocument.QuerySelector("head meta[name=description]").GetAttribute("content") ?? string.Empty;
						   _news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
						   _allNews.Add(_news);
					   }
				   });
				}
			}
			return _allNews;
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
