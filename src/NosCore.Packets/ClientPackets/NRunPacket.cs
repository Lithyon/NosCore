﻿using NosCore.Core.Serializing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NosCore.Packets.ClientPackets
{
    [PacketHeader("n_run")]
    public class NrunPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public short Runner { get; set; }

        [PacketIndex(1)]
        public byte Type { get; set; }

        [PacketIndex(2)]
        public short Value { get; set; }

        [PacketIndex(3)]
        public int NpcId { get; set; }
    }
}