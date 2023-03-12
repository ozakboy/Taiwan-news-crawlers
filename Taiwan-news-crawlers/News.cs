using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	public class News
	{
		/// <summary>
		/// 標題
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 內文
		/// </summary>
		public string ContentBody { get; set; }
		/// <summary>
		/// 內文(含HTML)
		/// </summary>
		public string ContentBodyHtml { get; set; }
		/// <summary>
		/// 作者
		/// </summary>
		public string Author { get; set; }
		/// <summary>
		/// 備註
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 原始連結
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// 圖片連結
		/// </summary>
		public string UrlToImage { get; set; }
		/// <summary>
		/// 來源
		/// </summary>
		public string Source { get; set; }
		/// <summary>
		/// 發布時間
		/// </summary>
		public DateTime PublishedAt { get; set; }
	}
}
