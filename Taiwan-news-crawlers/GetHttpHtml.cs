using System.Text.Json;
using System.Text;

namespace Taiwan_news_crawlers
{
    public static class GetHttpClient
    {
        private static readonly ProxyService _proxyService = new();

        /// <summary>
        /// 取得網頁內容
        /// </summary>
        public static async Task<string> GetHtml(string url, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                using var client = _proxyService.GetHttpClient();
                try
                {
                    await DelayRequest();
                    using var response = await client.GetAsync(url);
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

                    if ((int)response.StatusCode == 403)
                    {
                        await Task.Delay(5000 * (i + 1)); // 逐次增加等待時間
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on try {i + 1}: {ex.Message}");
                    if (i == maxRetries - 1) return string.Empty;
                    await Task.Delay(2000 * (i + 1));
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 取得API JSON響應
        /// </summary>
        public static async Task<T?> GetApiJson<T>(string url) where T : class
        {
            using var client = _proxyService.GetHttpClient();
            try
            {
                using var response = await client.GetAsync(url);
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

        private static async Task DelayRequest()
        {
            Random rand = new Random();
            int delay = rand.Next(1000, 10000); // 1-10秒隨機延遲
            await Task.Delay(delay);
        }
    }
}