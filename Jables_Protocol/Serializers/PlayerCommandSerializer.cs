using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Serializes and deserializes <see cref="PlayerCommandDto"/> objects
    /// to and from a binary format for network transmission.
    /// </summary>
    /// <remarks>
    /// This serializer is used for client-to-server communication,
    /// where the client sends player actions (e.g., hit, stand, bet).
    /// </remarks>
    public class PlayerCommandSerializer : ISerializer<PlayerCommandDto>
    {
        /// <summary>
        /// Serializes a <see cref="PlayerCommandDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The player command DTO to serialize.</param>
        /// <returns>A byte array representing the serialized command.</returns>
        /// <remarks>
        /// The serialized format is:
        /// <list type="bullet">
        /// <item><description>1 byte: Player action (enum)</description></item>
        /// <item><description>8 bytes: Bet amount (double)</description></item>
        /// </list>
        /// Total size: 9 bytes.
        /// </remarks>
        public byte[] Serialize(PlayerCommandDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.Action);
            bw.Write(dto.BetAmount);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="PlayerCommandDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized command data.
        /// Must be at least 9 bytes in length.
        /// </param>
        /// <returns>
        /// A <see cref="PlayerCommandDto"/> reconstructed from the byte array.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to read both fields.
        /// </exception>
        /// <remarks>
        /// This method assumes the byte array follows the format defined in <see cref="Serialize"/>.
        /// No validation is performed on the action or bet amount.
        /// </remarks>
        public static PlayerCommandDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            byte packetType = br.ReadByte();
            double betAmount = br.ReadDouble();

            return new PlayerCommandDto
            {
                Action = (PlayerAction)packetType,
                BetAmount = betAmount
            };
        }
    }
}