using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.Frames
{
    public interface ITransmissionFactory
    {
        byte[] CreateTransmission(ulong laddress, ushort saddress, byte[] data, byte frame_id, byte radius, byte options);
        byte[] CreateATCommand(string command, byte[] value, byte frame_id, bool queue);
    }
}
