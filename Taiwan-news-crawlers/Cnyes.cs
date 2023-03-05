using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Taiwan_news_crawlers
{
	/// <summary>
	/// 鉅亨網
	/// </summary>
	public class Cnyes
	{
		public static readonly string CnyesUrl = "https://news.cnyes.com/";
		public static readonly string CnyesApiUrl = "https://api.cnyes.com/";

		public List<News> GetNews(CnyesNewsType _cnyesNewsType)
		{
			List<News> list = new List<News>();

            var StartNow = GetIntTIme(DateTime.Now.AddDays(-3));
			var EndNow = GetIntTIme(DateTime.Now);

			var ApiUrl = $"{CnyesApiUrl}media/api/v1/newslist/category/{_cnyesNewsType}?startAt={StartNow}&endAt={EndNow}&limit=30";

            var CnyesApiRep = GetHttpClient.GetApiJson<VCnyesApiRep>(ApiUrl);
			if (CnyesApiRep is null)
				return list;
            else if (CnyesApiRep.items is null)
                return list;
            else if (CnyesApiRep.items.data is null)
				return list;

            foreach (var OneNews in CnyesApiRep.items.data)
			{
				var CreateNews = new News()
				{
					Title = OneNews.title,
					Description = OneNews.summary,
					Author = OneNews.source ?? string.Empty,
					ContentBody = OneNews.content,
					ContentBodyHtml = OneNews.content,
					PublishedAt = IntTimeToDateTIme(OneNews.publishAt) ,
					Url = $"{CnyesUrl}news/id/{OneNews.newsId}",
					UrlToImage = OneNews.coverSrc?.l.src ?? string.Empty
                };
				list.Add(CreateNews);
			}

			return list;


        }

		/// <summary>
		/// 鉅亨網 新聞分類
		/// </summary>
		public enum CnyesNewsType
		{

            #region 台股

            /// <summary>
            /// 台股全部
            /// </summary>
            tw_stock,
			/// <summary>
			/// 台股盤勢
			/// </summary>
            tw_quo,
			/// <summary>
			/// 台灣政經
			/// </summary>
            tw_macro,
			/// <summary>
			/// 台股新聞
			/// </summary>
            tw_stock_news,

            #endregion

            #region 期貨

            /// <summary>
            /// 期貨全部
            /// </summary>
            future,
			/// <summary>
			/// 期貨指數
			/// </summary>
            index_futures,
			/// <summary>
			/// 期貨股票
			/// </summary>
            stock_futures,

			/// <summary>
			/// 期貨能源
			/// </summary>
            energy,

            /// <summary>
            /// 期貨債券
            /// </summary>
            futu_bond,

            /// <summary>
            /// 期貨農作
            /// </summary>
            futu_produce,

			/// <summary>
			/// 期貨黃金
			/// </summary>
            precious_metals,

            #endregion

            #region 國際股

			/// <summary>
			/// 國際股 全部
			/// </summary>
            wd_stock,

			/// <summary>
			/// 美股
			/// </summary>
            us_stock,

			/// <summary>
			/// 美股雷達
			/// </summary>
            us_stock_live,

			/// <summary>
			/// 歐亞股
			/// </summary>
            eu_asia_stock,

			/// <summary>
			/// 國際政經
			/// </summary>
            wd_macro,

            #endregion

            /// <summary>
            /// 外匯 全部
            /// </summary>
            forex,

			/// <summary>
			/// 區塊鏈 全部
			/// </summary>
            bc,

			/// <summary>
			/// 房產 全部
			/// </summary>
            cnyeshouse,

        }

        #region 轉時間格式

        /// <summary>
        ///  取得鉅亨時間格式
        /// </summary>
        /// <param name="_dateTime"></param>
        /// <returns></returns>
        private long GetIntTIme(DateTime _dateTime)
        {

			// 將 datetime 轉換成 UTC 時間
            DateTime utcDatetime = _dateTime.ToUniversalTime();

            // 計算 UTC datetime 距離 1970 年 1 月 1 日 00:00:00 UTC 的秒數
            long unixTimestamp = (long)(utcDatetime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
			return unixTimestamp;

        }

		/// <summary>
		/// 把數字時間格式轉回時間型態
		/// </summary>
		/// <param name="IntTIme"></param>
		/// <returns></returns>
		private DateTime IntTimeToDateTIme(long IntTIme)
		{
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(IntTIme);
        }

        #endregion

        #region  

        private class VCnyesApiRep
		{	
			public VCnyesApiData items { get; set; }

			public string message { get; set; }
			public int statusCode { get; set; }



        }

		private class VCnyesApiData
		{
            public int current_page { get; set; }
            public int from { get; set; }
            public int last_page { get; set; }
            public int per_page { get; set; }
            public int to { get; set; }
            public int total { get; set; }
            /// <summary>
            /// 新聞資料
            /// </summary>
            public List<VCnyesNews> data { get; set; }
        }



        private class VCnyesNews
		{

			/// <summary>
			/// 內文
			/// </summary>
			public string content { get; set; }
            /// <summary>
            /// 概括
            /// </summary>
            public string summary { get; set; }
			/// <summary>
			/// 標題
			/// </summary>
			public string title { get; set; }
			/// <summary>
			///  圖片連結
			/// </summary>
			public VCnyesNewsCoverSrc? coverSrc { get; set; }
			/// <summary>
			/// 分類
			/// </summary>
			public string categoryName { get; set; }

			/// <summary>
			/// 發布時間
			/// </summary>
			public long publishAt { get; set; }

			/// <summary>
			/// 新聞ID
			/// </summary>
			public long newsId { get; set; }

			public string source { get; set; }

        }

		private class VCnyesNewsCoverSrc
		{

			public VCnyesImg xs { get; set; }
			public VCnyesImg s { get; set; }
			public VCnyesImg m { get; set; }
			public VCnyesImg l { get; set; }
			public VCnyesImg xl { get; set; }
			public VCnyesImg xxl { get; set; }
        }

		public class VCnyesImg
		{
			public string src { get; set; }
			public int width { get; set; }
			public int height { get; set; }

        }

        #endregion
    }

}
