using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.IO;
using AbaciConnect.Relay;
using AbaciConnect.Relay.Emission;
using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Processors;
using AbaciConnect.Relay.TransmissionObjects;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class RelayControllerTests
    {
        [TestMethod]
        public void SendData()
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
        public void ScrambleAndTransmit()
        {
            int expected_byte_count = 1000;
            uint expected_object_id = 5;
            byte[] raw_bytes = new byte[expected_byte_count];
            for (int i = 0; i < expected_byte_count; i++)
            {
                raw_bytes[i] = (byte)i;
            }
            TransmissionObjectFactory transmissionFactory = new TransmissionObjectFactory();
            using (RelayController ctrl_send = RelayController.ConnectSerial("COM4"))
            {
                using (RelayController ctrl_rcv = RelayController.ConnectSerial("COM6"))
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
                        TransmissionObject xm = transmissionFactory.Create(raw_bytes, expected_object_id);
                        byte[] header_bytes = xm.Header.Pack().ToArray();
                        ctrl_send.SendRawBytes(short_address, header_bytes);
                        int total_byte_count = header_bytes.Length;
                        Random rnd = new Random();
                        IEnumerable<TransmissionChunk> chunks = xm.Chunks.OrderBy(c => rnd.Next());
                        foreach (TransmissionChunk chunk in chunks)
                        {
                            byte[] packet_bytes = chunk.Pack().ToArray();
                            try
                            {
                                ctrl_send.SendRawBytes(short_address, packet_bytes);
                                total_byte_count += packet_bytes.Length;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Failed at packet {chunk.Header.ID} after {total_byte_count} bytes");
                                throw e;
                            }
                        }
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
                    rcv_bytes = ctrl_rcv.ReceiveTransmission(expected_object_id);
                    Assert.AreEqual(expected_byte_count, rcv_bytes.Count);
                    for (int i = 0; i < expected_byte_count; i++)
                    {
                        Assert.AreEqual(raw_bytes[i], rcv_bytes[i]);
                    }
                }
            }
        }
        [TestMethod]
        public void TransmitLargeObject()
        {
            int expected_byte_count = 20000;
            byte[] raw_bytes = new byte[expected_byte_count];
            for(int i=0; i< expected_byte_count; i++)
            {
                raw_bytes[i]=(byte)i;
            }
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
                    Assert.AreEqual(expected_byte_count, rcv_bytes.Count);
                    for(int i=0; i< expected_byte_count; i++)
                    {
                        Assert.AreEqual(raw_bytes[i], rcv_bytes[i]);
                    }
                }
            }
        }
    }
}
