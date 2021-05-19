﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public enum DeviceRoles : byte
    {
        Join = 0,
        Create = 1
    }
    public class NetworkSettings
    {
        public bool EnableEncryption { get;set;} = false;
        public byte MaxTransmissionSize { get;set;} = 0xFF;
        public ulong NetworkID { get; set; } = 0xABCD;
        public byte NodeDiscoveryDelay { get;set;} = 0x20;
        public byte NodeJoinTime { get;set;} = 0xFF;
        public DeviceRoles Role { get;set;} = DeviceRoles.Join;
    }
}
