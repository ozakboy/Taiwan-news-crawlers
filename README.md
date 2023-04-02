[![nuget](https://img.shields.io/badge/nuget-Taiwan--ews--crawlers-blue)](https://www.nuget.org/packages/Taiwan-news-crawlers)

# Taiwan-news-crawlers

台灣新聞爬蟲

## 簡介

利用 C# .net 6 寫的的新聞爬蟲函式庫 

自用、長期維護、緩慢更新


## 開發環境

.net 6

## 使用到的其他套件

1. Newtonsoft.Json
2. AngleSharp

## 目前支援爬蟲的新聞網站

1. 中時新聞網
2. 鉅亨網
3. 自由時報網址
4. 中央通訊社
5. Setn三立新聞網
6. 工商時報
7. 華視

## 輸出格式
| key | value |
| :--- | :--- |
| Title | 標題|
| ContentBody | 內文|
| ContentBodyHtml | 內文(含HTML) |
| Author | 作者 |
| Description | 備註 |
| Url | 原始連結 |
| UrlToImage | 圖片連結 |
| Source | 新聞來源 |
| PublishedAt | 發布時間 |

## License

The MIT License