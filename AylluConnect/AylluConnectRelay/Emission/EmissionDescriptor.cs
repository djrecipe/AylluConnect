using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Emission
{
    public class EmissionDescriptor
    { 
        private static ulong currentId = 0;
        private readonly EmissionHeader header=null;
        private readonly List<byte> data=null;
        public EmissionHeader Header => this.header;
        public List<byte> Data => new List<byte>(this.data);
        public ulong ID {get; private set; }
        public EmissionDescriptor(EmissionHeader header_in, List<byte> data_in)
        {
            this.header = header_in;
            this.data=new List<byte>(data_in);
            this.ID = currentId++;
        }
    }
}
