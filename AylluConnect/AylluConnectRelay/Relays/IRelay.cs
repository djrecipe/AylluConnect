using AbaciConnect.Relay.TransmissionObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public interface IRelay : IDisposable
    {
        ITransmissionObjectFormatter TranmissionFormatter { get;}
        void ClearBuffer();
        void SendBytes(byte[] data);
    }
}
