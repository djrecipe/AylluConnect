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
using AbaciConnect.Relay.Processors;

namespace AbaciConnect.Android.Relay
{
    public class BluetoothRelay : IRelay
    {
        private readonly XBeeBLEDevice device = null;
        private readonly IEmissionProcessor receiver;
        internal BluetoothRelay(IDevice device_in, string password, IEmissionProcessor receiver_in)
        {
            this.receiver = receiver_in;
            this.device = new XBeeBLEDevice(device_in, null);
            this.device.PacketReceived += Device_PacketReceived;
            this.device.SerialDataReceived += Device_SerialDataReceived;
            this.device.UserDataRelayReceived += Device_UserDataRelayReceived;
            this.device.ReceiveTimeout = 1000;
            this.device.SetBluetoothPassword(password);
            this.device.Open();
            this.device.EnableBluetooth();
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
            if(!this.device.IsOpen)
                throw new Exception("Error while sending data via bluetooth: device not open");
            if(!this.device.IsInitialized)
                throw new Exception("Error while sending data via bluetooth: device not initialized");
            XBeePacket packet = XBeePacket.ParsePacket(data, XBeeLibrary.Core.Models.OperatingMode.API); 
            this.device.SendPacketAsync(packet);
        }
        private void Device_PacketReceived(object sender, XBeeLibrary.Core.Events.PacketReceivedEventArgs e)
        {
            Debug.WriteLine("Bluetooth user packet received");
            byte[] bytes = e.ReceivedPacket.GenerateByteArray();
            // 
            this.receiver.ReceiveBytes(bytes.ToList());
        }
        private void Device_SerialDataReceived(object sender, XBeeLibrary.Core.Events.Relay.SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine("Bluetooth serial data received");
            byte[] bytes = e.Data;
            // 
            this.receiver.ReceiveBytes(bytes.ToList());
        }
        private void Device_UserDataRelayReceived(object sender, XBeeLibrary.Core.Events.UserDataRelayReceivedEventArgs e)
        {
            Debug.WriteLine("Bluetooth user data relay received");
            byte[] bytes = e.UserDataRelayMessage.Data;
            // 
            this.receiver.ReceiveBytes(bytes.ToList());
        }
    }
}
