using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization logic for <see cref="PlayerDto"/> objects.
    /// Converts player data to and from a binary format for network transmission.
    /// </summary>
    public class PlayerSerializer : ISerializer<DTOs.PlayerDto>
    {
        /// <summary>
        /// Serializes a <see cref="PlayerDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The player DTO to serialize.</param>
        /// <returns>A byte array representing the serialized player.</returns>
        /// <remarks>
        /// The serialized format is written in the following order:
        /// <list type="number">
        /// <item><description>Player name (UTF-8 bytes, null-terminated)</description></item>
        /// <item><description>Card count (4 bytes)</description></item>
        /// <item><description>Cards (2 bytes per card)</description></item>
        /// <item><description>Current bet (8 bytes, double)</description></item>
        /// <item><description>Has doubled flag (1 byte)</description></item>
        /// <item><description>Has insured flag (1 byte)</description></item>
        /// <item><description>Action count (4 bytes)</description></item>
        /// <item><description>Balance (8 bytes, double)</description></item>
        /// </list>
        /// </remarks>
        public byte[] Serialize(PlayerDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            // player name
            byte[] nameBytes = Encoding.UTF8.GetBytes(dto.Name ?? string.Empty);
            bw.Write(nameBytes);
            bw.Write((byte)0x00); // null terminator

            // player hand
            if (dto.Hand == null || dto.CardCount == 0)
            {
                bw.Write(dto.CardCount);
            }
            else
            {
                bw.Write(dto.CardCount);
                foreach (var card in dto.Hand)
                {
                    var cardBytes = new CardSerializer().Serialize(card);
                    bw.Write(cardBytes);
                }
            }

            // player current bet
            bw.Write(dto.CurrentBet);

            // player has doubled
            bw.Write(dto.HasDoubled);

            // player has insured
            bw.Write(dto.HasInsured);

            // player action count
            bw.Write(dto.ActionCount);

            // player balance
            bw.Write(dto.Balance);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="PlayerDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized player data.
        /// Must follow the format defined in <see cref="Serialize(PlayerDto)"/>.
        /// </param>
        /// <returns>A <see cref="PlayerDto"/> reconstructed from the byte array.</returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to fully reconstruct the object.
        /// </exception>
        /// <remarks>
        /// The player name is read as a null-terminated UTF-8 string.
        /// Card data is read based on the value of <c>CardCount</c>.
        /// </remarks>
        public static PlayerDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            var dto = new PlayerDto();

            // player name (null-terminated string)
            var nameBytes = new List<byte>();
            byte b;

            while ((b = br.ReadByte()) != 0)
            {
                nameBytes.Add(b);
            }

            dto.Name = Encoding.UTF8.GetString(nameBytes.ToArray());

            // player hand
            dto.CardCount = br.ReadInt32();

            dto.Hand = new List<CardDto>();
            if (dto.CardCount > 0)
            {
                for (int i = 0; i < dto.CardCount; i++)
                {
                    byte[] bytesHand = br.ReadBytes(2);
                    var cardDto = CardSerializer.Deserialize(bytesHand);
                    dto.Hand.Add(cardDto);
                }
            }

            // player current bet
            dto.CurrentBet = br.ReadDouble();

            // player has doubled
            dto.HasDoubled = br.ReadBoolean();

            // player has insured
            dto.HasInsured = br.ReadBoolean();

            // player action count
            dto.ActionCount = br.ReadInt32();

            // player balance
            dto.Balance = br.ReadDouble();

            return dto;
        }
    }
}