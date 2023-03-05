using Taiwan_news_crawlers;
using static Taiwan_news_crawlers.Chinatimes;
using static Taiwan_news_crawlers.Cnyes;
using static Taiwan_news_crawlers.Newsltn;

//Chinatimes _chinatimes = new Chinatimes();
//_chinatimes.GetNews(ChinatimesNewsType.Life);

//Newsltn _newsltn = new Newsltn();
//_newsltn.GetNews(NewsltnNewsType.local);
//_newsltn.GetNews(NewsltnNewsType.society);


Cnyes _cnyes = new Cnyes();
List<CnyesNewsType> CnyesNewsTypeAll = new List<CnyesNewsType>()
{
    CnyesNewsType.tw_stock ,
    CnyesNewsType.tw_quo ,
    CnyesNewsType.tw_macro ,
    CnyesNewsType.tw_stock_news ,
    CnyesNewsType.future ,
    CnyesNewsType.index_futures ,
    CnyesNewsType.stock_futures ,
    CnyesNewsType.energy ,
    CnyesNewsType.futu_bond ,
    CnyesNewsType.futu_produce ,
    CnyesNewsType.precious_metals ,
    CnyesNewsType.forex ,
    CnyesNewsType.bc ,
    CnyesNewsType.cnyeshouse ,
    CnyesNewsType.wd_stock ,
    CnyesNewsType.us_stock ,
    CnyesNewsType.us_stock_live ,
    CnyesNewsType.eu_asia_stock ,
    CnyesNewsType.wd_macro ,
};
foreach (var _CnyesNewsType in CnyesNewsTypeAll)
{
    Console.WriteLine($"分類:{_CnyesNewsType}");
    Console.WriteLine($"");
    var data = _cnyes.GetNews(_CnyesNewsType);
    var i = 1;
    foreach (var item in data)
    {
        Console.WriteLine($"{i}.{item.Title}");
        i++;
    }
    Console.WriteLine($"");
}

