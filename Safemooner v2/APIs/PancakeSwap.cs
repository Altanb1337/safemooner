namespace Safemooner.API
{
    public class PancakeSwap
    {
        public class Data
        {
            public string name { get; set; }
            public string symbol { get; set; }
            public string price { get; set; }
            public string price_BNB { get; set; }
        }

        public class PancakeAPI
        {
            public Data data { get; set; }
        }
    }
}
