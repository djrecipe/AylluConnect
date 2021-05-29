using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Emission;
using AbaciConnect.Relay.TransmissionObjects;
using AbaciConnect.Relay.Processors;

namespace AbaciConnect.Relay
{
    public class RelayController : IRelayController
    {
        public static RelayController ConnectSerial(string port)
        {
            IEmissionProcessor receiver = new EmissionProcessor();
            IRelay relay = new SerialRelay(port, receiver);
            return new RelayController(relay, receiver);
        }
        private readonly IEmissionProcessor frameByteReceiver = null;
        private readonly TransmissionProcessor transmissionReceiver = new TransmissionProcessor();
        private readonly IRelay relay = null;
        private readonly CommandBytesFactory factory = new CommandBytesFactory();
        private readonly TransmissionObjectFactory transmissionFactory = new TransmissionObjectFactory();
        internal RelayController(IRelay relay_in, IEmissionProcessor receiver_in)
        {
            this.relay = relay_in;
            this.frameByteReceiver = receiver_in;
            this.frameByteReceiver.OnEmission += FrameByteReceiver_OnEmission;
        }

        public void Clear()
        {
            this.relay.ClearBuffer();
            this.frameByteReceiver.Clear();
            this.transmissionReceiver.Clear();
        }
        public void Dispose()
        {
            this.relay.Dispose();
        }
        public ushort Discover(ulong address)
        {
            string text = CONSTANTS.DISCOVER;
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.CreateSendDataFrame(address, data_bytes, 1);
            relay.SendBytes(bytes);
            //
            EmissionDescriptor desc = this.frameByteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
            ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
            response.Unpack(desc.Data);
            if(response.DeliveryStatus !=0)
                throw new Exception($"Error while discovering full address for {address}");
            return response.Address;
        }
        public List<EmissionDescriptor> GetAllEmissions()
        {
            return this.frameByteReceiver.GetAllEmissions();
        }
        public List<byte> ReceiveBytes()
        {
            EmissionDescriptor desc = this.frameByteReceiver.WaitForEmission(EmissionTypes.ReceivePacket, 1000);
            this.frameByteReceiver.RemoveEmission(desc.ID);
            ReceivePacketEmission response = new ReceivePacketEmission();
            response.Unpack(desc.Data);
            return response.Data;
        }
        public List<byte> ReceiveTransmission(int id)
        {
            TransmissionObject xm = this.transmissionReceiver.WaitForTransmission(id);
            this.transmissionReceiver.RemoveTransmission(id);
            List<byte> bytes = this.transmissionFactory.GetDataBytes(xm);
            return bytes;
        }
        internal void SendRawBytes(ushort address, byte[] data)
        {
            byte[] frame_bytes = factory.CreateSendDataFrame(address, data, 1);
            this.relay.SendBytes(frame_bytes);
            EmissionDescriptor desc = this.frameByteReceiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
            ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
            response.Unpack(desc.Data);
            if (response.DeliveryStatus != 0)
                throw new Exception($"Error during data transmission of {data.Length} bytes to {address}");
        }
        public void Transmit(ushort address, byte[] data)
        {
            TransmissionObject xm = this.transmissionFactory.Create(data, 0);
            byte[] header_bytes = xm.Header.Pack().ToArray();
            this.SendRawBytes(address, header_bytes);
            int total_byte_count = header_bytes.Length;
            foreach(TransmissionChunk packet in xm.Packets)
            {
                byte[] packet_bytes = packet.Pack().ToArray();
                try
                {
                    this.SendRawBytes(address, packet_bytes);
                    total_byte_count+= packet_bytes.Length;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Failed at packet {packet.Header.ID} after {total_byte_count} bytes");
                    throw e;
                }
            }
        }
        private void FrameByteReceiver_OnEmission(object sender, EmissionDescriptor emission)
        {
            this.transmissionReceiver.ProcessEmission(emission);
        }
    }
}
