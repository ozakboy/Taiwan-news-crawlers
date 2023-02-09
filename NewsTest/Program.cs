using Taiwan_news_crawlers;
using static Taiwan_news_crawlers.Chinatimes;

Chinatimes _chinatimes = new Chinatimes();
_chinatimes.GetNews(NewsType.Life);