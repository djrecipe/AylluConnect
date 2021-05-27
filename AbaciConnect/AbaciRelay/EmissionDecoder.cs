using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class EmissionDecoder
    {
        private readonly StructFactory structFactory = new StructFactory();
        public EmissionDescriptor Decode(byte[] data)
        {
            if (data.Length < 4)
                throw new Exception($"Invalid emission frame length ({data.Length})");
            if (data[0] != CONSTANTS.START_BYTE)
                throw new Exception($"Invalid emission frame start byte ({data[0]})");
            EmissionHeader header = this.structFactory.Unpack<EmissionHeader>(data, CONSTANTS.EMISSION_HEADER_SIZE);
            EmissionDescriptor descriptor = new EmissionDescriptor(header);
            return descriptor;
        }
    }
}
