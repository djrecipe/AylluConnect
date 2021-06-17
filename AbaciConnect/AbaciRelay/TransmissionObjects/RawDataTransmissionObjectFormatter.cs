using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class RawDataTransmissionObjectFormatter : ITransmissionObjectFormatter
    {
        public byte[] FormatDataBytes(ushort address, byte[] data)
        {
            return data;
        }

        public byte[] FormatDataBytes(ulong address, byte[] data)
        {
            return data;
        }

        public byte[] GetHeaderBytes(TransmissionObject obj)
        {
            return null;
        }
    }
}
