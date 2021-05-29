using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Processors
{
    public interface IByteProcessor
    {
        public byte StartByte { get; }
        public void Clear();
    }
}
