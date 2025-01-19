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

        /// <summary>
        /// 取得新聞
        /// </summary>
        public async Task<List<News>> GetNews(SetnType setnType)
        {
            string url = DefulUrl + $"/ViewAll.aspx?PageGroupID={setnType.GetHashCode()}";
            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (string.IsNullOrEmpty(html)) return allNews;

            var parser = new HtmlParser(html);
            var newsList = parser.QuerySelector("[id=NewsList]");
            if (newsList == null) return allNews;

            var newsItems = newsList.QuerySelectorAll(".col-sm-12.newsItems");
            var tasks = newsItems.Select(async element =>
            {
                try
                {
                    var titleElement = element.QuerySelector(".view-li-title");
                    var linkElement = titleElement?.QuerySelector("a");
                    var timeElement = element.QuerySelector("time");

                    if (titleElement == null || linkElement == null) return null;

                    var news = new News
                    {
                        Title = titleElement.TextContent.Trim(),
                        Url = $"{DefulUrl}{linkElement.GetAttribute("href") ?? string.Empty}",
                        Source = "setn三立新聞網",
                        UrlToImage = string.Empty
                    };

                    if (timeElement != null)
                    {
                        news.PublishedAt = DateTime.Parse($"{DateTime.Now.Year}/{timeElement.TextContent}");
                    }

                    await FetchNewsContent(news);
                    if (!string.IsNullOrEmpty(news.ContentBodyHtml))
                    {
                        return news;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing news: {ex.Message}");
                }
                return null;
            });

            var results = await Task.WhenAll(tasks);
            allNews.AddRange(results.Where(n => n != null)!);
            return allNews;
        }

        private async Task FetchNewsContent(News news)
        {
            var html = await GetHttpClient.GetHtml(news.Url);
            if (string.IsNullOrEmpty(html)) return;

            var parser = new HtmlParser(html);
            var pageText = parser.QuerySelector(".page-text");
            if (pageText == null) return;

            var mainContent = pageText
                .QuerySelector("[id=ckuse]")?
                .QuerySelector("article")?
                .QuerySelector("[id=Content1]");

            if (mainContent == null) return;

            news.Author = parser.QuerySelector("head meta[name=author]")?.GetAttribute("content") ?? string.Empty;
            news.Description = parser.QuerySelector("head meta[name=Description]")?.GetAttribute("content") ?? string.Empty;
            news.ContentBody = mainContent.TextContent.Trim();
            news.ContentBodyHtml = mainContent.InnerHtml.Trim();
        }
    }

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
}