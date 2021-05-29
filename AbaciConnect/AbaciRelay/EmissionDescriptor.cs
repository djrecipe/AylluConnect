using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class EmissionDescriptor
    { 
        private readonly EmissionHeaderDescriptor header=null;
        private readonly List<byte> data=null;
        public EmissionHeaderDescriptor Header => this.header;
        public List<byte> Data => new List<byte>(this.data);
        public EmissionDescriptor(EmissionHeaderDescriptor header_in, List<byte> data_in)
        {
            this.header = header_in;
            this.data=new List<byte>(data_in);
        }
    }
}
