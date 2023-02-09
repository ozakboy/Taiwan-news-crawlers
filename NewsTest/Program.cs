using Taiwan_news_crawlers;
using static Taiwan_news_crawlers.Chinatimes;
using static Taiwan_news_crawlers.Newsltn;

//Chinatimes _chinatimes = new Chinatimes();
//_chinatimes.GetNews(ChinatimesNewsType.Life);

Newsltn _newsltn = new Newsltn();
//_newsltn.GetNews(NewsltnNewsType.local);
_newsltn.GetNews(NewsltnNewsType.society);