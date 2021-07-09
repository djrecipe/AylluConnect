using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AbaciConnect.Relay.Emission;
using AbaciConnect.Relay.TransmissionObjects;
using AbaciConnect.Relay.Common;

namespace AbaciConnect.Relay.Processors
{
    public class TransmissionProcessor : IByteProcessor
    {
        private readonly Mutex mutexTransmissions = new Mutex();
        private readonly List<TransmissionObject> transmissions = new List<TransmissionObject>();
        private readonly List<TransmissionObjectHeader> headers = new List<TransmissionObjectHeader>();
        private readonly List<TransmissionChunk> chunks = new List<TransmissionChunk>();
        public byte StartByte => CONSTANTS.XM_HEADER_START_BYTE;
        public void Clear()
        {
            this.mutexTransmissions.WaitOne();
            // TODO 5/29/21: clear headers and packets too
            this.transmissions.Clear();
            this.mutexTransmissions.ReleaseMutex();
        }
        public void RemoveTransmission(uint id)
        {
            this.transmissions.RemoveAll(em => em.Header.ID == id);
        }
        public void ProcessEmission(EmissionDescriptor emission)
        {
            if(emission.Header.FrameType != Emission.EmissionTypes.ReceivePacket)
                return;
            ReceivePacketEmission response = new ReceivePacketEmission();
            response.Unpack(emission.Data);
            List<byte> data = response.Data;
            switch(data[0])
            {
                case CONSTANTS.XM_HEADER_START_BYTE:
                    this.ProcessTransmissionObjectHeader(data);
                    break;
                case CONSTANTS.XM_PACKET_HEADER_START_BYTE:
                    this.ProcessTransmissionPacket(data);
                    break;
                default:
                    break;
            }
        }
        private void ProcessTransmissionObjectHeader(List<byte> data)
        {
            if(data.Count < 13)
                throw new Exception($"Invalid byte count ({data.Count}) when processing transmission object header");
            if(data[0] != CONSTANTS.XM_HEADER_START_BYTE)
                throw new Exception($"Invalid start byte {data[0]} when processing transmission object header");
            TransmissionObjectHeader header = new TransmissionObjectHeader();
            header.Unpack(data);
            this.headers.Add(header);
            this.TryCompleteTransmission(header.ID);
        }
        private void ProcessTransmissionPacket(List<byte> data)
        {
            if (data.Count < 11)
                throw new Exception($"Invalid byte count ({data.Count}) when processing transmission packet");
            if (data[0] != CONSTANTS.XM_PACKET_HEADER_START_BYTE)
                throw new Exception($"Invalid start byte {data[0]} when processing transmission packet");
            TransmissionChunk packet = new TransmissionChunk();
            packet.Unpack(data);
            if (packet.ValidChecksum)
            {
                this.chunks.Add(packet);
                this.TryCompleteTransmission(packet.Header.ObjectID);
            }
            else
            {
                Console.WriteLine($"Invalid checksum found on object {packet.Header.ObjectID} packet {packet.Header.ID}");
            }

        }
        private void TryCompleteTransmission(uint id)
        {
            int header_index = this.headers.FindIndex(h => h.ID == id);
            if(header_index < 0)
                return;
            uint expected_packet_count = this.headers[header_index].PacketCount;
            IEnumerable<TransmissionChunk> packets = this.chunks.Where(p => p.Header.ObjectID == id);
            if(packets.Count() < expected_packet_count)
                return;
            // TODO 5/29/21: assert one of each sequential packet IDs exist
            TransmissionObject xm = new TransmissionObject(this.headers[header_index], packets);
            this.headers.RemoveAt(header_index);
            this.chunks.RemoveAll(p=> p.Header.ObjectID == id);
            this.mutexTransmissions.WaitOne();
            this.transmissions.Add(xm);
            this.mutexTransmissions.ReleaseMutex();
            return;
        }
        public TransmissionObject WaitForTransmission(uint id)
        {
            TransmissionObject desc = null;
            while (desc == null)
            {
                this.mutexTransmissions.WaitOne();
                int index = this.transmissions.FindIndex(em => em.Header.ID == id);
                if (index > -1)
                {
                    desc = this.transmissions[index];
                    this.transmissions.RemoveAt(index);
                }
                this.mutexTransmissions.ReleaseMutex();
            }
            return desc;
        }
    }
}
