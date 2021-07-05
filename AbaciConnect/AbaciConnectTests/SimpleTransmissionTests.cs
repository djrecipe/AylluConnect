using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using AbaciConnect.Relay;
using AbaciConnect.Relay.Emission;
using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Processors;
using AbaciConnect.Relay.Frames;

namespace AbaciConnectTests
{
    [TestClass]
    [DeploymentItem("SimpleImage.bmp")]
    public class SimpleTransmissionTests
    {
        private readonly CommandBytesFactory factory = new CommandBytesFactory(new FrameBytesFactory());
        private readonly ApiFrameEmissionProcessor byteReceiver = new ApiFrameEmissionProcessor();
        [TestMethod]
        public void OpenAndClose()
        {
            using (SerialRelay relay = new SerialRelay("COM4", this.byteReceiver))
            {
            }
        }
        [TestMethod]
        public void SetName()
        {
            byte[] bytes = factory.CreateSetNameFrame("XBee A");
            using (SerialRelay relay = new SerialRelay("COM4", this.byteReceiver))
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
            byte[] bytes = factory.CreateSetNetworkSettingsFrame(settings);
            using (SerialRelay relay = new SerialRelay(port, this.byteReceiver))
            {
                relay.SendBytes(bytes);
            }
        }
        [TestMethod]
        public void SendSimpleData()
        {
            ulong broadcast = 0x000000000000FFFF;
            ulong address = 0x0013A20041B764AD;
            string text = "xxxxx";
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.CreateSendDataFrame(address, data_bytes, 1);
            using (SerialRelay relay = new SerialRelay("COM4", this.byteReceiver))
            {
                relay.SendBytes(bytes);
                EmissionDescriptor em = this.byteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
                ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
                response.Unpack(em.Data);
                Console.WriteLine("Frame ID: {0}; Result: {1}",
                    response.FrameID, response.DeliveryStatus);
                Assert.AreEqual(0, response.DeliveryStatus);
            }
        }
        [TestMethod]
        public void SendSimpleDataStream()
        {
            ulong broadcast = 0x000000000000FFFF;
            ulong long_address = 0x0013A20041B764AD;
            string text = "";
            for (int i = 0; i < CONSTANTS.MAX_FRAME_DATA; i++)
            {
                text += "x";
            } // looks like 84 is the magic number so that the packets dont fragment; should probably start making a transmission/packet/etc class that can hold arrays of these 84 byte transmissions
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.CreateSendDataFrame(long_address, data_bytes, 1);
            using (SerialRelay relay = new SerialRelay("COM4", this.byteReceiver))
            {
                relay.SendBytes(bytes);
                EmissionDescriptor em = this.byteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
                ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
                response.Unpack(em.Data);
                Console.WriteLine("ID: {0}; Address: 0x{1:X4}; Result: 0x{2:X2}",
                    response.FrameID, response.Address, response.DeliveryStatus);
                bytes = factory.CreateSendDataFrame(response.Address, data_bytes, 1);
                for (int i = 0; i < 10; i++)
                {
                    relay.SendBytes(bytes);
                    em = this.byteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
                    response.Unpack(em.Data);
                    Console.WriteLine("ID: {0}; Address: 0x{1:X4}; Result: 0x{2:X2}",
                        response.FrameID, response.Address, response.DeliveryStatus);
                    Assert.AreEqual(0, response.DeliveryStatus);
                    //System.Threading.Thread.Sleep(10);
                }
            }
        }
        [TestMethod]
        public void SendLargeDataStream()
        {
            ulong broadcast = 0x000000000000FFFF;
            ulong long_address = 0x0013A20041B764AD;
            string text = "";
            for (int i = 0; i < CONSTANTS.MAX_FRAME_DATA; i++)
            {
                text += "x";
            } // looks like 84 is the magic number so that the packets dont fragment; should probably start making a transmission/packet/etc class that can hold arrays of these 84 byte transmissions
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] discover_bytes = Encoding.UTF8.GetBytes("Discover");
            byte[] bytes = factory.CreateSendDataFrame(long_address, discover_bytes, 1);
            using (SerialRelay relay = new SerialRelay("COM4", this.byteReceiver))
            {
                relay.SendBytes(bytes);
                EmissionDescriptor em = this.byteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
                ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
                response.Unpack(em.Data);
                Console.WriteLine("ID: {0}; Address: 0x{1:X4}; Result: 0x{2:X2}",
                    response.FrameID, response.Address, response.DeliveryStatus);
                bytes = factory.CreateSendDataFrame(response.Address, data_bytes, 1);
                for (int i = 0; i < 1000; i++)
                {
                    relay.SendBytes(bytes);
                    em = this.byteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
                    response.Unpack(em.Data);
                    Console.WriteLine("ID: {0}; Address: 0x{1:X4}; Result: 0x{2:X2}",
                        response.FrameID, response.Address, response.DeliveryStatus);
                    Assert.AreEqual(0, response.DeliveryStatus);
                }
            }
        }
    }
}
