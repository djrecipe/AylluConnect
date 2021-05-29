using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IByteReceiver
    {
        public byte StartByte { get;}
        public void ReceiveBytes(List<byte> bytes);
        public EmissionDescriptor WaitForEmission(EmissionTypes type);
    }
}
