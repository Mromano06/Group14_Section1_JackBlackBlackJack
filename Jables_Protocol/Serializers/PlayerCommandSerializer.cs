using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

// TODO: Write the PlayerCommand DTO

namespace Jables_Protocol.Serializers
{
    /// <summary>
    ///     Serializes and deserializes PlayerCommandDto objects to and from byte arrays for network transmission.
    /// </summary>
    internal class PlayerCommandSerializer : ISerializer<PlayerCommandDto>
    {
        public byte[] Serialize(PlayerCommandDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.Action);
            bw.Write(dto.BetAmount);

            return ms.ToArray();
        }

        public PlayerCommandDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            byte packetType = br.ReadByte();
            int betAmount = br.ReadInt32();

            return new PlayerCommandDto { Action = (PlayerAction)packetType, BetAmount = betAmount };
        }
    }
}
