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
        public void SendArbitraryDataStream()
        {
            string text = "";
            for (int i = 0; i < 10000; i++)
            {
                text += "x";
            }
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            using (RelayController controller = RelayController.ConnectSerial("COM4"))
            {
                ulong long_address = 0x0013A20041B764AD;
                ushort short_address = controller.Discover(long_address);
                controller.SendBytes(short_address, data_bytes);
            }
        }
    }
}
