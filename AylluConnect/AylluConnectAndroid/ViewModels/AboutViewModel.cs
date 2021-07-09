using AbaciConnect.Android.Relay;
using AbaciConnect.Relay;
using AbaciConnect.Relay.Compression;
using AbaciConnect.Relay.Processors;
using AbaciConnect.Relay.TransmissionObjects;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AbaciAndroid.ViewModels
{
    public class AboutViewModel : BaseViewModel
    { 
        private IRelay relay = null;
        private RelayController relayController = null;
        private readonly BluetoothRelayFactory relayFactory = null;
        private readonly IEmissionProcessor processor = new MicropyEmissionProcessor();
        public ICommand CommandConnect { get; }
        public ICommand CommandDiscover { get; }
        public ICommand CommandSend{ get; }
        private string _DataOut = "Test";
        public string DataOut
        {
            get
            {
                return this._DataOut;
            }
            set
            {
                this._DataOut = value;
                this.OnPropertyChanged(nameof(DataOut));
            }
        }
        public AboutViewModel()
        {
            this.relayFactory = new BluetoothRelayFactory(this.processor);
            //
            Title = "Bluetooth";
            CommandConnect = new Command(() => this.Connect());
            CommandDiscover = new Command(() => this.Discover());
            CommandSend = new Command(() => this.Send());
        }
        private void Connect()
        {
            try
            {
                this.relay = this.relayFactory.Create("XBee A", "12345");
                this.relayController = new RelayController(this.relay, this.processor, new TransmissionObjectFactory(new NonCompressor()));
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error while connecting to bluetooth ({e.Message})");
            }
        }
        private void Discover()
        {
            this.relayFactory.Discover();
        }
        private void Send()
        {
            try
            {
                byte[] data_bytes = Encoding.UTF8.GetBytes(this.DataOut+"\r\n");
                IEmissionProcessor receiver = new ApiFrameEmissionProcessor();
                ulong long_address = 0x0013A20041B764AD;
                // TODO 06/28/21: determine how to discover via micropython
                //ushort short_address = this.relayController.Discover(long_address);
                this.relayController.Transmit(long_address, data_bytes);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while transmitting data via bluetooth ({e.Message})");
            }
        }
    }
}