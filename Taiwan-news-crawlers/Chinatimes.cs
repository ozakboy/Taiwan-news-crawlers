using System.Text.RegularExpressions;

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
            string url = $"https://www.chinatimes.com/realtimenews/{newsType.GetHashCode()}/?chdtv";
            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (!string.IsNullOrEmpty(html))
            {
                var parser = new HtmlParser(html);
                var newsListElements = parser.QuerySelectorAll("div.articlebox-compact");

                var tasks = newsListElements.Select(async element => {
                    try
                    {
                        var titleElement = element.QuerySelector(".title");
                        if (titleElement == null) return null;

                        var linkElement = element.QuerySelector("a");
                        var imageElement = element.QuerySelector("img");
                        var timeElement = element.QuerySelector("time");
                        var introElement = element.QuerySelector("p.intro");

                        var news = new News
                        {
                            Title = titleElement.TextContent.Trim(),
                            Url = DefultUrl + (linkElement?.GetAttribute("href") ?? string.Empty),
                            UrlToImage = imageElement?.GetAttribute("src") ?? string.Empty,
                            Description = introElement?.TextContent.Trim() ?? string.Empty,
                            Source = "中時新聞網"
                        };

                        if (timeElement?.GetAttribute("datetime") is string dateStr)
                        {
                            if (DateTime.TryParse(dateStr, out var date))
                            {
                                news.PublishedAt = date;
                            }
                        }

                        // 獲取新聞內文
                        var articleHtml = await GetHttpClient.GetHtml(news.Url);
                        if (!string.IsNullOrEmpty(articleHtml))
                        {
                            var articleParser = new HtmlParser(articleHtml);
                            var authorElement = articleParser.QuerySelector(".meta-info .author");
                            var contentElement = articleParser.QuerySelector("div.article-body");

                            if (contentElement != null)
                            {
                                news.Author = authorElement?.TextContent.Trim() ?? string.Empty;
                                news.ContentBody = contentElement.TextContent.Trim();
                                news.ContentBodyHtml = contentElement.InnerHtml.Trim();
                                return news;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error if needed
                        Console.WriteLine($"Error processing news: {ex.Message}");
                    }
                    return null;
                }).ToList();

                var results = await Task.WhenAll(tasks);
                allNews.AddRange(results.Where(n => n != null)!);
            }

            return allNews;
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