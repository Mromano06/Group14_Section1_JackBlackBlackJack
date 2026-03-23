using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    public class CardSerializer : ISerializer<CardDto>
    {
        public byte[] Serialize(CardDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.Rank);
            bw.Write((byte)dto.Suit);

            return ms.ToArray();
        }

        public static CardDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            char rank = (char)br.ReadByte();
            char suit = (char)br.ReadByte();
            return new CardDto { Rank = rank, Suit = suit };
        }
    }
}
