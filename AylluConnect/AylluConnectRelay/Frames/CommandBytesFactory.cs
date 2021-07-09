using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Frames
{
    public class CommandBytesFactory
    {
        private readonly ITransmissionFactory transmissionFactory = null;
        public CommandBytesFactory(ITransmissionFactory tranmission_factory)
        {
            this.transmissionFactory = tranmission_factory;
        }
        public byte[] CreateSetNameFrame(string name)
        {
            byte[] name_bytes = Encoding.UTF8.GetBytes(name);
            return this.transmissionFactory.CreateATCommand("NI", name_bytes, 0, false);
        }
        public byte[] CreateSetNetworkSettingsFrame(NetworkSettings settings)
        {
            List<byte> result = new List<byte>();
            byte[] parameter;
            byte[] buffer;
            // enable host verification
            parameter = new byte[] { (byte)(settings.EnableHostVerification ? 1 : 0) };
            buffer = this.transmissionFactory.CreateATCommand("JV", parameter, 0, true);
            result.AddRange(buffer);
            // enable encryption
            parameter = new byte[] { (byte)(settings.EnableEncryption ? 1 : 0) };
            buffer = this.transmissionFactory.CreateATCommand("EE", parameter, 0, true);
            result.AddRange(buffer);
            // enable join verification
            parameter = new byte[] { (byte)(settings.EnableJoinVerification ? 1 : 0) };
            buffer = this.transmissionFactory.CreateATCommand("JN", parameter, 0, true);
            result.AddRange(buffer);
            // max transmission size
            parameter = new byte[] { settings.MaxTransmissionSize };
            buffer = this.transmissionFactory.CreateATCommand("NP", parameter, 0, true);
            result.AddRange(buffer);
            // network id
            parameter = new byte[]
            {
                (byte)(settings.NetworkID >> 56),
                (byte)(settings.NetworkID >> 48),
                (byte)(settings.NetworkID >> 40),
                (byte)(settings.NetworkID >> 32),
                (byte)(settings.NetworkID >> 24),
                (byte)(settings.NetworkID >> 16),
                (byte)(settings.NetworkID >> 8),
                (byte)(settings.NetworkID),
            };
            buffer = this.transmissionFactory.CreateATCommand("ID", parameter, 0, true);
            result.AddRange(buffer);
            // node discovery delay
            parameter = new byte[] { settings.NodeDiscoveryDelay };
            buffer = this.transmissionFactory.CreateATCommand("NT", parameter, 0, true);
            result.AddRange(buffer);
            // node join time
            parameter = new byte[] { settings.NodeJoinTime };
            buffer = this.transmissionFactory.CreateATCommand("NJ", parameter, 0, true);
            result.AddRange(buffer);
            // role
            parameter = new byte[] { (byte)settings.Role };
            buffer = this.transmissionFactory.CreateATCommand("CE", parameter, 0, true);
            result.AddRange(buffer);
            // apply changes
            parameter = new byte[] { };
            buffer = this.transmissionFactory.CreateATCommand("AC", parameter, 0, false);
            result.AddRange(buffer);
            return result.ToArray();
        }
        public byte[] CreateSendDataFrame(ulong address, byte[] data, byte frame_id)
        {
            return this.transmissionFactory.CreateTransmission(address, 0xFFFE, data, frame_id, 0, 0);
        }
        public byte[] CreateSendDataFrame(ushort address, byte[] data, byte frame_id)
        {
            return this.transmissionFactory.CreateTransmission(0xFFFFFFFFFFFFFFFF, address, data, frame_id, 0, 0);
        }
    }
}
