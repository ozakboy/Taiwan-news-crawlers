using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Taiwan_news_crawlers.Cna;

namespace Taiwan_news_crawlers
{
	/// <summary>
	/// 三立新聞網
	/// </summary>
	public class Setn
	{
		/// <summary>
		/// 三立新聞網網址
		/// </summary>
		private static readonly string DefulUrl = "https://www.setn.com";
		private static IBrowsingContext _context;

		/// <summary>
		/// 取得新聞
		/// </summary>
		/// <param name="_cnaType"></param>
		/// <returns></returns>
		public async Task<List<News>> GetNews(SetnType _cnaType)
		{
			string Url = DefulUrl + $"/ViewAll.aspx?PageGroupID={_cnaType.GetHashCode()}";
			string Html = await GetHttpClient.GetHtml(Url);
			var _allNews = new List<News>();
			if (!string.IsNullOrEmpty(Html))
			{
				var config = Configuration.Default;
				_context = BrowsingContext.New(config);
				var document = await _context.OpenAsync(res => res.Content(Html));
				var MainList = document.QuerySelector("[id=NewsList]");
				if (MainList is not null)
					_allNews.AddRange(await GetHtmlNewsObject(MainList));				
			}
			return _allNews;
		}

		/// <summary>
		/// 取得新聞物件
		/// </summary>
		/// <param name="_element"></param>
		/// <returns></returns>
		private async Task<List<News>> GetHtmlNewsObject(IElement? _element)
		{
			var _allNews = new List<News>();
			var newsHtml = _element.QuerySelectorAll(".col-sm-12.newsItems");
			Parallel.ForEach(newsHtml, async (element) =>
			{
				try
				{
					var _news = new News()
					{
						Title = element.QuerySelector(".view-li-title").TextContent.Trim(),
						Url = $"{DefulUrl}{element.QuerySelector(".view-li-title a").GetAttribute("href") ?? string.Empty}",
						PublishedAt = Convert.ToDateTime($" {DateTime.Now.Year}/{element.QuerySelector("time")?.TextContent}" ?? DateTime.Now.ToString()),
						Source = "setn三立新聞網",
						UrlToImage = string.Empty
					};
					await GetNewsBodyHtml(_news);
					if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
						_allNews.Add(_news);
				}
				catch{}
			});
			return _allNews;
		}

		/// <summary>
		/// 取得新聞內文
		/// </summary>
		/// <param name="_news"></param>
		/// <returns></returns>
		private async Task GetNewsBodyHtml(News _news)
		{
			var OneNewsHtml = await GetHttpClient.GetHtml(_news.Url);
			if (!string.IsNullOrEmpty(OneNewsHtml))
			{
				var Bodydocument = await _context.OpenAsync(res => res.Content(OneNewsHtml));
				var centralContent = Bodydocument.QuerySelector(".page-text").QuerySelector("[id=ckuse]").QuerySelector("article").QuerySelector("[id=Content1]");
				_news.Author = Bodydocument.QuerySelector("head meta[name=author]").GetAttribute("content") ?? string.Empty;
				_news.Description = Bodydocument.QuerySelector("head meta[name=Description]").GetAttribute("content") ?? string.Empty;

				_news.ContentBody = centralContent?.TextContent ?? string.Empty;
				_news.ContentBodyHtml = centralContent.InnerHtml;
			}
		}
		

	}

	#region 新聞類型

	public enum SetnType
	{
		/// <summary>
		/// 政治
		/// </summary>
		politics = 6,
		/// <summary>
		/// 社會
		/// </summary>
		Society = 41,
		/// <summary>
		/// 國際
		/// </summary>
		internationality = 5,
		/// <summary>
		/// 兩岸
		/// </summary>
		bilateral = 68,
		/// <summary>
		/// 生活
		/// </summary>
		Life = 4,
		/// <summary>
		/// 運動
		/// </summary>
		sports = 34,
		/// <summary>
		/// 地方
		/// </summary>
		local = 97,
		/// <summary>
		/// 財經
		/// </summary>
		financial = 2,
		/// <summary>
		/// 名家
		/// </summary>
		Famous = 9,
		/// <summary>
		/// 新奇
		/// </summary>
		novel = 42,
		/// <summary>
		/// 科技
		/// </summary>
		ScienceAndTechnology = 7,
		/// <summary>
		/// 汽車
		/// </summary>
		car = 12,
		/// <summary>
		/// 寵物
		/// </summary>
		pet = 47,
		/// <summary>
		/// 健康
		/// </summary>
		healthy = 65,

	}

	#endregion
}
