﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciConnect.Relay.EmissionStructures;

namespace AbaciConnect.Relay
{
    public class RelayController : IRelayController
    {
        public static RelayController ConnectSerial(string port)
        {
            IByteReceiver receiver = new FrameByteReceiver();
            IRelay relay = new SerialRelay(port, receiver);
            return new RelayController(relay, receiver);
        }
        private readonly IByteReceiver receiver = null;
        private readonly IRelay relay = null;
        private readonly CommandBytesFactory factory = new CommandBytesFactory();
        private RelayController(IRelay relay_in, IByteReceiver receiver_in)
        {
            this.relay = relay_in;
            this.receiver = receiver_in;
        }
        public void Dispose()
        {
            this.relay.Dispose();
        }
        public ushort Discover(ulong address)
        {
            string text = CONSTANTS.DISCOVER;
            byte[] data_bytes = Encoding.UTF8.GetBytes(text);
            byte[] bytes = factory.CreateSendDataFrame(address, data_bytes);
            relay.SendBytes(bytes);
            //
            EmissionDescriptor desc = this.receiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus);
            ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
            response.Unpack(desc.Data);
            if(response.DeliveryStatus !=0)
                throw new Exception($"Error while discovering full address for {address}");
            return response.Address;
        }
        public List<byte> ReceiveBytes()
        {
            EmissionDescriptor desc = this.receiver.WaitForEmission(EmissionTypes.ReceivePacket);
            this.receiver.RemoveEmission(desc.ID);
            ReceivePacketEmission response = new ReceivePacketEmission();
            response.Unpack(desc.Data);
            return response.Data;
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
                //
                EmissionDescriptor desc = this.receiver.WaitForEmission(EmissionTypes.ExtendedTransmitStatus);
                ExtendedTransmitStatusEmission response = new ExtendedTransmitStatusEmission();
                response.Unpack(desc.Data);
                if (response.DeliveryStatus != 0)
                    throw new Exception($"Error during data transmission to {address} ({i}\\{data.Length})");
            }
        }
    }
}