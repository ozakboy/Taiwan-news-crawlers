﻿using Newtonsoft.Json;
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

		/// <summary>
		/// 取得API 架構用
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Url"></param>
		/// <returns></returns>
		public static T? GetApiJson<T>(string Url)
		{
			T? deserializeObject = default;
            HttpClient httpClient = new HttpClient();

            //發送請求並取得回應內容
            var responseMessage = httpClient.GetAsync(Url).Result;

			//讀取Content內容
			if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var Result = responseMessage.Content.ReadAsStringAsync().Result;
				deserializeObject = JsonConvert.DeserializeObject<T>(Result);
				return deserializeObject;
			}
			else
				return deserializeObject;
        }
	}
}
