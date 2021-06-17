using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class ApiFrameTransmissionObjectFormatter : ITransmissionObjectFormatter
    {
        private readonly CommandBytesFactory commandFactory = new CommandBytesFactory();
        public byte[] FormatDataBytes(ushort address, byte[] data)
        {
            return commandFactory.CreateSendDataFrame(address, data, 1);
        }

        public byte[] FormatDataBytes(ulong address, byte[] data)
        {
            return commandFactory.CreateSendDataFrame(address, data, 1);
        }

        public byte[] GetHeaderBytes(TransmissionObject obj)
        {
            return obj.Header.Pack().ToArray();
        }
    }
}
