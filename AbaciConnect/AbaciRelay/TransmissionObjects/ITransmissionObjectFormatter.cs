using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public interface ITransmissionObjectFormatter
    {
        byte[] GetHeaderBytes(TransmissionObject obj);
        byte[] FormatDataBytes(ushort address, byte[] data);
        byte[] FormatDataBytes(ulong address, byte[] data);
    }
}
