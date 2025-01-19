using System.Text.RegularExpressions;
using System.Web;

namespace Taiwan_news_crawlers
{
    /// <summary>
    /// HTML解析器
    /// </summary>
    public class HtmlParser
    {
        private readonly string _html;

        public HtmlParser(string html)
        {
            _html = html;
        }

        /// <summary>
        /// 查找指定CSS選擇器的第一個元素
        /// </summary>
        public HtmlElement? QuerySelector(string selector)
        {
            // 解析選擇器
            var (tag, id, classes, attributes) = ParseSelector(selector);

            // 使用正則表達式匹配HTML
            var pattern = BuildRegexPattern(tag, id, classes, attributes);
            var match = Regex.Match(_html, pattern, RegexOptions.Singleline);

            return match.Success ? new HtmlElement(match.Value) : null;
        }

        /// <summary>
        /// 查找指定CSS選擇器的所有元素
        /// </summary>
        public IEnumerable<HtmlElement> QuerySelectorAll(string selector)
        {
            var (tag, id, classes, attributes) = ParseSelector(selector);
            var pattern = BuildRegexPattern(tag, id, classes, attributes);

            foreach (Match match in Regex.Matches(_html, pattern, RegexOptions.Singleline))
            {
                yield return new HtmlElement(match.Value);
            }
        }

        private (string tag, string id, List<string> classes, Dictionary<string, string> attributes) ParseSelector(string selector)
        {
            var tag = string.Empty;
            var id = string.Empty;
            var classes = new List<string>();
            var attributes = new Dictionary<string, string>();

            // 解析基本選擇器
            var parts = selector.Split(new[] { '[', ']', '.', '#' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                tag = parts[0].ToLower();
            }

            // 解析ID
            if (selector.Contains('#'))
            {
                var idMatch = Regex.Match(selector, @"#([^\s\[\]\.#]+)");
                if (idMatch.Success)
                {
                    id = idMatch.Groups[1].Value;
                }
            }

            // 解析類
            var classMatches = Regex.Matches(selector, @"\.([^\s\[\]\.#]+)");
            foreach (Match match in classMatches)
            {
                classes.Add(match.Groups[1].Value);
            }

            // 解析屬性
            var attrMatches = Regex.Matches(selector, @"\[([^\]]+)\]");
            foreach (Match match in attrMatches)
            {
                var attr = match.Groups[1].Value;
                var attrParts = attr.Split('=');
                if (attrParts.Length == 2)
                {
                    attributes[attrParts[0].Trim()] = attrParts[1].Trim().Trim('\"', '\'');
                }
            }

            return (tag, id, classes, attributes);
        }

        private string BuildRegexPattern(string tag, string id, List<string> classes, Dictionary<string, string> attributes)
        {
            var pattern = $@"<{tag}[^>]*";

            if (!string.IsNullOrEmpty(id))
            {
                pattern += $@"\s+id=(['""]?){id}\1[^>]*";
            }

            foreach (var className in classes)
            {
                pattern += $@"\s+class=(['""]?)[^'""]*(^|\s){className}(\s|$)[^'""]* *\1[^>]*";
            }

            foreach (var attr in attributes)
            {
                pattern += $@"\s+{attr.Key}=(['""]?){attr.Value}\1[^>]*";
            }

            pattern += @">[^<]*(?:</[^>]*>)?";
            return pattern;
        }
    }

    /// <summary>
    /// HTML元素
    /// </summary>
    public class HtmlElement
    {
        private readonly string _elementHtml;
        private readonly HtmlParser _parser;

        public HtmlElement(string elementHtml)
        {
            _elementHtml = elementHtml;
            _parser = new HtmlParser(elementHtml);
        }

        /// <summary>
        /// 獲取元素的文本內容
        /// </summary>
        public string TextContent => HttpUtility.HtmlDecode(
            Regex.Replace(
                Regex.Replace(_elementHtml, @"<[^>]+>", string.Empty),
                @"\s+", " "
            ).Trim()
        );

        /// <summary>
        /// 獲取元素的HTML內容
        /// </summary>
        public string InnerHtml
        {
            get
            {
                var match = Regex.Match(_elementHtml, @">(.*)</[^>]*>", RegexOptions.Singleline);
                return match.Success ? HttpUtility.HtmlDecode(match.Groups[1].Value.Trim()) : string.Empty;
            }
        }

        /// <summary>
        /// 獲取屬性值
        /// </summary>
        public string? GetAttribute(string name)
        {
            var match = Regex.Match(_elementHtml, $@"{name}=(['""]?)([^'""]*)\1");
            return match.Success ? HttpUtility.HtmlDecode(match.Groups[2].Value) : null;
        }

        /// <summary>
        /// 查找子元素
        /// </summary>
        public HtmlElement? QuerySelector(string selector) => _parser.QuerySelector(selector);

        /// <summary>
        /// 查找所有符合的子元素
        /// </summary>
        public IEnumerable<HtmlElement> QuerySelectorAll(string selector) => _parser.QuerySelectorAll(selector);
    }
}