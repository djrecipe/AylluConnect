using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Common
{
    interface IPackable
    {
        List<byte> Pack();
    }
    interface IUnpackable
    {
         void Unpack(List<byte> bytes);
    }
}
