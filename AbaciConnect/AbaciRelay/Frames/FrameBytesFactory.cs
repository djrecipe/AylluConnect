using AbaciConnect.Relay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public class FrameBytesFactory
    {
        public byte[] CreateATCommand(string command, byte[] value, byte frame_id, bool queue)
        {
            ushort data_length = (ushort)(4 + value.Length);
            ushort total_length = (ushort)(data_length +4);
            byte[] result = new byte[total_length];
            //
            result[0] = CONSTANTS.FRAME_START_BYTE;               // start byte
            result[1] = (byte)(data_length >> 8);           // data length MSB
            result[2] = (byte)data_length;                  // data length LSB
            result[3] = queue ?
                CONSTANTS.FT_ATCMDQ : CONSTANTS.FT_ATCMD;   // frame type
            result[4] = frame_id;                           // frame ID
            result[5] = (byte)command[0];                   // AT command char 0
            result[6] = (byte)command[1];                   // AT command char 1
            for(int i=0; i< value.Length; i++)
                result[i+7]=value[i];                       // data
            byte checksum = Checksum.Calculate(result.Skip(3).Take(total_length-3-1)); // skip first 3 bytes and last byte
            result[total_length-1] = checksum;               // checksum
            return result;
        }
        public byte[] CreateTransmission(ulong laddress, ushort saddress, byte[] data, byte frame_id, byte radius, byte options)
        {
            ushort data_length = (ushort)(14 + data.Length);
            ushort total_length = (ushort)(data_length + 4);
            byte[] result = new byte[total_length];
            //
            result[0] = CONSTANTS.FRAME_START_BYTE;               // start byte
            result[1] = (byte)(data_length >> 8);           // data length MSB
            result[2] = (byte)data_length;                  // data length LSB
            result[3] = CONSTANTS.FT_TRANSMIT;              // frame type
            result[4] = frame_id;                           // frame ID
            result[5] = (byte)(laddress >> 56);             // destination address MSB
            result[6] = (byte)(laddress >> 48);             // destination address
            result[7] = (byte)(laddress >> 40);             // destination address
            result[8] = (byte)(laddress >> 32);             // destination address
            result[9] = (byte)(laddress >> 24);             // destination address
            result[10] = (byte)(laddress >> 16);            // destination address
            result[11] = (byte)(laddress >> 8);             // destination address
            result[12] = (byte)laddress;                    // destination address LSB
            result[13] = (byte)(saddress>>8);               // 16-bit address MSB
            result[14] = (byte)saddress;                    // 16-bit address LSB
            result[15] = radius;                            // broadcast radius
            result[16] = options;                           // transmission options
            for (int i = 0; i < data.Length; i++)
                result[i + 17] = data[i];                   // data
            byte checksum = Checksum.Calculate(result.Skip(3).Take(total_length - 3 - 1)); // skip first 3 bytes and last byte
            result[total_length - 1] = checksum;            // checksum
            return result;
        }
    }
}
