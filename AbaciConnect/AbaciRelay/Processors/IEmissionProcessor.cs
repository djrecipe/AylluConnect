using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciConnect.Relay.Emission;

namespace AbaciConnect.Relay.Processors
{
    interface IEmissionProcessor : IByteProcessor
    {
        public List<EmissionDescriptor> GetAllEmissions();
        public void ReceiveBytes(List<byte> bytes);
        public void RemoveEmission(ulong id);
        public EmissionDescriptor WaitForEmission(EmissionTypes type, int timeout);
        public event EventHandler<EmissionDescriptor> OnEmission;
    }
}
