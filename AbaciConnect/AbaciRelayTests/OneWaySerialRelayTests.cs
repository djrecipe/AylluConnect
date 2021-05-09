using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
namespace AbaciConnect.RelayTests
{
    [TestClass]
    [DeploymentItem("SimpleImage.bmp")]
    public class OneWaySerialRelayTests
    {
        [TestMethod]
        public void OpenAndClose()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
            }
        }
        [TestMethod]
        public void SendBytes()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                string text = "SerialRelayTests.SendBytes";
                byte[] data = Encoding.UTF8.GetBytes(text);
                relay.SendBytes(data);
            }
        }
        [TestMethod]
        public void SendString()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                string text = "SerialRelayTests.SendString";
                relay.SendString(text);
            }
        }
        [TestMethod]
        public void SendTextFile()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                string path = "SimpleText.txt";
                relay.SendFile(path);
            }
        }
        [TestMethod]
        public void SendBitmapFile()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                string path = "SimpleImage.bmp";
                relay.SendFile(path);
            }
        }
    }
}
