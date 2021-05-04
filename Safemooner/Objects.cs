namespace Safemooner
{
    public class Data
    {
        public string name { get; set; }
        public string symbol { get; set; }
        public string price { get; set; }
        public string price_BNB { get; set; }
    }

    public class API
    {
        public Data data { get; set; }
    }
    public class BalanceAPI
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public string result { get; set; }
    }


}
