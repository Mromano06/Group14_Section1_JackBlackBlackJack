using Jables_Protocol.DTOs;
using System.IO;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Serializes and deserializes <see cref="LoginResponseDto"/> objects
    /// to and from binary format for network transmission.
    /// </summary>
    /// <remarks>
    /// Serialized format:
    /// - 1 byte:  Accepted (bool)
    /// - 4 bytes: Message length prefix (int)
    /// - N bytes: Message (UTF-8 string)
    /// </remarks>


    [ExcludeFromCodeCoverage]
    public class LoginResponseSerializer : ISerializer<LoginResponseDto>
    {
        /// <summary>Serializes a <see cref="LoginResponseDto"/> into a byte array.</summary>
        public byte[] Serialize(LoginResponseDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.UTF8);

            bw.Write(dto.Accepted);
            bw.Write(dto.Message ?? string.Empty);

            return ms.ToArray();
        }

        /// <summary>Deserializes a byte array into a <see cref="LoginResponseDto"/>.</summary>
        public static LoginResponseDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms, Encoding.UTF8);

            bool accepted = br.ReadBoolean();
            string message = br.ReadString();

            return new LoginResponseDto { Accepted = accepted, Message = message };
        }
    }
}