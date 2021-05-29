using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class RelayControllerTests
    {
        [TestMethod]
        public void SendAndReceiveArbitraryDataStream()
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
                using(RelayController ctrl_receive = RelayController.ConnectSerial("COM6"))
                {
                    ulong long_address = 0x0013A20041B764AD;
                    ushort short_address = ctrl_send.Discover(long_address);
                    ctrl_send.SendBytes(short_address, data_bytes);
                    // receive "Discover" emission
                    List<byte> rcv_bytes = ctrl_receive.ReceiveBytes();
                    Assert.AreEqual("Discover".Length, rcv_bytes.Count);
                    // receive ACTUAL data transmission
                    rcv_bytes = ctrl_receive.ReceiveBytes();
                    Assert.AreEqual(expected_byte_count, rcv_bytes.Count);
                }
            }
        }
    }
}
