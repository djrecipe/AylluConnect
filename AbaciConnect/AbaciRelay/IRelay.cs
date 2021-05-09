using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IRelay : IDisposable
    {
        public void SendBytes(byte[] data);
        public void SendFile(string path);
        public void SendString(string text);
    }
}
