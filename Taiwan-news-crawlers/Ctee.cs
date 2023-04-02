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
	/// 工商時報
	/// </summary>
	public class Ctee
	{
		/// <summary>
		/// 工商時報網址
		/// </summary>
		private static readonly string DefulUrl = "https://ctee.com.tw/";
		private static IBrowsingContext _context;

		/// <summary>
		/// 取得新聞
		/// </summary>
		/// <param name="_cnaType"></param>
		/// <returns></returns>
		public async Task<List<News>> GetNews(CteeType _cteeType)
		{
			string Url = DefulUrl + $"livenews/{_cteeType}";
			string Html = await GetHttpClient.GetHtml(Url);
			var _allNews = new List<News>();
			if (!string.IsNullOrEmpty(Html))
			{
				var config = Configuration.Default;
				_context = BrowsingContext.New(config);
				var document = await _context.OpenAsync(res => res.Content(Html));
				var MainList = document.QuerySelector(".listing.listing-text.listing-text-2.clearfix");
				if (MainList is not null)
					_allNews.AddRange(await GetHtmlNewsObject(MainList));
			}
			return _allNews;
		}

		private async Task<List<News>> GetHtmlNewsObject(IElement? _element)
		{
			var _allNews = new List<News>();
			var newsHtml = _element.QuerySelectorAll(".item-content");
			Parallel.ForEach(newsHtml, async (element) =>
			{
				var _news = new News();
				_news.Url = element.FirstElementChild.Children[1].GetAttribute("href") ?? string.Empty;
				var Time = element.FirstElementChild.Children[1].QuerySelector("span")?.TextContent.Replace("|", "").Trim() ?? string.Empty;
				_news.PublishedAt = Convert.ToDateTime($"{DateTime.Now.Year}/{Time}");
				_news.Source = "工商時報";
				_news.Title = element.FirstElementChild.Children[1].TextContent.Trim().Replace("|", "").Replace(Time, "").Trim();
				await GetNewsBodyHtml(_news);
				if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
					_allNews.Add(_news);
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
				var centralContent = Bodydocument.QuerySelector(".entry-content.clearfix.single-post-content");
				_news.Author = Bodydocument.QuerySelector(".post-meta-author")?.TextContent ?? string.Empty;
				_news.Description = string.Empty;
				_news.UrlToImage = Bodydocument.QuerySelector(".single-featured")?.QuerySelector("figure")?.QuerySelector("img")?.GetAttribute("src") ?? string.Empty;

				_news.ContentBody = centralContent?.TextContent ?? string.Empty;
				_news.ContentBodyHtml = centralContent.InnerHtml;
			}
		}
	}

	public enum CteeType
	{
		/// <summary>
		/// 財經
		/// </summary>
		aj,
		/// <summary>
		/// 國際
		/// </summary>
		gj,
		/// <summary>
		/// 政治
		/// </summary>
		jj,
		/// <summary>
		/// 兩岸
		/// </summary>
		lm,
		/// <summary>
		/// 科技
		/// </summary>
		kj,
		/// <summary>
		/// 生活
		/// </summary>
		ch
	}
}
