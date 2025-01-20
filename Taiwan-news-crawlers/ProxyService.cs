using System.Net;

namespace Taiwan_news_crawlers
{
    public class ProxyService
    {
        private readonly List<ProxyInfo> _proxyList;
        private int _currentIndex = 0;
        private readonly object _lock = new();

        public ProxyService()
        {
            // 這裡添加代理伺服器清單
            // 建議從配置檔案或外部服務獲取
            _proxyList = new List<ProxyInfo>
            {
                new ProxyInfo("219.87.79.144", 3128),
                new ProxyInfo("211.75.95.66", 80),
                new ProxyInfo("210.65.248.181", 80),
                new ProxyInfo("122.116.125.115", 8888),
                new ProxyInfo("211.75.95.66", 3128),
                new ProxyInfo("210.65.248.181", 3128),
                new ProxyInfo("114.35.140.157", 8080),
                new ProxyInfo("182.155.254.159", 80),
                new ProxyInfo("211.78.63.115", 80),
                new ProxyInfo("34.81.160.132", 80),
                new ProxyInfo("140.110.160.130", 3128) {Type ="https"},
                new ProxyInfo("125.229.149.168", 65110),
                new ProxyInfo("140.110.160.38", 3128),
                new ProxyInfo("210.201.86.72", 8080),
                new ProxyInfo("220.132.41.160", 1088) {Type = "https"},
              //  new ProxyInfo("", 8888),
            };
        }

        public HttpClient GetHttpClient()
        {
            var proxy = GetNextProxy();
            var handler = new HttpClientHandler
            {
                Proxy = CreateWebProxy(proxy),
                UseProxy = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // 設置請求標頭
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");

            return client;
        }

        private ProxyInfo GetNextProxy()
        {
            lock (_lock)
            {
                if (_currentIndex >= _proxyList.Count)
                {
                    _currentIndex = 0;
                }
                return _proxyList[_currentIndex++];
            }
        }

        private IWebProxy CreateWebProxy(ProxyInfo proxyInfo)
        {
            var proxy = new WebProxy(proxyInfo.Host, proxyInfo.Port);

            if (!string.IsNullOrEmpty(proxyInfo.Username) && !string.IsNullOrEmpty(proxyInfo.Password))
            {
                proxy.Credentials = new NetworkCredential(proxyInfo.Username, proxyInfo.Password);
            }

            return proxy;
        }

        private string GetRandomUserAgent()
        {
            var userAgents = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Edge/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0"
            };

            return userAgents[Random.Shared.Next(userAgents.Length)];
        }
    }
}