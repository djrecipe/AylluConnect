using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AbaciConnect.Relay.EmissionStructures
{
    public class EmissionHeader : IUnpackable
    {
        public byte StartByte { get; private set; } = 0;
        public ushort Length { get; private set; } = 0;
        public EmissionTypes FrameType { get; private set; } = EmissionTypes.None;
        public void Unpack(List<byte> bytes)
        {
            if(bytes.Count < 4)
                throw new Exception($"Invalid byte count {bytes.Count} to unpack emission header");
            this.StartByte = bytes[0];
            if(this.StartByte != CONSTANTS.START_BYTE)
                throw new Exception($"Invalid start byte {this.StartByte} while unpacking emission header");
            this.Length = (ushort)(((ushort)bytes[1] << 8) | bytes[2]);
            if (!Enum.IsDefined(typeof(EmissionTypes), bytes[3]))
                throw new NotImplementedException($"Emission frame type {bytes[3]} not implemented");
            this.FrameType = (EmissionTypes)bytes[3];
        }
    }
}
