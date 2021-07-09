using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Emission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AbaciConnect.Relay.Processors
{
    public class MicropyEmissionProcessor : IEmissionProcessor
    {
        private readonly Mutex mutexEmissions = new Mutex();
        private readonly List<EmissionDescriptor> emissions = new List<EmissionDescriptor>();
        public byte StartByte => throw new NotImplementedException();

        public event EventHandler<EmissionDescriptor> OnEmission;

        public void Clear()
        {
        }

        public List<EmissionDescriptor> GetAllEmissions()
        {
            throw new NotImplementedException();
        }

        public void ReceiveBytes(List<byte> bytes)
        {
            // check start byte
            if(bytes[0] != CONSTANTS.FRAME_START_BYTE)
                throw new Exception("Invalid start byte for custom emission processor");
            // decode header
            int required_count = CONSTANTS.EMISSION_HEADER_SIZE;
            if(bytes.Count < required_count)
                throw new Exception("Invalid byte count for custom emission processor header");
            EmissionHeader header = new EmissionHeader();
            header.Unpack(bytes.Take(required_count).ToList());
            // remove header bytes
            bytes.RemoveRange(0, required_count);
            // decode emission descriptor
            required_count = header.Length;
            if (bytes.Count < required_count)
                throw new Exception("Invalid byte count for custom emission processor data");
            EmissionDescriptor desc = new EmissionDescriptor(header,
                bytes.Take(required_count).ToList());
            // signal done
            this.mutexEmissions.WaitOne();
            this.emissions.Add(desc);
            this.mutexEmissions.ReleaseMutex();
        }

        public void RemoveEmission(ulong id)
        {
        }

        public EmissionDescriptor WaitForEmission(EmissionTypes type, int timeout)
        {

            EmissionDescriptor desc = null;
            DateTime start = DateTime.Now;
            while (desc == null)
            {
                if (DateTime.Now > start + TimeSpan.FromMilliseconds(timeout))
                    throw new TimeoutException("Timeout while waiting for emission");
                this.mutexEmissions.WaitOne();
                int index = this.emissions.FindIndex(em => em.Header.FrameType == type);
                if (index > -1)
                {
                    desc = this.emissions[index];
                    this.emissions.RemoveAt(index);
                }
                this.mutexEmissions.ReleaseMutex();
            }
            return desc;
        }
    }
}
