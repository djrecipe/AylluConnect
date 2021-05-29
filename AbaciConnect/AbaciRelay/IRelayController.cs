﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IRelayController : IDisposable
    {
        public List<byte> ReceiveBytes();
        public void SendBytes(ushort address, byte[] data);
    }
}