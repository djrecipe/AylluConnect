using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    [DeploymentItem("SimpleImage.bmp")]
    public class OneWaySerialRelayTests
    {
        private readonly CommandBytesFactory factory = new CommandBytesFactory();
        [TestMethod]
        public void OpenAndClose()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
            }
        }
        [TestMethod]
        public void SetName()
        {
            byte[] bytes = factory.SetName("XBee A");
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                relay.SendBytes(bytes);
            }
        }
        [TestMethod]
        public void SetNetworkSettings()
        {
            string port = "COM4";
            NetworkSettings settings = new NetworkSettings();
            settings.Role = DeviceRoles.Create;
            byte[] bytes = factory.SetNetworkSettings(settings);
            using (SerialRelay relay = new SerialRelay(port))
            {
                relay.SendBytes(bytes);
            }
        }
        [TestMethod]
        public void SendEasyData()
        {
            ulong broadcast = 0x000000000000FFFF;
            ulong address = 0x0013A20041B764AD;
            string text = "xxxxx";
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.SendData(address, data_bytes);
            int response_size = 11;
            TransmissionResponse response = new TransmissionResponse();
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                for(int i=0; i<10000; i++)
                {
                    relay.SendBytes(bytes);
                    List<byte> response_bytes = relay.WaitForBytes(response_size);
                    IntPtr ptr = Marshal.AllocHGlobal(response_size);
                    Marshal.Copy(response_bytes.ToArray(), 0, ptr, response_size);
                    response = (TransmissionResponse)Marshal.PtrToStructure(ptr, typeof(TransmissionResponse));
                    Marshal.FreeHGlobal(ptr);
                    Console.WriteLine("Frame ID: {0}; Result: {1}", response.FrameID, response.DeliveryStatus);
                    System.Threading.Thread.Sleep(10);
                }
            }
        }
    }
}
