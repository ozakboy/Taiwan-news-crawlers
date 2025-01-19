using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	/// <summary>
	/// 鉅亨網
	/// </summary>
	public class Cnyes
	{
        public static readonly string CnyesUrl = "https://news.cnyes.com/";
        public static readonly string CnyesApiUrl = "https://api.cnyes.com/";

        public async Task<List<News>> GetNews(CnyesNewsType newsType)
        {
            var startTime = GetUnixTimestamp(DateTime.Now.AddDays(-3));
            var endTime = GetUnixTimestamp(DateTime.Now);
            var apiUrl = $"{CnyesApiUrl}media/api/v1/newslist/category/{newsType}?startAt={startTime}&endAt={endTime}&limit=30";

            var apiResponse = await GetHttpClient.GetApiJson<ApiResponse>(apiUrl);
            var newsList = new List<News>();

            if (apiResponse?.Items?.Data == null) return newsList;

            foreach (var item in apiResponse.Items.Data)
            {
                var news = new News
                {
                    Title = item.Title,
                    Description = item.Summary,
                    Author = item.Source ?? string.Empty,
                    ContentBody = item.Content,
                    ContentBodyHtml = item.Content,
                    PublishedAt = UnixTimestampToDateTime(item.PublishAt),
                    Url = $"{CnyesUrl}news/id/{item.NewsId}",
                    UrlToImage = item.CoverSrc?.Large.Src ?? string.Empty,
                    Source = "鉅亨網"
                };
                newsList.Add(news);
            }

            return newsList;
        }

        private static long GetUnixTimestamp(DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime.ToUniversalTime()).ToUnixTimeSeconds();
        }

        private static DateTime UnixTimestampToDateTime(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        }

        #region API Response Classes
        private class ApiResponse
        {
            [JsonPropertyName("items")]
            public ApiItems Items { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("statusCode")]
            public int StatusCode { get; set; }
        }

        private class ApiItems
        {
            [JsonPropertyName("current_page")]
            public int CurrentPage { get; set; }

            [JsonPropertyName("from")]
            public int From { get; set; }

            [JsonPropertyName("last_page")]
            public int LastPage { get; set; }

            [JsonPropertyName("per_page")]
            public int PerPage { get; set; }

            [JsonPropertyName("to")]
            public int To { get; set; }

            [JsonPropertyName("total")]
            public int Total { get; set; }

            [JsonPropertyName("data")]
            public List<NewsItem> Data { get; set; }
        }

        private class NewsItem
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("summary")]
            public string Summary { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("coverSrc")]
            public CoverSource CoverSrc { get; set; }

            [JsonPropertyName("categoryName")]
            public string CategoryName { get; set; }

            [JsonPropertyName("publishAt")]
            public long PublishAt { get; set; }

            [JsonPropertyName("newsId")]
            public long NewsId { get; set; }

            [JsonPropertyName("source")]
            public string Source { get; set; }
        }

        private class CoverSource
        {
            [JsonPropertyName("xs")]
            public ImageInfo ExtraSmall { get; set; }

            [JsonPropertyName("s")]
            public ImageInfo Small { get; set; }

            [JsonPropertyName("m")]
            public ImageInfo Medium { get; set; }

            [JsonPropertyName("l")]
            public ImageInfo Large { get; set; }

            [JsonPropertyName("xl")]
            public ImageInfo ExtraLarge { get; set; }

            [JsonPropertyName("xxl")]
            public ImageInfo ExtraExtraLarge { get; set; }
        }

        private class ImageInfo
        {
            [JsonPropertyName("src")]
            public string Src { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }

            [JsonPropertyName("height")]
            public int Height { get; set; }
        }
        #endregion
    }

    /// <summary>
    /// 鉅亨網 新聞分類
    /// </summary>
    public enum CnyesNewsType
    {

        #region 台股

        /// <summary>
        /// 台股全部
        /// </summary>
        tw_stock,
        /// <summary>
        /// 台股盤勢
        /// </summary>
        tw_quo,
        /// <summary>
        /// 台灣政經
        /// </summary>
        tw_macro,
        /// <summary>
        /// 台股新聞
        /// </summary>
        tw_stock_news,

        #endregion

        #region 期貨

        /// <summary>
        /// 期貨全部
        /// </summary>
        future,
        /// <summary>
        /// 期貨指數
        /// </summary>
        index_futures,
        /// <summary>
        /// 期貨股票
        /// </summary>
        stock_futures,

        /// <summary>
        /// 期貨能源
        /// </summary>
        energy,

        /// <summary>
        /// 期貨債券
        /// </summary>
        futu_bond,

        /// <summary>
        /// 期貨農作
        /// </summary>
        futu_produce,

        /// <summary>
        /// 期貨黃金
        /// </summary>
        precious_metals,

        #endregion

        #region 國際股

        /// <summary>
        /// 國際股 全部
        /// </summary>
        wd_stock,

        /// <summary>
        /// 美股
        /// </summary>
        us_stock,

        /// <summary>
        /// 美股雷達
        /// </summary>
        us_stock_live,

        /// <summary>
        /// 歐亞股
        /// </summary>
        eu_asia_stock,

        /// <summary>
        /// 國際政經
        /// </summary>
        wd_macro,

        #endregion

        #region 其他

        /// <summary>
        /// 外匯 全部
        /// </summary>
        forex,

        /// <summary>
        /// 區塊鏈 全部
        /// </summary>
        bc,

        /// <summary>
        /// 房產 全部
        /// </summary>
        cnyeshouse,

        /// <summary>
        /// 消費 全部
        /// </summary>
        spending,

        #endregion

    }

}
