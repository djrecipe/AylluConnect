using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class RelayController : IRelayController
    {
        public static RelayController ConnectSerial(string port)
        {
            IRelay relay = new SerialRelay(port);
            return new RelayController(relay);
        }
        private readonly IRelay relay = null;
        private readonly CommandBytesFactory factory = new CommandBytesFactory();
        private readonly EmissionDecoder emissionDecoder = new EmissionDecoder();
        private readonly StructFactory structFactory = new StructFactory();
        private RelayController(IRelay relay_in)
        {
            this.relay = relay_in;
        }
        public void Dispose()
        {
            this.relay.Dispose();
        }
        public ushort Discover(ulong address)
        {
            string text = "Discover";
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.CreateSendDataFrame(address, data_bytes);
            relay.SendBytes(bytes);
            List<byte> response_bytes = relay.WaitForBytes(CONSTANTS.EMISSION_HEADER_SIZE);
            // TODO 5/27/21: move all emission response code to be asynchronous and in response to serial receives, and then fitler through those responses
            EmissionDescriptor em_desc = this.emissionDecoder.Decode(response_bytes.ToArray());
            response_bytes = relay.WaitForBytes(em_desc.Length);
            // 
            ExtendedTransmitStatus response = this.structFactory.Unpack<ExtendedTransmitStatus>(
                response_bytes.ToArray(), CONSTANTS.XTRANSMIT_RESPONSE_SIZE);
            ushort short_address = (ushort)(((ushort)response.AddressH << 8) | response.AddressL);
            if(response.DeliveryStatus !=0)
                throw new Exception($"Error while discovering full address for {address}");
            return short_address;
        }
        public void SendBytes(ushort address, byte[] data)
        {
            for (int i=0; i<data.Length; i+=84 )
            {
                int current_count = Math.Min(84, data.Length-i);
                byte[] current_data_bytes = new byte[current_count];
                Array.Copy(data, i, current_data_bytes, 0, current_count);
                byte[] frame_bytes = factory.CreateSendDataFrame(address, current_data_bytes);
                this.relay.SendBytes(frame_bytes);
                List<byte> response_bytes = relay.WaitForBytes(CONSTANTS.EMISSION_HEADER_SIZE);
                // TODO 5/27/21: move all emission response code to be asynchronous and in response to serial receives, and then fitler through those responses
                EmissionDescriptor em_desc = this.emissionDecoder.Decode(response_bytes.ToArray());
                response_bytes = relay.WaitForBytes(em_desc.Length);
                // 
                ExtendedTransmitStatus response = this.structFactory.Unpack<ExtendedTransmitStatus>(
                    response_bytes.ToArray(), CONSTANTS.XTRANSMIT_RESPONSE_SIZE);
                if (response.DeliveryStatus != 0)
                    throw new Exception($"Error during data transmission to {address} ({i}\\{data.Length})");
            }
        }
    }
}
