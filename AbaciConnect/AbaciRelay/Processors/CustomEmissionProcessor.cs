using AbaciConnect.Relay.Emission;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.Processors
{
    public class CustomEmissionProcessor : IEmissionProcessor
    {
        public byte StartByte => throw new NotImplementedException();

        public event EventHandler<EmissionDescriptor> OnEmission;

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public List<EmissionDescriptor> GetAllEmissions()
        {
            throw new NotImplementedException();
        }

        public void ReceiveBytes(List<byte> bytes)
        {
            throw new NotImplementedException();
        }

        public void RemoveEmission(ulong id)
        {
            throw new NotImplementedException();
        }

        public EmissionDescriptor WaitForEmission(EmissionTypes type, int timeout)
        {
            throw new NotImplementedException();
        }
    }
}
