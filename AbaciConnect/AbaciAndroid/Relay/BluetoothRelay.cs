using AbaciConnect.Relay;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using XBeeLibrary.Xamarin;
using XBeeLibrary.Core.Packet;

namespace AbaciConnect.Android.Relay
{
    public class BluetoothRelay : IRelay
    {
        private readonly XBeeBLEDevice device = null;
        internal BluetoothRelay(IDevice device_in, string password)
        {
            this.device = new XBeeBLEDevice(device_in, null);
            this.device.SetBluetoothPassword(password);
            this.device.Open();
            this.device.PacketReceived += Device_PacketReceived;
            this.device.UserDataRelayReceived += Device_UserDataRelayReceived;
            return;
        }


        public void ClearBuffer()
        {
        }
        public void Dispose()
        {
            if(this.device.IsOpen)
                this.device.Close();
        }

        public void SendBytes(byte[] data)
        {
            XBeePacket packet = XBeePacket.ParsePacket(data, XBeeLibrary.Core.Models.OperatingMode.API); 
            this.device.SendPacket(packet);
        }
        private void Device_PacketReceived(object sender, XBeeLibrary.Core.Events.PacketReceivedEventArgs e)
        {
            // 
        }
        private void Device_UserDataRelayReceived(object sender, XBeeLibrary.Core.Events.UserDataRelayReceivedEventArgs e)
        {
        }
    }
}
