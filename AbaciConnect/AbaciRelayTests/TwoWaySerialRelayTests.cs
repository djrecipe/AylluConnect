using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AbaciConnect.Relay;
using System.Diagnostics;

namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class TwoWaySerialRelayTests
    {
        [TestMethod, Timeout(2000)]
        public void TransmitText()
        {
            string expected_text = "TwoWaySerialRelayTests.SendText";
            string result_text = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    receiver.ClearBuffer();
                    sender.SendString(expected_text);
                    List<byte> result_bytes = receiver.WaitForBytes(expected_text.Length);
                    result_text = Encoding.ASCII.GetString(result_bytes.ToArray());
                    Debug.WriteLine(result_text);
                }
            }
            Assert.AreEqual(expected_text, result_text);
        }
        [TestMethod, Timeout(2000)]
        public void TransmitTextFile()
        {
            string expected_path = "SimpleText.txt";
            string result_path = "SimpleText_Out.txt";
            string expected_text = File.ReadAllText(expected_path);
            string result_text = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    receiver.ClearBuffer();
                    sender.SendFile(expected_path);
                    List<byte> result_bytes = receiver.WaitForBytes(expected_text.Length);
                    File.WriteAllBytes(result_path, result_bytes.ToArray());
                    result_text = File.ReadAllText(result_path);
                }
            }
            Assert.AreEqual(expected_text, result_text);
        }
        [TestMethod, Timeout(5000)]
        public void TransmitBitmapFile246B()
        {
            string expected_path = "SimpleImage.bmp";
            string result_path = "SimpleImage_Out.bmp";
            byte[] expected_bytes = File.ReadAllBytes(expected_path);
            int expected_length = expected_bytes.Length;
            byte[] result_bytes = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    receiver.ClearBuffer();
                    sender.SendFile(expected_path);
                    List<byte> result_bytes_list = receiver.WaitForBytes(expected_length);
                    File.WriteAllBytes(result_path, result_bytes_list.ToArray());
                    result_bytes = File.ReadAllBytes(result_path);
                }
            }
            Assert.IsNotNull(result_bytes);
            Assert.AreEqual(expected_length, result_bytes.Length);
            for(int i=0; i< expected_length; i++)
            {
                Assert.AreEqual(expected_bytes[i], result_bytes[i]);
            }
        }
        [TestMethod, Timeout(5000)]
        public void TransmitBitmapFile11KB()
        {
            string expected_path = "ShinChan.jpg";
            string result_path = "ShinChan_Out.jpg";
            byte[] expected_bytes = File.ReadAllBytes(expected_path);
            int expected_length = expected_bytes.Length;
            byte[] result_bytes = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    receiver.ClearBuffer();
                    sender.SendFile(expected_path);
                    List<byte> result_bytes_list = receiver.WaitForBytes(expected_length);
                    File.WriteAllBytes(result_path, result_bytes_list.ToArray());
                    result_bytes = File.ReadAllBytes(result_path);
                }
            }
            Assert.IsNotNull(result_bytes);
            Assert.AreEqual(expected_length, result_bytes.Length);
            for (int i = 0; i < expected_length - 10; i += 10)
            {
                Assert.AreEqual(expected_bytes[i], result_bytes[i]);
            }
        }
        [TestMethod, Timeout(20000)]
        public void TransmitBitmapFile10MB()
        {
            string expected_path = "Mustang.jpg";
            string result_path = "Mustang_Out.jpg";
            byte[] expected_bytes = File.ReadAllBytes(expected_path);
            int expected_length = expected_bytes.Length;
            byte[] result_bytes = null;
            using (SerialRelay sender = new SerialRelay("COM4"))
            {
                using (SerialRelay receiver = new SerialRelay("COM6"))
                {
                    receiver.ClearBuffer();
                    sender.SendFile(expected_path);
                    List<byte> result_bytes_list = receiver.WaitForBytes(expected_length);
                    File.WriteAllBytes(result_path, result_bytes_list.ToArray());
                    result_bytes = File.ReadAllBytes(result_path);
                }
            }
            Assert.IsNotNull(result_bytes);
            Assert.AreEqual(expected_length, result_bytes.Length);
            for (int i = 0; i < expected_length - 10; i += 10)
            {
                Assert.AreEqual(expected_bytes[i], result_bytes[i]);
            }
        }
    }
}
