using AbaciConnect.Relay;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Android.Relay
{
    public class BluetoothRelay : IRelay
    {
        private readonly IAdapter bleAdapter;
        private readonly IBluetoothLE ble;
        private readonly List<IDevice> devices = new List<IDevice>();
        public BluetoothRelay()
        {
            this.ble = CrossBluetoothLE.Current;
            this.ble.StateChanged += Bluetooth_StateChanged;
            this.bleAdapter = CrossBluetoothLE.Current.Adapter;
            this.bleAdapter.DeviceDiscovered += Bluetooth_DeviceDiscovered;
            this.devices.AddRange(this.bleAdapter.GetSystemConnectedOrPairedDevices());
            foreach(IDevice device in this.devices)
                Debug.WriteLine($"Discovered {device.Name} ({device.Id})");
        }


        public void ClearBuffer()
        {
        }
        public void Discover()
        {
            this.bleAdapter.ScanMode = ScanMode.LowLatency;
            this.bleAdapter.ScanTimeout = 5000;
            this.bleAdapter.StartScanningForDevicesAsync();
        }
        public void Dispose()
        {
        }

        public void SendBytes(byte[] data)
        {
        }
        private void Bluetooth_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"Discovered {e.Device.Name} ({e.Device.Id})");
        }
        private void Bluetooth_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            Debug.WriteLine($"Bluetooth state changed to {e.NewState}");
        }
    }
}
