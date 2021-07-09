using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IRelayController : IDisposable
    {
        List<byte> ReceiveBytes();
        void Transmit(ushort address, byte[] data);
    }
}
