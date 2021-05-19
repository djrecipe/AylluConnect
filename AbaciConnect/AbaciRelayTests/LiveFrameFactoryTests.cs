using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbaciConnect.Relay;
using System.Text;
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
            NetworkSettings settings = new NetworkSettings();
            settings.Role = DeviceRoles.Create;
            byte[] bytes = factory.SetNetworkSettings(settings);
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                relay.SendBytes(bytes);
            }
        }
        [TestMethod]
        public void SendEasyData()
        {
            ulong broadcast = 0x000000000000FFFF;
            ulong address = 0x0013A20041B764AD;
            string text = "test";
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.SendData(broadcast, data_bytes);
            for(int i=0; i<bytes.Length; i++)
            {
                System.Console.WriteLine("{0:X2} ({0})", bytes[i], bytes[i]);
            }
            using (SerialRelay relay = new SerialRelay("COM4"))
            {
                relay.SendBytes(bytes);
            }
        }
    }
}
