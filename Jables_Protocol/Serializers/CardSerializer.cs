using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization logic for <see cref="CardDto"/> objects.
    /// Converts card data to and from a compact binary format for network transmission.
    /// </summary>
    public class CardSerializer : ISerializer<CardDto>
    {
        /// <summary>
        /// Serializes a <see cref="CardDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The card data transfer object to serialize.</param>
        /// <returns>
        /// A byte array representing the serialized card.
        /// </returns>
        /// <remarks>
        /// The serialized format is:
        /// <list type="bullet">
        /// <item><description>1 byte: Rank (ASCII character)</description></item>
        /// <item><description>1 byte: Suit (ASCII character)</description></item>
        /// </list>
        /// Total size: 2 bytes.
        /// </remarks>
        public byte[] Serialize(CardDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.Rank);
            bw.Write((byte)dto.Suit);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="CardDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized card data.
        /// Must be at least 2 bytes in length.
        /// </param>
        /// <returns>
        /// A <see cref="CardDto"/> reconstructed from the byte array.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to read both rank and suit.
        /// </exception>
        /// <remarks>
        /// This method assumes the byte array follows the format defined in <see cref="Serialize"/>.
        /// No validation is performed on the resulting rank or suit values.
        /// </remarks>
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
