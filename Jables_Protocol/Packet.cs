using System.ComponentModel;
using System.Numerics;
using SharedModels.Models;
using SharedModels.Core;
using GameLogic.Core;
using System.Security.Cryptography.X509Certificates;

// Sam Pelot
// WIP

namespace Jables_Protocol
{

    public class Packet
    {
        public PacketType Type { get; set; }
        public int PayloadSize { get; set; }

        // initiallizes with an static empty cached array instance to avoid null references & multiple empty array instances
        public byte[] Payload { get; set; } = Array.Empty<byte>();


        // To Bytes
        public byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((byte)Type);
            bw.Write(PayloadSize);
            bw.Write(Payload);

            return ms.ToArray();
        }

        // From Bytes
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
