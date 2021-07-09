using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciConnect.Relay.Emission;

namespace AbaciConnect.Relay.Processors
{
    public interface IEmissionProcessor : IByteProcessor
    {
        List<EmissionDescriptor> GetAllEmissions();
        void ReceiveBytes(List<byte> bytes);
        void RemoveEmission(ulong id);
        EmissionDescriptor WaitForEmission(EmissionTypes type, int timeout);
        event EventHandler<EmissionDescriptor> OnEmission;
    }
}
