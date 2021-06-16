using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using AbaciConnect.Relay.Processors;

namespace AbaciConnect.Android.Relay
{
    internal class BluetoothRelayFactory
    {
        private readonly IEmissionProcessor processor;
        private readonly IAdapter adapter;
        private readonly List<IDevice> devices = new List<IDevice>();
        public BluetoothRelayFactory(IEmissionProcessor processor_in)
        {
            this.processor = processor_in;
            this.adapter = CrossBluetoothLE.Current.Adapter;
            this.adapter.DeviceAdvertised += Adapter_DeviceAdvertised;
            this.adapter.DeviceConnected += Adapter_DeviceConnected;
            this.adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
            this.adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            this.adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
        }


        public BluetoothRelay Create(string name, string password)
        {
            IDevice device = this.devices.FirstOrDefault(d => d.Name == name);
            if(device == null)
                throw new Exception($"Error while creating bluetooth relay using name '{name}'");
            return new BluetoothRelay(device, password, this.processor);
        }

        public void Discover()
        {
            this.devices.Clear();
            this.devices.AddRange(this.adapter.GetSystemConnectedOrPairedDevices());
            this.adapter.ScanMode = ScanMode.LowLatency;
            this.adapter.ScanTimeout = 20000;
            CancellationTokenSource source = new CancellationTokenSource();
            Guid[] guids = new Guid[3]
            {
                Guid.Parse("53da53b9-0447-425a-b9ea-9837505eb59a"),
                Guid.Parse("7dddca00-3e05-4651-9254-44074792c590"),
                Guid.Parse("f9279ee9-2cd0-410c-81cc-adf11e4e5aea")
            };
            this.adapter.StartScanningForDevicesAsync(serviceUuids: guids, allowDuplicatesKey: true, cancellationToken: source.Token);
        }

        private void Adapter_DeviceAdvertised(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"Bluetooth device advertised: {e.Device.Name}");
            if(!this.devices.Any(d => d.Name == e.Device.Name))
                this.devices.Add(e.Device);
        }
        private void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"Bluetooth device connected: {e.Device.Name}");
        }
        private void Adapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            Debug.WriteLine($"Bluetooth device connection lost: {e.Device.Name}");
            if (this.devices.Any(d => d.Name == e.Device.Name))
                this.devices.RemoveAll(d => d.Name == e.Device.Name);
        }
        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"Bluetooth device discovered: {e.Device.Name}");
            if (!this.devices.Any(d => d.Name == e.Device.Name))
                this.devices.Add(e.Device);
        }
        private void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"Bluetooth device disconnected: {e.Device.Name}");
        }
    }
}
