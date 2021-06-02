using AbaciConnect.Android.Relay;
using AbaciConnect.Relay;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AbaciAndroid.ViewModels
{
    public class AboutViewModel : BaseViewModel
    { 
        private BluetoothRelay bluetoothRelay = new BluetoothRelay();
        public ICommand CommandDiscover { get; }
        public AboutViewModel()
        {
            Title = "Bluetooth";
            CommandDiscover = new Command(()  => this.Discover());
        }
        private void Discover()
        {
            this.bluetoothRelay.Discover();
        }
    }
}