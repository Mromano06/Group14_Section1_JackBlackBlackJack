using Jables_Protocol.DTOs;
using System.IO;
using System.Text;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Serializes and deserializes <see cref="LoginDto"/> objects
    /// to and from binary format for network transmission.
    /// </summary>
    /// <remarks>
    /// Serialized format:
    /// - 4 bytes: Passcode (int)
    /// - 4 bytes: PlayerName length prefix (int)
    /// - N bytes: PlayerName (UTF-8 string)
    /// </remarks>
    public class LoginSerializer : ISerializer<LoginDto>
    {
        /// <summary>Serializes a <see cref="LoginDto"/> into a byte array.</summary>
        public byte[] Serialize(LoginDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.UTF8);

            bw.Write(dto.Passcode);
            bw.Write(dto.PlayerName ?? string.Empty);

            return ms.ToArray();
        }

        /// <summary>Deserializes a byte array into a <see cref="LoginDto"/>.</summary>
        public static LoginDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms, Encoding.UTF8);

            int passcode = br.ReadInt32();
            string name = br.ReadString();

            return new LoginDto { Passcode = passcode, PlayerName = name };
        }
    }
}