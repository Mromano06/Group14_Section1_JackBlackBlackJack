using System.ComponentModel;
using System.Numerics;
using SharedModels.Models;
using SharedModels.Core;
using GameLogic.Core;
using System.Security.Cryptography.X509Certificates;

// Sam Pelot

/// <summary>
/// Represents a network packet used for communication between client and server.
/// A packet consists of a type identifier, payload size, and raw payload data.
/// </summary>
namespace Jables_Protocol
{
    /// <summary>
    /// Gets or sets the type of the packet, indicating how the payload should be interpreted.
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Gets or sets the type of the packet, indicating how the payload should be interpreted.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// Gets or sets the size of the payload in bytes.
        /// This value should match the length of <see cref="Payload"/>.
        /// </summary>
        public int PayloadSize { get; set; }

        /// <summary>
        /// Gets or sets the raw payload data of the packet.
        /// Initialized to an empty array to avoid null references and unnecessary allocations.
        /// </summary>
        public byte[] Payload { get; set; } = Array.Empty<byte>();


        /// <summary>
        /// Serializes the current <see cref="Packet"/> instance into a byte array
        /// suitable for transmission over a network.
        /// </summary>
        /// <returns>
        /// A byte array containing the serialized packet data in the following order:
        /// <list type="bullet">
        /// <item><description>1 byte: Packet type</description></item>
        /// <item><description>4 bytes: Payload size (int)</description></item>
        /// <item><description>N bytes: Payload data</description></item>
        /// </list>
        /// </returns>
        public byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)Type);
            bw.Write(PayloadSize);
            bw.Write(Payload);

            return ms.ToArray();
        }

        /// <summary>
        /// Deserializes a byte array into a <see cref="Packet"/> instance.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized packet data.
        /// Must follow the format defined in <see cref="ToBytes"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Packet"/> object reconstructed from the provided byte array.
        /// </returns>
        /// <remarks>
        /// This method assumes the payload size stored in the byte array is valid.
        /// If the payload size is incorrect or maliciously altered, it may result in incomplete reads.
        /// </remarks>
        public static Packet FromBytes(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            
            PacketType type = (PacketType)br.ReadByte();

            int payloadSize = br.ReadInt32();
            byte[] payload = br.ReadBytes(payloadSize);

            return new Packet { Type = type, PayloadSize = payloadSize, Payload = payload };
        }
    }
}
