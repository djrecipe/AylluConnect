using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Common
{
    interface IPackable
    {
        public List<byte> Pack();
    }
    interface IUnpackable
    {
         public void Unpack(List<byte> bytes);
    }
}
