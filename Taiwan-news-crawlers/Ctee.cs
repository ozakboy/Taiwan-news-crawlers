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

        /// <summary>
        /// 取得新聞
        /// </summary>
        public async Task<List<News>> GetNews(CteeType cteeType)
        {
            string url = DefulUrl + $"livenews/{cteeType}";
            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (string.IsNullOrEmpty(html)) return allNews;

            var parser = new HtmlParser(html);
            var mainList = parser.QuerySelector(".listing.listing-text.listing-text-2.clearfix");
            if (mainList == null) return allNews;

            var newsItems = mainList.QuerySelectorAll(".item-content");
            var tasks = newsItems.Select(async element =>
            {
                try
                {
                    var firstChild = element.QuerySelector("a");
                    if (firstChild == null) return null;

                    var timeSpan = firstChild.QuerySelector("span");
                    var timeText = timeSpan?.TextContent.Replace("|", "").Trim() ?? string.Empty;

                    var news = new News
                    {
                        Url = firstChild.GetAttribute("href") ?? string.Empty,
                        Source = "工商時報",
                        Title = firstChild.TextContent.Trim()
                                        .Replace("|", "")
                                        .Replace(timeText, "")
                                        .Trim()
                    };

                    if (!string.IsNullOrEmpty(timeText))
                    {
                        news.PublishedAt = DateTime.Parse($"{DateTime.Now.Year}/{timeText}");
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
            var contentElement = parser.QuerySelector(".entry-content.clearfix.single-post-content");
            if (contentElement == null) return;

            var authorElement = parser.QuerySelector(".post-meta-author");
            var imageElement = parser.QuerySelector(".single-featured figure img");

            news.Author = authorElement?.TextContent?.Trim() ?? string.Empty;
            news.Description = string.Empty;
            news.UrlToImage = imageElement?.GetAttribute("src") ?? string.Empty;
            news.ContentBody = contentElement.TextContent?.Trim() ?? string.Empty;
            news.ContentBodyHtml = contentElement.InnerHtml?.Trim() ?? string.Empty;
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