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
            IEmissionProcessor processor = new EmissionProcessor();
            IRelay relay = new SerialRelay(port, processor);
            return new RelayController(relay, processor);
        }
        private readonly IEmissionProcessor emissionProcessor = null;
        private readonly TransmissionProcessor transmissionProcessor = new TransmissionProcessor();
        private readonly IRelay relay = null;
        private readonly CommandBytesFactory commandFactory = new CommandBytesFactory();
        internal RelayController(IRelay relay_in, IEmissionProcessor processor_in)
        {
            this.relay = relay_in;
            this.emissionProcessor = processor_in;
            this.emissionProcessor.OnEmission += EmissionProcessor_OnEmission;
        }

        public void Clear()
        {
            this.relay.ClearBuffer();
            this.emissionProcessor.Clear();
            this.transmissionProcessor.Clear();
        }
        public void Dispose()
        {
            this.relay.Dispose();
        }
        public ushort Discover(ulong address)
        {
            string text = CONSTANTS.DISCOVER;
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = commandFactory.CreateSendDataFrame(address, data_bytes, 1);
            relay.SendBytes(bytes);
            //
            EmissionDescriptor desc = this.emissionProcessor.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
            ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
            response.Unpack(desc.Data);
            if(response.DeliveryStatus !=0)
                throw new Exception($"Error while discovering full address for {address}");
            return response.Address;
        }
        public List<EmissionDescriptor> GetAllEmissions()
        {
            return this.emissionProcessor.GetAllEmissions();
        }
        public List<byte> ReceiveBytes()
        {
            EmissionDescriptor desc = this.emissionProcessor.WaitForEmission(EmissionTypes.ReceivePacket, 1000);
            this.emissionProcessor.RemoveEmission(desc.ID);
            ReceivePacketEmission response = new ReceivePacketEmission();
            response.Unpack(desc.Data);
            return response.Data;
        }
        public byte[] ReceiveTransmission(uint id)
        {
            TransmissionObject xm = this.transmissionProcessor.WaitForTransmission(id);
            this.transmissionProcessor.RemoveTransmission(id);
            byte[] bytes = TransmissionObjectFactory.GetData(xm);
            return bytes;
        }
        internal void SendRawBytes(ushort address, byte[] data)
        {
            byte[] frame_bytes = commandFactory.CreateSendDataFrame(address, data, 1);
            this.relay.SendBytes(frame_bytes);
            EmissionDescriptor desc = this.emissionProcessor.WaitForEmission(EmissionTypes.ExtendedTransmitStatus, 1000);
            ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
            response.Unpack(desc.Data);
            if (response.DeliveryStatus != 0)
                throw new Exception($"Error during data transmission of {data.Length} bytes to {address}");
        }
        public void Transmit(ushort address, byte[] data)
        {
            TransmissionObject xm = TransmissionObjectFactory.Create(data, 0);
            byte[] header_bytes = xm.Header.Pack().ToArray();
            this.SendRawBytes(address, header_bytes);
            int total_byte_count = header_bytes.Length;
            foreach(TransmissionChunk chunk in xm.Chunks)
            {
                byte[] chunk_bytes = chunk.Pack().ToArray();
                try
                {
                    this.SendRawBytes(address, chunk_bytes);
                    total_byte_count+= chunk_bytes.Length;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Failed at chunk {chunk.Header.ID} after {total_byte_count} bytes");
                    throw e;
                }
            }
        }
        private void EmissionProcessor_OnEmission(object sender, EmissionDescriptor emission)
        {
            this.transmissionProcessor.ProcessEmission(emission);
        }
    }
}
