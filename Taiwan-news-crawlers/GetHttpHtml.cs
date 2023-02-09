using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	public static class GetHttpHtml
	{
		public static string GetHtml(string Url) 
		{
			HttpClient httpClient = new HttpClient();

			//發送請求並取得回應內容
			var responseMessage = httpClient.GetAsync(Url).Result;

			//讀取Content內容
			if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
				return responseMessage.Content.ReadAsStringAsync().Result;
			else
				return string.Empty;
		}
	}
}
