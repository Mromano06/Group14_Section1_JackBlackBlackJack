using Jables_Protocol.DTOs;
using SharedModels.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides serialization and deserialization logic for <see cref="GameStateDto"/> objects.
    /// Converts game state data to and from a compact binary format.
    /// </summary>
    public class GameStateSerializer : ISerializer<GameStateDto>
    {
        /// <summary>
        /// Serializes a <see cref="GameStateDto"/> into a byte array.
        /// </summary>
        /// <param name="dto">The game state DTO to serialize.</param>
        /// <returns>A byte array representing the serialized game state.</returns>
        /// <remarks>
        /// The serialized format consists of:
        /// <list type="bullet">
        /// <item><description>1 byte: Game state enum value</description></item>
        /// </list>
        /// </remarks>
        public byte[] Serialize(GameStateDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)dto.GameState);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="GameStateDto"/>.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized game state data.
        /// Must be at least 1 byte in length.
        /// </param>
        /// <returns>
        /// A <see cref="GameStateDto"/> reconstructed from the byte array.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the byte array does not contain enough data to read the game state.
        /// </exception>
        /// <remarks>
        /// This method assumes the byte represents a valid <see cref="GameStateEnum"/> value.
        /// No validation is performed on the enum value.
        /// </remarks>
        public static GameStateDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            byte gameState = br.ReadByte();

            return new GameStateDto
            {
                GameState = (GameStateEnum)gameState
            };
        }
    }
}