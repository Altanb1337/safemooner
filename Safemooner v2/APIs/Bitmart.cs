using System.Collections.Generic;

namespace Safemooner.API
{
    public class Bitmart
    {
        public class Kline
        {
            public string last_price { get; set; }
            public int timestamp { get; set; }
            public string volume { get; set; }
            public string open { get; set; }
            public string close { get; set; }
            public string high { get; set; }
            public string low { get; set; }
        }

        public class Data
        {
            public List<Kline> klines { get; set; }
        }

        public class Price
        {
            public string message { get; set; }
            public int code { get; set; }
            public string trace { get; set; }
            public Data data { get; set; }
        }
    }
}
