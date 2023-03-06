using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	public static class GetHttpClient
    {
		/// <summary>
		/// 取得網頁內文用
		/// </summary>
		/// <param name="Url"></param>
		/// <returns></returns>
		public static async Task <string> GetHtml(string Url) 
		{
			try
			{
                using (HttpClient httpClient = new HttpClient())
                {
                    //發送請求並取得回應內容
                    var responseMessage = httpClient.GetAsync(Url).Result;
                    //讀取Content內容
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var Result = await responseMessage.Content.ReadAsStringAsync();
                        return Result;
                    }
                    else
                        return string.Empty;
                }
            }
			catch 
			{
                return string.Empty;
            }
		
        }

		/// <summary>
		/// 取得API 架構用
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Url"></param>
		/// <returns></returns>
		public static async Task<T?> GetApiJson<T>(string Url)
		{
            T? deserializeObject = default;
            try
			{
                using (HttpClient httpClient = new HttpClient())
                {                  
                    //發送請求並取得回應內容
                    var responseMessage = httpClient.GetAsync(Url).Result;

                    //讀取Content內容
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var Result = await responseMessage.Content.ReadAsStringAsync();
                        deserializeObject = JsonConvert.DeserializeObject<T>(Result);
                        return deserializeObject;
                    }
                    else
                        return deserializeObject;
                }
            }
			catch 
			{
                return deserializeObject;
			}
		
        }
	}
}
