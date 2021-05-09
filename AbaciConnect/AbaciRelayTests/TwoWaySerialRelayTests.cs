﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AbaciConnect.Relay;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class TwoWaySerialRelayTests
    {
        [TestMethod]
        public void TransmitText()
        {
            string expected_text = "TwoWaySerialRelayTests.SendText";
            string result_text = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    sender.SendString(expected_text);
                    System.Threading.Thread.Sleep(3000);
                    result_text = receiver.GetText();
                    Console.WriteLine(result_text);
                }
            }
            Assert.AreEqual(expected_text, result_text);
        }
        [TestMethod]
        public void TransmitBitmap()
        {
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    sender.SendFile("SimpleImage.bmp");
                    System.Threading.Thread.Sleep(3000);
                    string result = receiver.GetText();
                    byte[] bytes = Encoding.UTF8.GetBytes(result);
                    File.WriteAllBytes("ImageOut.bmp", bytes);
                }
            }
        }
    }
}