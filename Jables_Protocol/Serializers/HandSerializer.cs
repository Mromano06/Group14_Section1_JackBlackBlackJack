using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization logic for <see cref="HandDto"/> objects.
    /// Converts hand data to and from a binary format for network transmission.
    /// </summary>
    public class HandSerializer : ISerializer<HandDto>
    {
        /// <summary>
        /// Serializes a <see cref="HandDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The hand DTO to serialize.</param>
        /// <returns>
        /// A byte array representing the serialized hand data.
        /// </returns>
        /// <remarks>
        /// The serialized format is:
        /// <list type="number">
        /// <item><description>Card count (4 bytes)</description></item>
        /// <item><description>Serialized card data (2 bytes per card)</description></item>
        /// </list>
        /// Each card is serialized using <see cref="CardSerializer"/>.
        /// </remarks>
        public byte[] Serialize(HandDto dto)
        {
            if (dto.Cards == null || dto.Count == 0)
                return new byte[] { 0 };    // return a byte array with a single byte indicating zero cards

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(dto.Count);

            foreach (var card in dto.Cards)
            {
                var cardBytes = new CardSerializer().Serialize(card);
                bw.Write(cardBytes);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="HandDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized hand data.
        /// The data must follow the format produced by <see cref="Serialize(HandDto)"/>.
        /// </param>
        /// <returns>
        /// A <see cref="HandDto"/> reconstructed from the provided byte array.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to read the hand count or all cards.
        /// </exception>
        /// <remarks>
        /// This method first reads the card count, then reads card data in 2-byte chunks
        /// until the stream is exhausted.
        /// </remarks>
        public static HandDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            var handDto = new HandDto { Cards = new List<CardDto>() };
            handDto.Count = br.ReadInt32();

            if (handDto.Count == 0)
                return handDto;

            while (ms.Position < ms.Length)
            {
                var cardBytes = br.ReadBytes(2);
                var cardDto = CardSerializer.Deserialize(cardBytes);
                handDto.Cards.Add(cardDto);
            }

            return handDto;
        }
    }
}