using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Emission;

namespace AbaciConnect.Relay.Processors
{
    public class ApiFrameEmissionProcessor : IEmissionProcessor
    {
        private readonly Mutex mutexEmissions = new Mutex();
        private readonly Mutex mutexBytes = new Mutex();
        private readonly List<EmissionDescriptor> emissions = new List<EmissionDescriptor>();
        private EmissionHeader currentHeader = new EmissionHeader();
        private List<byte> pendingBytes = new List<byte>();
        public event EventHandler<EmissionDescriptor> OnEmission;
        public enum ReceiveStates : int
        {
            ReceiveStartByte = 0,
            ReceiveHeader = 1,
            ReceiveFrame = 2
        }
        public ReceiveStates ReceiveState { get; private set; } = ReceiveStates.ReceiveStartByte;
        public byte StartByte => CONSTANTS.FRAME_START_BYTE;
        public void Clear()
        {
            this.mutexEmissions.WaitOne();
            this.emissions.Clear();
            this.mutexEmissions.ReleaseMutex();
            this.mutexBytes.WaitOne();
            this.pendingBytes.Clear();
            this.mutexBytes.ReleaseMutex();
        }
        public List<EmissionDescriptor> GetAllEmissions()
        {
            return this.emissions;
        }
        public void ReceiveBytes(List<byte> bytes)
        {
            this.pendingBytes.AddRange(bytes);
            bool success = false;
            do
            {
                switch (this.ReceiveState)
                {
                    case ReceiveStates.ReceiveStartByte:
                        success = this.ReceiveStartByte();
                        break;
                    case ReceiveStates.ReceiveHeader:
                        success = this.ReceiveHeader();
                        break;
                    case ReceiveStates.ReceiveFrame:
                        success = this.ReceiveFrame();
                        break;
                    default:
                        throw new NotImplementedException("Receive state not implemented");
                }
            }
            while(success);
        }
        private bool ReceiveStartByte()
        {
            // find start byte
            bool success = false;
            int index = this.pendingBytes.FindIndex(b => b == CONSTANTS.FRAME_START_BYTE);
            // determine success
            if(index < 0)       // if start byte not found, remove all bytes
                index = this.pendingBytes.Count;
            else
            { 
                this.ReceiveState = ReceiveStates.ReceiveHeader;
                success = true; // if start byte found, remove bytes leading up to it and indicate success
            }
            // remove all bytes leading up to start byte
            if (index > 0)
                this.pendingBytes.RemoveRange(0, index);
            return success;
        }
        private bool ReceiveFrame()
        {
            // check if enough bytes ready
            ushort required_count = this.currentHeader.Length;
            if(this.pendingBytes.Count < required_count)
                return false;
            this.ReceiveState = ReceiveStates.ReceiveStartByte;
            // create emission description
            EmissionDescriptor desc = new EmissionDescriptor(this.currentHeader,
                this.pendingBytes.Take(required_count).ToList());
            this.mutexEmissions.WaitOne();
            this.emissions.Add(desc);
            this.mutexEmissions.ReleaseMutex();
            // remove emission bytes
            this.pendingBytes.RemoveRange(0, required_count);
            // invoke callback
            if(this.OnEmission != null)
                this.OnEmission(this, desc);
            return true;
        }
        public void RemoveEmission(ulong id)
        {
            this.emissions.RemoveAll(em => em.ID == id);
        }
        private bool ReceiveHeader()
        {
            // check if enough bytes ready
            int required_count = CONSTANTS.EMISSION_HEADER_SIZE;
            if (this.pendingBytes.Count < required_count)
                return false;
            this.ReceiveState = ReceiveStates.ReceiveFrame;
            // decode header
            this.currentHeader.Unpack(this.pendingBytes.Take(required_count).ToList());
            // remove header bytes
            this.pendingBytes.RemoveRange(0, required_count);
            return true;
        }
        public EmissionDescriptor WaitForEmission(EmissionTypes type, int timeout)
        {
            EmissionDescriptor desc = null;
            DateTime start = DateTime.Now;
            while (desc == null)
            {
                if(DateTime.Now > start + TimeSpan.FromMilliseconds(timeout))
                    throw new TimeoutException("Timeout while waiting for emission");
                this.mutexEmissions.WaitOne();
                int index = this.emissions.FindIndex(em => em.Header.FrameType == type);
                if(index > -1)
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
