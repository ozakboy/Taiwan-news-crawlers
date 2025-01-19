using System.Text.Json;
using System.Text;

namespace Taiwan_news_crawlers
{
    public static class GetHttpClient
    {
        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        static GetHttpClient()
        {
            // 設置默認請求頭，模擬瀏覽器行為
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
        }

        /// <summary>
        /// 取得網頁內容
        /// </summary>
        public static async Task<string> GetHtml(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // 檢測編碼
                    var contentType = response.Content.Headers.ContentType?.CharSet;
                    var encoding = !string.IsNullOrEmpty(contentType)
                        ? Encoding.GetEncoding(contentType)
                        : Encoding.UTF8;

                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return encoding.GetString(bytes);
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 取得API JSON響應
        /// </summary>
        public static async Task<T?> GetApiJson<T>(string url) where T : class
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    return JsonSerializer.Deserialize<T>(jsonString, options);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}