using System.Text.RegularExpressions;

namespace Taiwan_news_crawlers
{
    /// <summary>
    /// 中央通訊社
    /// </summary>
    public class Cna
    {
        /// <summary>
        /// 中央通訊社網址
        /// </summary>
        private static readonly string DefulUrl = "https://www.cna.com.tw/";

        public async Task<List<News>> GetNews(CnaType cnaType)
        {
            string url = DefulUrl + $"list/{cnaType}.aspx";
            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (string.IsNullOrEmpty(html)) return allNews;

            var parser = new HtmlParser(html);
            var statement = parser.QuerySelector(".statement");
            if (statement == null) return allNews;

            // 處理主要新聞列表
            var mainList = statement.QuerySelector(".mainList");
            if (mainList != null)
            {
                var mainNews = await GetNewsFromList(mainList);
                allNews.AddRange(mainNews);
            }

            // 處理圖片新聞列表
            var imgModuleList = statement.QuerySelector(".mainList.imgModule");
            if (imgModuleList != null)
            {
                var imgNews = await GetNewsFromList(imgModuleList);
                allNews.AddRange(imgNews);
            }

            return allNews;
        }

        private async Task<List<News>> GetNewsFromList(HtmlElement element)
        {
            var newsList = new List<News>();
            var newsItems = element.QuerySelectorAll("li");

            var tasks = newsItems.Select(async item =>
            {
                try
                {
                    var titleElement = item.QuerySelector("span");
                    var linkElement = item.QuerySelector("a");
                    var dateElement = item.QuerySelector(".date");

                    if (titleElement == null || linkElement == null) return null;

                    var news = new News
                    {
                        Title = titleElement.TextContent.Trim(),
                        Url = linkElement.GetAttribute("href") ?? string.Empty,
                        PublishedAt = dateElement != null ?
                            DateTime.Parse(dateElement.TextContent) :
                            DateTime.Now,
                        UrlToImage = string.Empty,
                        Source = "中央通訊社"
                    };

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
            newsList.AddRange(results.Where(n => n != null)!);
            return newsList;
        }

        private async Task FetchNewsContent(News news)
        {
            var html = await GetHttpClient.GetHtml(news.Url);
            if (string.IsNullOrEmpty(html)) return;

            var parser = new HtmlParser(html);
            var centralContent = parser.QuerySelector(".centralContent");
            if (centralContent == null) return;

            var paragraphElement = centralContent.QuerySelector(".paragraph");
            if (paragraphElement == null) return;

            // 移除不需要的元素
            var moreArticles = paragraphElement.QuerySelectorAll(".paragraph.moreArticle.flexhalf");
            var bottomBox = paragraphElement.QuerySelector(".paragraph.bottomBox");

            var contentHtml = paragraphElement.InnerHtml;
            foreach (var article in moreArticles)
            {
                contentHtml = RemoveElement(contentHtml, article.InnerHtml);
            }
            if (bottomBox != null)
            {
                contentHtml = RemoveElement(contentHtml, bottomBox.InnerHtml);
            }

            news.ContentBodyHtml = contentHtml;
            news.ContentBody = StripHtml(contentHtml).Trim();

            // 獲取其他元數據
            var imageElement = parser.QuerySelector(".fullPic img");
            news.UrlToImage = imageElement?.GetAttribute("data-src") ?? string.Empty;

            var authorMeta = parser.QuerySelector("head meta[itemprop=author]");
            news.Author = authorMeta?.GetAttribute("content") ?? string.Empty;

            var descMeta = parser.QuerySelector("head meta[name=description]");
            news.Description = descMeta?.GetAttribute("content") ?? string.Empty;
        }

        private string RemoveElement(string html, string elementHtml)
        {
            return html.Replace(elementHtml, string.Empty);
        }

        private string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }

    public enum CnaType
    {
        /// <summary>
        /// 政治
        /// </summary>
        aipl,
        /// <summary>
        /// 國際
        /// </summary>
        aopl,
        /// <summary>
        /// 兩岸
        /// </summary>
        acn,
        /// <summary>
        /// 產經
        /// </summary>
        aie,
        /// <summary>
        /// 證券
        /// </summary>
        asc,
        /// <summary>
        /// 科技
        /// </summary>
        ait,
        /// <summary>
        /// 生活
        /// </summary>
        ahel,
        /// <summary>
        /// 社會
        /// </summary>
        asoc,
        /// <summary>
        /// 地方
        /// </summary>
        aloc,
        /// <summary>
        /// 文化
        /// </summary>
        acul,
        /// <summary>
        /// 運動
        /// </summary>
        aspt,
        /// <summary>
        /// 娛樂
        /// </summary>
        amov
    }
}