namespace Taiwan_news_crawlers
{
    public class ProxyInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string Type { get; set; } = "http"; 

        public ProxyInfo(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public ProxyInfo(string host, int port, string username, string password) : this(host, port)
        {
            Username = username;
            Password = password;
        }
    }
}