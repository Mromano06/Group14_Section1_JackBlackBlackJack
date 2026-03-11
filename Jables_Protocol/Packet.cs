using System.ComponentModel;
using System.Numerics;
using SharedModels.Models;
using SharedModels.Core;
using GameLogic.Core;

// Sam Pelot
// WIP


namespace Jables_Protocol
{
    
    public class Packet
    {
        public PacketType Type {  get; set; }
        public int PayloadSize { get; set; }
        
        // initiallizes with an static empty cached array instance to avoid null references & multiple empty array instances
        public byte[] Payload { get; set; } = Array.Empty<byte>();

    }

    
}
