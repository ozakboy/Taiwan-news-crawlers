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
        private static readonly string DefulUrl = "https://news.ltn.com.tw/";
        /// <summary>
        /// 自由時報-自由財經
        /// </summary>
        private static readonly string DefulStrategyUrl = "https://ec.ltn.com.tw/";

        public async Task<List<News>> GetNews(NewsltnNewsType type)
        {
            string url = type == NewsltnNewsType.strategy
                ? $"{DefulStrategyUrl}list/strategy"
                : $"{DefulUrl}list/breakingnews/{type}";

            string html = await GetHttpClient.GetHtml(url);
            var allNews = new List<News>();

            if (string.IsNullOrEmpty(html)) return allNews;

            var parser = new HtmlParser(html);

            if (type == NewsltnNewsType.strategy)
            {
                var strategyList = parser.QuerySelector("div.whitecon.boxTitle.boxText[data-desc=列表]");
                if (strategyList != null)
                {
                    allNews.AddRange(await GetStrategyNews(strategyList));
                }
            }
            else
            {
                var newsList = parser.QuerySelector("ul.list");
                if (newsList != null)
                {
                    allNews.AddRange(await GetRegularNews(newsList));
                }
            }

            return allNews;
        }

        private async Task<List<News>> GetStrategyNews(HtmlElement element)
        {
            var allNews = new List<News>();

            // 處理照片新聞
            var photoNews = element.QuerySelectorAll(".listphoto");
            var photoTasks = photoNews.Select(async news =>
            {
                try
                {
                    var newsItem = new News
                    {
                        Title = news.GetAttribute("title") ?? string.Empty,
                        Url = news.GetAttribute("href") ?? string.Empty,
                        UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                        Source = "自由時報"
                    };

                    var timeElement = news.QuerySelector(".newstime");
                    if (timeElement != null)
                    {
                        newsItem.PublishedAt = DateTime.Parse(timeElement.InnerHtml);
                    }

                    await FetchStrategyNewsContent(newsItem);
                    return newsItem;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing photo news: {ex.Message}");
                    return null;
                }
            });

            // 處理列表新聞
            var listItems = element.QuerySelectorAll("ul[data-desc=列表] li");
            var listTasks = listItems.Select(async news =>
            {
                try
                {
                    var link = news.QuerySelector("a");
                    if (link == null) return null;

                    var newsItem = new News
                    {
                        Title = link.GetAttribute("title") ?? string.Empty,
                        Url = link.GetAttribute("href") ?? string.Empty,
                        UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                        Source = "自由時報"
                    };

                    var timeElement = news.QuerySelector(".newstime");
                    if (timeElement != null)
                    {
                        newsItem.PublishedAt = DateTime.Parse(timeElement.InnerHtml);
                    }

                    await FetchStrategyNewsContent(newsItem);
                    return newsItem;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing list news: {ex.Message}");
                    return null;
                }
            });

            var allTasks = photoTasks.Concat(listTasks);
            var results = await Task.WhenAll(allTasks);
            allNews.AddRange(results.Where(n => n != null && !string.IsNullOrEmpty(n.ContentBodyHtml))!);

            return allNews;
        }

        private async Task<List<News>> GetRegularNews(HtmlElement element)
        {
            var allNews = new List<News>();
            var newsItems = element.QuerySelectorAll("li");

            var tasks = newsItems.Select(async news =>
            {
                try
                {
                    var newsItem = new News
                    {
                        Title = news.QuerySelector(".title")?.TextContent.Trim() ?? string.Empty,
                        Url = news.QuerySelector("a")?.GetAttribute("href") ?? string.Empty,
                        UrlToImage = news.QuerySelector("img")?.GetAttribute("data-src") ?? string.Empty,
                        Source = "自由時報"
                    };

                    await FetchRegularNewsContent(newsItem);
                    return newsItem;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing regular news: {ex.Message}");
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);
            allNews.AddRange(results.Where(n => n != null && !string.IsNullOrEmpty(n.ContentBodyHtml))!);

            return allNews;
        }

        private async Task FetchStrategyNewsContent(News news)
        {
            var html = await GetHttpClient.GetHtml(news.Url);
            if (string.IsNullOrEmpty(html)) return;

            var parser = new HtmlParser(html);
            var contentElement = parser.QuerySelector(".whitecon.boxTitle.boxText[data-desc=內文] .text");
            if (contentElement == null) return;

            // 移除不需要的內容
            var elementsToRemove = new[]
            {
                contentElement.QuerySelector(".before_ir"),
                contentElement.QuerySelector(".after_ir"),
                contentElement.QuerySelector(".appE1121"),
                contentElement.QuerySelector("[id=oneadIRMIRTag]"),
                contentElement.QuerySelector("[id=ad-IR1]"),
                contentElement.QuerySelector("[id=ad-IR2]"),
                contentElement.QuerySelector(".suggest")
            };

            var contentHtml = contentElement.InnerHtml;
            foreach (var element in elementsToRemove.Where(e => e != null))
            {
                contentHtml = RemoveElement(contentHtml, element.InnerHtml);
            }

            // 移除腳本標籤
            var scriptElements = contentElement.QuerySelectorAll("script");
            foreach (var script in scriptElements)
            {
                contentHtml = RemoveElement(contentHtml, script.InnerHtml);
            }

            news.ContentBodyHtml = contentHtml.Trim();
            news.ContentBody = StripHtml(contentHtml).Trim();
            news.Description = parser.QuerySelector("head meta[name=description]")?.GetAttribute("content") ?? string.Empty;
            news.Author = parser.QuerySelector("head meta[name=author]")?.GetAttribute("content") ?? string.Empty;
        }

        private async Task FetchRegularNewsContent(News news)
        {
            var html = await GetHttpClient.GetHtml(news.Url);
            if (string.IsNullOrEmpty(html)) return;

            var parser = new HtmlParser(html);
            var contentElement = parser.QuerySelector(".text.boxTitle.boxText");
            if (contentElement == null) return;

            var timeElement = contentElement.QuerySelector(".time");
            if (timeElement != null)
            {
                news.PublishedAt = DateTime.Parse(timeElement.TextContent);
            }

            // 移除不需要的內容
            var elementsToRemove = new[]
            {
                contentElement.QuerySelector(".photo"),
                contentElement.QuerySelector(".time"),
                contentElement.QuerySelector(".before_ir"),
                contentElement.QuerySelector("[id=oneadIRMIRTag]"),
                contentElement.QuerySelector("[id=ad-IR1]"),
                contentElement.QuerySelector(".suggest"),
                contentElement.QuerySelector(".appE1121")
            };

            var contentHtml = contentElement.InnerHtml;
            foreach (var element in elementsToRemove.Where(e => e != null))
            {
                contentHtml = RemoveElement(contentHtml, element.InnerHtml);
            }

            // 移除腳本標籤
            var scriptElements = contentElement.QuerySelectorAll("script");
            foreach (var script in scriptElements)
            {
                contentHtml = RemoveElement(contentHtml, script.InnerHtml);
            }

            news.ContentBodyHtml = contentHtml.Trim();
            news.ContentBody = StripHtml(contentHtml).Trim();
            news.Description = parser.QuerySelector("head meta[name=description]")?.GetAttribute("content") ?? string.Empty;
            news.Author = parser.QuerySelector("head meta[name=author]")?.GetAttribute("content") ?? string.Empty;
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
