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
	/// 華視
	/// </summary>
	public class Cts
	{
		/// <summary>
		/// 華視網址
		/// </summary>
		private static readonly string DefulUrl = "https://news.cts.com.tw/";
		private static IBrowsingContext _context;

		/// <summary>
		/// 取得新聞
		/// </summary>
		/// <param name="_cnaType"></param>
		/// <returns></returns>
		public async Task<List<News>> GetNews(CtsType _ctsType)
		{
			string Url = DefulUrl + $"/{_ctsType}/index.html";
			string Html = await GetHttpClient.GetHtml(Url);
			var _allNews = new List<News>();
			if (!string.IsNullOrEmpty(Html))
			{
				var config = Configuration.Default;
				_context = BrowsingContext.New(config);
				var document = await _context.OpenAsync(res => res.Content(Html));
				var MainList = document.QuerySelector(".newslist-container.flexbox.one_row_style");
				if (MainList is not null)
					_allNews.AddRange(await GetHtmlNewsObject(MainList));
			}
			return _allNews;
		}

		private async Task<List<News>> GetHtmlNewsObject(IElement? _element)
		{
			var _allNews = new List<News>();
			var newsHtml = _element.QuerySelectorAll("a");
			Parallel.ForEach(newsHtml, async (element) =>
			{
				try
				{
					var _news = new News()
					{
						Title = element.GetAttribute("title") ?? string.Empty,
						Url = $"{element.GetAttribute("href") ?? string.Empty}",
						PublishedAt = Convert.ToDateTime($"{element.QuerySelector(".newstime")?.TextContent}" ?? DateTime.Now.ToString()),
						Source = "華視",
						UrlToImage = element.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty
					};
					await GetNewsBodyHtml(_news);
					if (!string.IsNullOrEmpty(_news.ContentBodyHtml))
						_allNews.Add(_news);
				}
				catch { }
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
				var centralContent = Bodydocument.QuerySelector(".artical-content[itemprop=articleBody]");
				_news.Author = Bodydocument.QuerySelector("head meta[itemprop=author]")?.GetAttribute("content") ?? string.Empty;
				_news.Description = Bodydocument.QuerySelector("head meta[itemprop=description]")?.GetAttribute("content") ?? string.Empty;


				

				var R1 = centralContent.QuerySelector("[id=yt_container_placeholder]");
				var R2 = centralContent.QuerySelectorAll(".flexbox.cts-tbfs");

				if (R1 is not null)
					centralContent.RemoveElement(R1);
				if(R2 is not null)
				{
					foreach (var c in R2)
						centralContent.RemoveElement(c);
				}
				_news.ContentBody = centralContent?.TextContent ?? string.Empty;
				_news.ContentBodyHtml = centralContent.InnerHtml;
			}
		}
	}

	public enum CtsType
	{
		/// <summary>
		/// 氣象
		/// </summary>
		weather,
		/// <summary>
		/// 政治
		/// </summary>
		politics,
		/// <summary>
		/// 國際
		/// </summary>
		international,
		/// <summary>
		/// 社會
		/// </summary>
		society,
		/// <summary>
		/// 運動
		/// </summary>
		sports,
		/// <summary>
		/// 生活
		/// </summary>
		life,
		/// <summary>
		/// 財經
		/// </summary>
		money,
		/// <summary>
		/// 台語
		/// </summary>
		taiwanese,
		/// <summary>
		/// 地方
		/// </summary>
		local,
		/// <summary>
		/// 產業
		/// </summary>
		pr,
		/// <summary>
		/// 綜合
		/// </summary>
		general,
		/// <summary>
		/// 藝文
		/// </summary>
		arts,
		/// <summary>
		/// 娛樂
		/// </summary>
		entertain,
	}

	
}
