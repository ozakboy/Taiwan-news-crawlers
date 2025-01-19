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

        /// <summary>
        /// 取得新聞
        /// </summary>
        public async Task<List<News>> GetNews(CtsType ctsType)
        {
            string url = DefulUrl + $"/{ctsType}/index.html";
            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (string.IsNullOrEmpty(html)) return allNews;

            var parser = new HtmlParser(html);
            var mainList = parser.QuerySelector(".newslist-container.flexbox.one_row_style");
            if (mainList == null) return allNews;

            var newsLinks = mainList.QuerySelectorAll("a");
            var tasks = newsLinks.Select(async element =>
            {
                try
                {
                    var timeElement = element.QuerySelector(".newstime");
                    var imageElement = element.QuerySelector("img");

                    var news = new News
                    {
                        Title = element.GetAttribute("title") ?? string.Empty,
                        Url = element.GetAttribute("href") ?? string.Empty,
                        Source = "華視",
                        UrlToImage = imageElement?.GetAttribute("data-src") ?? string.Empty
                    };

                    if (timeElement != null)
                    {
                        news.PublishedAt = DateTime.Parse(timeElement.TextContent);
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
            var contentElement = parser.QuerySelector(".artical-content[itemprop=articleBody]");
            if (contentElement == null) return;

            // Get metadata
            news.Author = parser.QuerySelector("head meta[itemprop=author]")?.GetAttribute("content") ?? string.Empty;
            news.Description = parser.QuerySelector("head meta[itemprop=description]")?.GetAttribute("content") ?? string.Empty;

            // Remove unnecessary elements
            var youtubeContainer = contentElement.QuerySelector("[id=yt_container_placeholder]");
            var flexboxElements = contentElement.QuerySelectorAll(".flexbox.cts-tbfs");

            var contentHtml = contentElement.InnerHtml;
            if (youtubeContainer != null)
            {
                contentHtml = RemoveElement(contentHtml, youtubeContainer.InnerHtml);
            }

            foreach (var flexbox in flexboxElements)
            {
                contentHtml = RemoveElement(contentHtml, flexbox.InnerHtml);
            }

            news.ContentBody = StripHtml(contentHtml).Trim();
            news.ContentBodyHtml = contentHtml.Trim();
        }

        private string RemoveElement(string html, string elementHtml)
        {
            return html.Replace(elementHtml, string.Empty);
        }

        private string StripHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
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