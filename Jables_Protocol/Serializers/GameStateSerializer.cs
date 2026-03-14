using Jables_Protocol.DTOs;
using SharedModels.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    internal class GameStateSerializer : ISerializer<GameStateDto>
    {
        public byte[] Serialize(GameStateDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.GameState);

            return ms.ToArray();
        }

        public GameStateDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            byte gameState = br.ReadByte();

            return new GameStateDto { GameState = (GameStateEnum)gameState };
        }
    }
}
