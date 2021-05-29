using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    interface IUnpackable
    {
         public void Unpack(List<byte> bytes);
    }
}
