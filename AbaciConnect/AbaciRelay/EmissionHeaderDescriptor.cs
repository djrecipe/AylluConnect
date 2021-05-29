using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public enum EmissionTypes : int
    {
        ExtendedTransmitStatus = 0
    }
    public class EmissionHeaderDescriptor
    {
        private static readonly Dictionary<byte, EmissionTypes> Types = new Dictionary<byte, EmissionTypes>
        {
            { CONSTANTS.EM_XTRANSMIT_STATUS, EmissionTypes.ExtendedTransmitStatus},
        };
        public EmissionTypes Type { get;set;}
        public ushort Length { get;set;}
        internal EmissionHeaderDescriptor(EmissionHeader header)
        {
            EmissionTypes type;
            if(!Types.TryGetValue(header.FrameType, out type))
                throw new NotSupportedException($"Emission header frame type not supported ({header.FrameType})");
            this.Type = type;
            this.Length = (ushort)(((ushort)header.LengthH << 8) | header.LengthL);
        }
    }
}
