using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciConnect.Relay.EmissionStructures;

namespace AbaciConnect.Relay
{
    interface IByteReceiver
    {
        public byte StartByte { get;}
        public void ReceiveBytes(List<byte> bytes);
        public void RemoveEmission(ulong id);
        public EmissionDescriptor WaitForEmission(EmissionTypes type);
    }
}
