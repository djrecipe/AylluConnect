using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Common
{
    public abstract class Checksum
    {
        public static byte Calculate(IEnumerable<byte> data)
        {
            byte byte_sum = 0;
            for (int i = 0; i < data.Count(); i++)
                byte_sum += data.ElementAt(i);
            byte checksum = (byte)(0xFF - byte_sum);
            return checksum;
        }
    }
}
