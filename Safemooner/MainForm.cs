using MetroFramework.Forms;
using System;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using Safemooner.Properties;

namespace Safemooner
{
    public partial class Safemooner : MetroForm
    {
        public Safemooner()
        {
            InitializeComponent();
        }

        private void Safemooner_Load(object sender, EventArgs e)
        {
            addrBox.Text = Settings.Default.Address;

            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            client.UploadStringCompleted += Client_UploadStringCompleted;
        }


        private WebClient client = new WebClient();
        bool turn = false;
        private void updatePrice_Tick(object sender, EventArgs e)
        {
            if (client.IsBusy)
                return;

            if (turn)
                client.DownloadStringAsync(new Uri("https://api.pancakeswap.info/api/tokens/0x8076c74c5e3f5852037f31ff0093eeb8c8add8d3"));
            else
                client.UploadStringAsync(new Uri("https://bsc-dataseed1.binance.org/"), "{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"eth_call\",\"params\":[{\"data\":\"0x70a08231000000000000000000000000" + addrBox.Text.ToLower().Remove(0, 2) + "\",\"to\":\"0x8076c74c5e3f5852037f31ff0093eeb8c8add8d3\"},\"latest\"]}");
            turn = !turn;
        }

        private double current_price = 0;

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            API api = JsonConvert.DeserializeObject<API>(e.Result);
            string price = api.data.price.Substring(0, 11);
            priceL.Text = $"Current Price: {price}$";
            current_price = double.Parse(price);
        }

        private void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            string balance = e.Result;
            BalanceAPI balanceapi = JsonConvert.DeserializeObject<BalanceAPI>(balance);
            long balanceint = Convert.ToInt64(balanceapi.result, 16) / 10000000; //9 decimals / used 6
            double balance4text = balanceint / 100.0;
            ownedL.Text = $"Balance: {string.Format("{0,0:N2}", balance4text)} $SAFE";

            if (current_price != 0)
            {
                double balancecash = (current_price * balance4text);
                balancecashL.Text = $"Balance in cash: {balancecash.ToString().Substring(0, 5)}$";
            }
        }

        bool toggle = false;
        private void toggleB_Click(object sender, EventArgs e)
        {
            toggle = !toggle;
            toggleB.Text = toggle ? "STOP" : "START";
            updatePrice.Enabled = toggle;
        }

        private void Safemooner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Address = addrBox.Text;
            Settings.Default.Save();
        }
    }
}
