using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AbaciConnect.Relay.EmissionStructures;

namespace AbaciConnect.Relay
{
    public class FrameByteReceiver : IByteReceiver
    {
        private readonly Mutex mutexEmissions = new Mutex();
        private readonly List<EmissionDescriptor> emissions = new List<EmissionDescriptor>();
        private EmissionHeader currentHeader = new EmissionHeader();
        private List<byte> pendingBytes = new List<byte>();
        public enum ReceiveStates : int
        {
            ReceiveStartByte = 0,
            ReceiveHeader = 1,
            ReceiveFrame = 2
        }
        public ReceiveStates ReceiveState { get; private set; } = ReceiveStates.ReceiveStartByte;
        public byte StartByte => CONSTANTS.START_BYTE;
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
            int index = this.pendingBytes.FindIndex(b => b == CONSTANTS.START_BYTE);
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
            this.mutexEmissions.WaitOne();
            this.emissions.Add(new EmissionDescriptor(this.currentHeader, this.pendingBytes.Take(required_count).ToList()));
            this.mutexEmissions.ReleaseMutex();
            // remove emission bytes
            this.pendingBytes.RemoveRange(0, required_count);
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
        public EmissionDescriptor WaitForEmission(EmissionTypes type)
        {
            EmissionDescriptor desc = null;
            while(desc == null)
            {
                this.mutexEmissions.WaitOne();
                int index = this.emissions.FindIndex(em => em.Header.FrameType == type);
                if(index > -1)
                {
                    desc = this.emissions[index];
                    this.emissions.RemoveAt(index);
                }
                this.mutexEmissions.ReleaseMutex();
                //Thread.Sleep(10);
            }
            return desc;
        }
    }
}
