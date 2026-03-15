using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    public class HandSerializer : ISerializer<HandDto>
    {
        public byte[] Serialize(HandDto dto)
        {
            if(dto.Cards == null || dto.Count == 0)
                return new byte[] { 0 };    // return a byte array with a single byte indicating zero cards

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(dto.Count);

            foreach (var  card in dto.Cards)
            {
                var cardBytes = new CardSerializer().Serialize(card);
                bw.Write(cardBytes);        // Write the card bytes
            }

            return ms.ToArray();
        }

        public HandDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            var handDto = new HandDto { Cards = new List<CardDto>() };
            handDto.Count = br.ReadInt32();

            // I think this is going to cause some problems
            if (ms.Length == 0 || handDto.Count == 0) // because technically, it's possible to have an empty hand.
                return handDto; // Return an empty hand if there are no bytes

            while (ms.Position < ms.Length)
            {
                var cardBytes = br.ReadBytes(2); // Read 2 bytes for each card
                var cardDto = new CardSerializer().Deserialize(cardBytes);
                handDto.Cards.Add(cardDto);
            }

            return handDto;
        }

    }
}
