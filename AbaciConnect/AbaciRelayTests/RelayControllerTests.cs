using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;
using AbaciConnect.Relay.Emission;
using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Processors;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class RelayControllerTests
    {
        [TestMethod]
        public void SendLargeDataStream()
        {
            string text = "";
            int expected_byte_count = CONSTANTS.MAX_FRAME_DATA;
            for (int i = 0; i < expected_byte_count; i++)
            {
                text += "x";
            }
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            IEmissionProcessor receiver = new EmissionProcessor();
            IRelay relay = new SerialRelay("COM4", receiver);
            using (RelayController ctrl_send = new RelayController(relay, receiver))
            {
                ulong long_address = 0x0013A20041B764AD;
                ushort short_address = ctrl_send.Discover(long_address);
                for (int i = 0; i < 1000; i++)
                {
                    // send ACTUAL data
                    try
                    {
                        ctrl_send.SendRawBytes(short_address, data_bytes);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Failed at iteration {i} after {CONSTANTS.MAX_FRAME_DATA * i} bytes");
                        break;
                    }
                }
            }
        }
        [TestMethod]
        public void SendAndReceiveSmallDataStream()
        {
            string text = "";
            int expected_byte_count = 10;
            for (int i = 0; i < expected_byte_count; i++)
            {
                text += "x";
            }
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                using (RelayController ctrl_receive = RelayController.ConnectSerial("COM6"))
                {
                    ulong long_address = 0x0013A20041B764AD;
                    ushort short_address = ctrl_send.Discover(long_address);
                    // receive "Discover" emission
                    List<byte> rcv_bytes = ctrl_receive.ReceiveBytes();
                    Assert.AreEqual("Discover".Length, rcv_bytes.Count);
                    // send ACTUAL data
                    ctrl_send.SendRawBytes(short_address, data_bytes);
                    // receive ACTUAL data transmission
                    rcv_bytes = ctrl_receive.ReceiveBytes();
                    Assert.AreEqual(expected_byte_count, rcv_bytes.Count);
                    for (int i = 0; i < expected_byte_count; i++)
                    {
                        Assert.AreEqual((byte)'x', rcv_bytes[i]);
                    }
                }
            }
        }
        [TestMethod]
        public void SendAndReceiveLargeDataStream()
        {
            string text = "";
            int expected_byte_count = 10;
            for (int i = 0; i < expected_byte_count; i++)
            {
                text += "x";
            }
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                using (RelayController ctrl_receive = RelayController.ConnectSerial("COM6"))
                {
                    ulong long_address = 0x0013A20041B764AD;
                    ushort short_address = ctrl_send.Discover(long_address);
                    // receive "Discover" emission
                    List<byte> rcv_bytes = ctrl_receive.ReceiveBytes();
                    Assert.AreEqual("Discover".Length, rcv_bytes.Count);
                    for(int i=0; i<1000; i++)
                    {
                        // send ACTUAL data
                        ctrl_send.SendRawBytes(short_address, data_bytes);
                        // receive ACTUAL data transmission
                        rcv_bytes = ctrl_receive.ReceiveBytes();
                        Assert.AreEqual(expected_byte_count, rcv_bytes.Count);
                        for (int j = 0; j < expected_byte_count; j++)
                        {
                            Assert.AreEqual((byte)'x', rcv_bytes[j]);
                        }
                    }
                }
            }
        }
        [TestMethod]
        public void SendAndReceiveTransmissonObject()
        {
            string text = "";
            for (int i = 0; i < 100; i++)
            {
                text += i.ToString()+"\n";
            }
            byte[] raw_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                ctrl_send.Clear();
                using (RelayController ctrl_rcv = RelayController.ConnectSerial("COM6"))
                {
                    ctrl_rcv.Clear();
                    ulong long_address = 0x0013A20041B764AD;
                    ushort short_address = ctrl_send.Discover(long_address);
                    // receive "Discover" emission
                    List<byte> rcv_bytes = ctrl_rcv.ReceiveBytes();
                    Assert.AreEqual("Discover".Length, rcv_bytes.Count);
                    // send actual data
                    ctrl_send.Transmit(short_address, raw_bytes);
                    // receive actual data
                    rcv_bytes = ctrl_rcv.ReceiveTransmission(0);
                    Assert.AreEqual(raw_bytes.Length, rcv_bytes.Count);
                    File.WriteAllBytes("SendAndReceiveTransmissonObject.txt", rcv_bytes.ToArray());
                }
            }
        }
        [TestMethod]
        public void SendLargeTransmissonObject()
        {
            string text = "";
            for (int i = 0; i < 20000; i++)
            {
                text += "x";
            }
            byte[] raw_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                ctrl_send.Clear();
                ulong long_address = 0x0013A20041B764AD;
                ushort short_address = ctrl_send.Discover(long_address);
                // send actual data
                try
                {
                    ctrl_send.Transmit(short_address, raw_bytes);
                }
                catch
                {
                    // ignored
                    Console.WriteLine("Transmission failed");
                }
                List<EmissionDescriptor> emissions = ctrl_send.GetAllEmissions();
                foreach(EmissionDescriptor descriptor in emissions)
                {
                    Console.WriteLine($"Unused emission with frame type {descriptor.Header.FrameType}");
                }
            }
        }
        [TestMethod]
        public void SendAndReceiveLargeTransmissonObject()
        {
            string text = "";
            for (int i = 0; i < 10000; i++)
            {
                text += i.ToString()+"\r\n";
            }
            byte[] raw_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                using(RelayController ctrl_rcv = RelayController.ConnectSerial("COM6"))
                {

                    ctrl_send.Clear();
                    ulong long_address = 0x0013A20041B764AD;
                    ushort short_address = ctrl_send.Discover(long_address);
                    // receive "Discover" emission
                    List<byte> rcv_bytes = ctrl_rcv.ReceiveBytes();
                    Assert.AreEqual("Discover".Length, rcv_bytes.Count);
                    // send actual data
                    try
                    {
                        ctrl_send.Transmit(short_address, raw_bytes);
                    }
                    catch
                    {
                        // ignored
                        Console.WriteLine("Transmission failed");
                    }
                    List<EmissionDescriptor> emissions = ctrl_send.GetAllEmissions();
                    foreach (EmissionDescriptor descriptor in emissions)
                    {
                        Console.WriteLine($"Unused emission with frame type {descriptor.Header.FrameType}");
                    }
                    // receive actual data
                    rcv_bytes = ctrl_rcv.ReceiveTransmission(0);
                    File.WriteAllBytes("SendAndReceiveLargeTransmissonObject.txt", rcv_bytes.ToArray());
                    Assert.AreEqual(raw_bytes.Length, rcv_bytes.Count);
                }
            }
        }
    }
}
