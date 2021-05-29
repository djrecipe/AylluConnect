﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IRelay : IDisposable
    {
        public void ClearBuffer();
        public void SendBytes(byte[] data);
    }
}