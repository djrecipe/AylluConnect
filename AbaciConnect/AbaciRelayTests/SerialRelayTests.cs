using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
namespace AbaciConnect.RelayTests
{
    [TestClass]
    public class SerialRelayTests
    {
        [TestMethod]
        public void OpenAndClose()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
            }
        }
        [TestMethod]
        public void SendData()
        {
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                string text = "test";
                byte[] data = Encoding.UTF8.GetBytes(text);
                relay.Send(data);
            }
        }
    }
}
