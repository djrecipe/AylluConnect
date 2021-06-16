using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AbaciConnect.Android.Relay
{
    internal class BluetoothRelayFactory
    {
        private readonly IAdapter adapter;
        private readonly List<IDevice> devices = new List<IDevice>();
        public BluetoothRelayFactory()
        {
            this.adapter = CrossBluetoothLE.Current.Adapter;
            this.adapter.DeviceAdvertised += Adapter_DeviceAdvertised;
            this.adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
        }

        public BluetoothRelay Create(string name, string password)
        {
            IDevice device = this.devices.FirstOrDefault(d => d.Name == name);
            if(device == null)
                throw new Exception($"Error while creating bluetooth relay using name '{name}'");
            return new BluetoothRelay(device, password);
        }

        public void Discover()
        {
            this.devices.Clear();
            this.devices.AddRange(this.adapter.GetSystemConnectedOrPairedDevices());
            this.adapter.StartScanningForDevicesAsync();
        }

        private void Adapter_DeviceAdvertised(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if(!this.devices.Any(d => d.Name == e.Device.Name))
                this.devices.Add(e.Device);
        }
        private void Adapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            if (this.devices.Any(d => d.Name == e.Device.Name))
                this.devices.RemoveAll(d => d.Name == e.Device.Name);
        }
    }
}
