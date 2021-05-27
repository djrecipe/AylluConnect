using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class EmissionDescriptor
    {
        private static readonly Dictionary<byte, Type> Types = new Dictionary<byte, Type>
        {
            { CONSTANTS.EM_XTRANSMIT_STATUS, typeof(ExtendedTransmitStatus)},
        };
        public Type Type { get;set;}
        public ushort Length { get;set;}
        internal EmissionDescriptor(EmissionHeader header)
        {
            Type type;
            if(!Types.TryGetValue(header.FrameType, out type))
                throw new NotSupportedException($"Emission header frame type not supported ({header.FrameType})");
            this.Type = type;
            this.Length = (ushort)(((ushort)header.LengthH << 8) | header.LengthL);
        }
    }
}
