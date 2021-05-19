using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class CommandBytesFactory
    {
        private readonly FrameBytesFactory bytesFactory = new FrameBytesFactory();
        public byte[] SetName(string name)
        {
            byte[] name_bytes = Encoding.UTF8.GetBytes(name);
            return this.bytesFactory.CreateATCommand("NI", name_bytes, 0);
        }
        public byte[] SetNetworkSettings(NetworkSettings settings)
        {
            List<byte> result = new List<byte>();
            byte[] parameter;
            byte[] buffer;
            // enable encryption
            parameter = new byte[] { (byte)(settings.EnableEncryption ? 1 : 0) };
            buffer = this.bytesFactory.CreateATCommand("EE", parameter, 0);
            result.AddRange(buffer);
            // max transmission size
            parameter = new byte[] { settings.MaxTransmissionSize };
            buffer = this.bytesFactory.CreateATCommand("NP", parameter, 0);
            result.AddRange(buffer);
            // network id
            parameter = new byte[]
            {
                (byte)(settings.NetworkID >> 24),
                (byte)(settings.NetworkID >> 16),
                (byte)(settings.NetworkID >> 8),
                (byte)(settings.NetworkID),
            };
            buffer = this.bytesFactory.CreateATCommand("ID", parameter, 0);
            result.AddRange(buffer);
            // node discovery delay
            parameter = new byte[] { settings.NodeDiscoveryDelay };
            buffer = this.bytesFactory.CreateATCommand("NT", parameter, 0);
            result.AddRange(buffer);
            // node join time
            parameter = new byte[] { settings.NodeJoinTime };
            buffer = this.bytesFactory.CreateATCommand("NJ", parameter, 0);
            result.AddRange(buffer);
            // role
            parameter = new byte[] { (byte)settings.Role };
            buffer = this.bytesFactory.CreateATCommand("CE", parameter, 0);
            result.AddRange(buffer);
            //
            return result.ToArray();
        }
        public byte[] SendData(ulong address, byte[] data)
        {
            return this.bytesFactory.CreateTransmission(address, data, 0, 0, 0);
        }
    }
}
