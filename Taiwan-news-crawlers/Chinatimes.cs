using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;


namespace Taiwan_news_crawlers
{
	/// <summary>
	/// 中時新聞網
	/// </summary>
	public class Chinatimes
	{
		private static readonly string DefultUrl = "https://www.chinatimes.com";
		public async Task<List<News>> GetNews(ChinatimesNewsType newsType)
		{
			string Url = $"https://www.chinatimes.com/realtimenews/{newsType.GetHashCode()}/?chdtv";
			string Html = await GetHttpClient.GetHtml(Url);
			var config = Configuration.Default;
			var context = BrowsingContext.New(config);
			var document = context.OpenAsync(res => res.Content(Html)).Result;
			var NewsList = document.QuerySelectorAll("section.article-list ul li div.articlebox-compact").Select(x=>x.FirstElementChild);
			var _allnews = new List<News>();
            Parallel.ForEach(NewsList, async (news) =>
            {
                var _news = new News()
                {
                    Title = news.QuerySelector(".title").TextContent.Trim(),
                    Url = DefultUrl + news.QuerySelector("a")?.GetAttribute("href") ?? string.Empty,
                    UrlToImage = news.QuerySelector("div a img")?.GetAttribute("src") ?? string.Empty,
                    PublishedAt = Convert.ToDateTime(news.QuerySelector("time")?.GetAttribute("datetime") ?? string.Empty),
                    Description = news.QuerySelector("p.intro").TextContent.Trim()
                };
                var OneNewsHtml = await GetHttpClient.GetHtml(_news.Url);
                var Bodydocument = context.OpenAsync(res => res.Content(OneNewsHtml)).Result;
                _news.Author = Bodydocument.QuerySelector(".meta-info .author").TextContent.Trim();
                _news.ContentBody = Bodydocument.QuerySelector("div.article-body").TextContent.Trim();
                _news.ContentBodyHtml = Bodydocument.QuerySelector("div.article-body").InnerHtml.Trim();
                _allnews.Add(_news);
            });
			return _allnews;
		}

		
	}

    public enum ChinatimesNewsType
    {
        /// <summary>
        /// 政治
        /// </summary>
        Politics = 260407,
        /// <summary>
        /// 生活
        /// </summary>
        Life = 260405,
        /// <summary>
        /// 社會
        /// </summary>
        Society = 260402,
        /// <summary>
        /// 娛樂
        /// </summary>
        Entertainment = 260404,
        /// <summary>
        /// 體育
        /// </summary>
        Physical_Education = 260403,
        /// <summary>
        /// 財經
        /// </summary>
        Financial = 260410,
        /// <summary>
        /// 國際
        /// </summary>
        Internationality = 260408,
        /// <summary>
        /// 兩岸
        /// </summary>
        Both_Sides_of_The_Strait = 260409,
        /// <summary>
        /// 科技
        /// </summary>
        Technology = 260412,
        /// <summary>
        /// 軍事
        /// </summary>
        Military = 260417,
        /// <summary>
        /// 健康
        /// </summary>
        Healthy = 260418
    }
}
