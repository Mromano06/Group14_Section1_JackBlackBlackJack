using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using SharedModels.Core;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization logic for <see cref="GameUpdateDto"/> objects.
    /// Converts full game update data to and from a binary format suitable for network transmission.
    /// </summary>
    public class GameUpdateSerializer : ISerializer<GameUpdateDto>
    {
        /// <summary>
        /// Serializes a <see cref="GameUpdateDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The game update data transfer object to serialize.</param>
        /// <returns>A byte array containing the serialized game update.</returns>
        /// <remarks>
        /// The serialized format is written in the following order:
        /// <list type="number">
        /// <item><description>Player byte length (4 bytes)</description></item>
        /// <item><description>Serialized player data (variable length)</description></item>
        /// <item><description>End-of-round flag (1 byte)</description></item>
        /// <item><description>Game state (1 byte)</description></item>
        /// <item><description>Dealer card count (4 bytes)</description></item>
        /// <item><description>Dealer cards (2 bytes per card)</description></item>
        /// <item><description>Current player index (4 bytes)</description></item>
        /// <item><description>Action result flag (1 byte)</description></item>
        /// <item><description>Round result (1 byte)</description></item>
        /// <item><description>Game result (1 byte)</description></item>
        /// </list>
        /// </remarks>
        public byte[] Serialize(GameUpdateDto dto) 
        { 
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            byte[] playerBytes = new PlayerSerializer().Serialize(dto.Player);
            bw.Write(playerBytes.Length);
            bw.Write(playerBytes);

            bw.Write(dto.IsEndRound);

            // game state enum
            bw.Write((byte)dto.GameState);

            // Dealer Card count & dealer card list
            if (dto.DealerCards == null || dto.DealerCardCount == 0)
            {
                bw.Write(dto.DealerCardCount);
            }
            else
            {
                bw.Write(dto.DealerCardCount);
                foreach (var card in dto.DealerCards)
                {
                    var cardBytes = new CardSerializer().Serialize(card);
                    bw.Write(cardBytes);
                }
            }

            // Player index
            bw.Write(dto.CurrentPlayerIndex);

            bw.Write(dto.ActionResult);

            bw.Write((byte)dto.RoundWin);

            bw.Write((byte)dto.gameResult);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="GameUpdateDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized game update data.
        /// The data must follow the format defined in <see cref="Serialize(GameUpdateDto)"/>.
        /// </param>
        /// <returns>A <see cref="GameUpdateDto"/> reconstructed from the provided byte array.</returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to fully reconstruct the object.
        /// </exception>
        /// <remarks>
        /// This method assumes the binary data is well-formed and that count fields
        /// such as player size and dealer card count are valid.
        /// </remarks>
        public static GameUpdateDto Deserialize(byte[] data)
        {
            GameUpdateDto dto = new GameUpdateDto();

            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            // Adding player data
            int playerSize = br.ReadInt32();

            byte[] playerBytes = br.ReadBytes(playerSize);
            PlayerDto playerDto = PlayerSerializer.Deserialize(playerBytes);
            dto.Player = playerDto;

            dto.IsEndRound = br.ReadBoolean();

            dto.GameState = (GameStateEnum)br.ReadByte();

            dto.DealerCardCount = br.ReadInt32();

            if (dto.DealerCardCount >= 0)
            {
                dto.DealerCards = new List<CardDto>();
                for (int i = 0; i < dto.DealerCardCount; i++)
                {
                    byte[] byteshand = br.ReadBytes(2);
                    var cardDto = CardSerializer.Deserialize(byteshand);
                    dto.DealerCards.Add(cardDto);
                }
            }

            dto.CurrentPlayerIndex = br.ReadInt32();

            dto.ActionResult = br.ReadBoolean();
            dto.RoundWin = (ROUND_RESULT)br.ReadByte();
            dto.gameResult = (GameResult)br.ReadByte();

            return dto;
        }


    }
}
