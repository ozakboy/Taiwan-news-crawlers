using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	public class News
	{
		public string Title { get; set; }
		public string ContentBody { get; set; }
		public string ContentBodyHtml { get; set; }
		public string Author { get; set; }
		public string Description { get; set; }
		public string Url { get; set; }
		public string UrlToImage { get; set; }
		public string PublishedAt { get; set; }
	}
}
