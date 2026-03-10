using System.ComponentModel;
using System.Numerics;
using SharedModels.Models;
using SharedModels.Core;
using GameLogic.Core;

// Sam Pelot
// WIP

// TODO:
// Restructure SerializeData
// - probably should just take a Packet object
// - should add total length into the header fields
// - should probably make header fields private
// Ask team about GameStateEnum
// - should it be moved to SharedModels?
// - should it be : byte to save space?
// Ask team about message types to make sure we're all on the same page.
// Deserialization

// should header be an interface implemented by a packet superclass?
// - would allow for more flexible packet types and easier deserialization
// - would also allow for more efficient serialization by only writing the fields that are relevant to the packet type
// - would also allow for more efficient deserialization by only reading the fields that are relevant to the packet type
// - would allow for packet subclasses to have their own specific fields without bloating the base packet class
// should we use a BinaryReader for deserialization?
// should we use a BinaryWriter or just write to a byte array directly?



namespace Jables_Protocol
{
    public enum PacketType : byte   // set the size of enum to byte
    {
        Error,
        PlayerAction,
        StateUpdate,
        CardsDealt,
        JoinRequest
    }
    public class Packet
    {
        // Main Packet Header
        private PacketType _type;
        public PacketType Type { get => _type; set => _type = value; }

        private int _payloadSize;
        public int PayloadSize { get => _payloadSize; set => _payloadSize = value; }

        // Other Header Fields
        public GameStateEnum GameState { get; set; }
        public byte NumCards { get; set; }
        

        // Packet Body
        public byte[]? DataField { get; set; }


        // Default Constructor
        public Packet()
        {
            GameState = GameStateEnum.IDLE;
            NumCards = 0;
            DataField = null;
        }

        // Constructor
        public Packet(GameStateEnum gameState, byte numCards, byte[]? payload)
        {
            GameState = gameState;
            this.NumCards = numCards;
            this.DataField = payload;
        }

        // Serialization - alternate version VS just spit out and looked interesting.
        //public static byte[] SerliazeData(GameStateEnum gameState, byte numCards, byte[]? payload)
        //{
        //    List<byte> data = new List<byte>();
        //    // Add GameState
        //    data.AddRange(BitConverter.GetBytes((int)gameState));
        //    // Add numCards
        //    data.Add(numCards);
        //    // Add payload if it exists
        //    if (payload != null)
        //    {
        //        data.AddRange(payload);
        //    }
        //    return data.ToArray();
        //}

        // I need to restructure this after talking with the team. 
        // state updates are a particular packet type.
        // Same with numCards, both of which would be part of the payload.
        public static byte[] SerializeData(PacketType type, GameStateEnum state, byte numCards, byte[]? payload)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);    // BinaryWriter writes little-endian as an FYI

            // I still need to change payload.Length to PayloadSize - that's why I wrote it..
            payload ??= Array.Empty<byte>();
            int payloadSize = payload.Length;

            int totalLength = sizeof(int) + sizeof(byte) + sizeof(int) +    // totalLength + PacketType + GameStateEnum + 
                              sizeof(byte) + sizeof(int) + payloadSize;     // numCards + payloadSize + size of payload

            bw.Write(totalLength);
            bw.Write((byte)type);
            bw.Write((int)state);
            bw.Write(numCards);
            bw.Write(payload);

            return ms.ToArray();
        }

        public static byte[] SerializeData(Packet packet)
        {
            using
            return SerializeData(packet.Type, packet.GameState, packet.NumCards, packet.DataField);
        }

        // Deserialization

    }
}
