using Taiwan_news_crawlers;
using ozakboy.NLOG;

var i = 1;

LOG.Info_Log($"=========Chinatimes============");
Chinatimes _chinatimes = new Chinatimes();
List<ChinatimesNewsType> _chinatimesNewsTypes = new List<ChinatimesNewsType>()
{
    ChinatimesNewsType.Politics,
    ChinatimesNewsType.Life,
    ChinatimesNewsType.Society,
    ChinatimesNewsType.Entertainment,
    ChinatimesNewsType.Physical_Education,
    ChinatimesNewsType.Financial,
    ChinatimesNewsType.Internationality,
    ChinatimesNewsType.Both_Sides_of_The_Strait,
    ChinatimesNewsType.Technology,
    ChinatimesNewsType.Military,
    ChinatimesNewsType.Healthy,
};
foreach (var item in _chinatimesNewsTypes)
{
    var list = await _chinatimes.GetNews(item);
    LOG.Info_Log($"分類:{item}");
    foreach (var c in list)
    {
        LOG.Info_Log($"{i}.{c.Title}");
        i++;
    }
    i = 1;
}

LOG.Info_Log($"=========Chinatimes============");

i = 1;

LOG.Info_Log($"=========Newsltn============");

Newsltn _newsltn = new Newsltn();

List<NewsltnNewsType> _newsltnNewsTypes = new List<NewsltnNewsType>()
{
    NewsltnNewsType.politics,
    NewsltnNewsType.society,
    NewsltnNewsType.life,
    NewsltnNewsType.world,
    NewsltnNewsType.local,
    NewsltnNewsType.novelty,
    NewsltnNewsType.strategy,
};
foreach (var item in _newsltnNewsTypes)
{
    var list = await _newsltn.GetNews(item);
    LOG.Info_Log($"分類:{item}");
    foreach (var c in list)
    {
        LOG.Info_Log($"{i}.{c.Title}");
        i++;
    }
    i = 1;
}

LOG.Info_Log($"=========Newsltn============");


LOG.Info_Log($"=========Cnyes============");
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

i = 1;
foreach (var _CnyesNewsType in CnyesNewsTypeAll)
{
    LOG.Info_Log($"分類:{_CnyesNewsType}");
    var data = await _cnyes.GetNews(_CnyesNewsType);
  
    foreach (var item in data)
    {
        LOG.Info_Log($"{i}.{item.Title}");
        i++;
    }
    i = 1;
}

LOG.Info_Log($"=========Cnyes============");