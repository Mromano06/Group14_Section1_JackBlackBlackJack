using System.ComponentModel;
using System.Numerics;

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
        // Packet Header
        public PacketType Type { get; set; }
        public GameStateEnum GameState { get; set; } 
        public byte NumCards { get; set; }
        public int PayloadSize { get; set; }

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

            // I still change payload.Length to PayloadSize - that's why I wrote it..
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

        // Deserialization

    }
}
