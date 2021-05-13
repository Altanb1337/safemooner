using Newtonsoft.Json;
using Safemooner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Token_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            TokenAddress.Text = Properties.Settings.Default.Address;
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        public WebClient client = new WebClient();
        public double balancecash;
        public double balancecashv2;

        private void Update()
        {
            Dispatcher.Invoke(() =>
            {
                if (TokenAddress.Text == string.Empty)
                    TokenAddress.Text = "0xAB5c79cE86863794A368C1b27C7DBE2DBeF474B3";

                string pancakeprice = GetPancakePrice();
                string pancakev2price = GetPancakePriceV2();
                long bscBalance = BscScanBalance();
                string[] bitmartprice = GetBitmartPrice();

                Balance.Content = $"Balance: {string.Format("{0,0:N2}", bscBalance)}";

                balancecash = bscBalance * double.Parse(pancakeprice);
                balancecashv2 = bscBalance * double.Parse(pancakev2price);
                BalanceInCash.Content = $"Balance ($): {balancecash.ToString().Substring(0, 6)}$ | V2: {balancecashv2.ToString().Substring(0, 6)}$";
                PancakePrice.Content = $"Price: {pancakeprice.Substring(0, 11)} | V2: {pancakev2price.Substring(0, 11)}";

                BitmartPrice.Content = $"Price: {bitmartprice[0]}$ | Vol: {bitmartprice[1]}";
            });
        }

        private long BscScanBalance()
        {
            string balance = client.UploadString("https://bsc-dataseed1.binance.org/", "{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"eth_call\",\"params\":[{\"data\":\"0x70a08231000000000000000000000000" + TokenAddress.Text.ToLower().Remove(0, 2) + "\",\"to\":\"0x8076c74c5e3f5852037f31ff0093eeb8c8add8d3\"},\"latest\"]}");
            BscScan.BalanceAPI balanceapi = JsonConvert.DeserializeObject<BscScan.BalanceAPI>(balance);
            long balanceint = Convert.ToInt64(balanceapi.result, 16) / 1000000000;
            return balanceint;
        }

        private string[] GetBitmartPrice()
        {
            long to = DateTimeOffset.Now.ToUnixTimeSeconds();
            long from = to - 10;

            string bitmartprice = client.DownloadString($"https://api-cloud.bitmart.com/spot/v1/symbols/kline?symbol=SAFEMOON_USDT&step=15&from={from}&to={to}");
            Bitmart.Price api = JsonConvert.DeserializeObject<Bitmart.Price>(bitmartprice);
            string price = api.data.klines[0].last_price;
            string volume = string.Format("{0,0:N2}", api.data.klines[0].volume.Replace(".0000000000", null));

            return new string[] { price, volume };
        }

        private string GetPancakePrice()
        {
            string data = client.DownloadString("https://api.pancakeswap.info/api/tokens/0x8076c74c5e3f5852037f31ff0093eeb8c8add8d3");
            PancakeSwap.PancakeAPI pancake = JsonConvert.DeserializeObject<PancakeSwap.PancakeAPI>(data);
            return pancake.data.price;
        }

        private string GetPancakePriceV2()
        {
            string data = client.DownloadString("https://api.pancakeswap.info/api/v2/tokens/0x8076c74c5e3f5852037f31ff0093eeb8c8add8d3");
            PancakeSwap.PancakeAPI pancake = JsonConvert.DeserializeObject<PancakeSwap.PancakeAPI>(data);
            return pancake.data.price;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TokenAddress.Text.Length > 5)
                Update();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Address = TokenAddress.Text;
            Properties.Settings.Default.Save();
        }
    }
}
