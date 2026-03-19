using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;
using SharedModels.Core;

namespace Jables_Protocol.Serializers
{
    public class GameUpdateSerializer : ISerializer<GameUpdateDto>
    {
        public byte[] Serialize(GameUpdateDto dto) 
        { 
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            // bet size
            bw.Write(dto.BetSize);

            // Card count & card list
            if (dto.Cards == null || dto.CardCount == 0)
            {
                bw.Write(dto.CardCount);
            }
            else
            {
                bw.Write(dto.CardCount);
                foreach (var card in dto.Cards)
                {
                    var cardBytes = new CardSerializer().Serialize(card);
                    bw.Write(cardBytes);
                }
            }

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

            return ms.ToArray();
        }

        public GameUpdateDto Deserialize(byte[] data)
        {
            GameUpdateDto dto = new GameUpdateDto();

            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            dto.BetSize = br.ReadDouble();
            dto.CardCount = br.ReadInt32();
            //byte[] byteHand = br.ReadBytes(2 * dto.CardCount);
            //var handDto = new HandSerializer().Deserialize(byteHand);
            if (dto.CardCount >= 0)
            {
                for (int i = 0; i < dto.CardCount; i++)
                {
                    byte[] byteshand = br.ReadBytes(2);
                    var cardDto = new CardSerializer().Deserialize(byteshand);
                    dto.Cards.Add(cardDto);
                }
            }

            dto.GameState = (GameStateEnum)br.ReadByte();

            dto.DealerCardCount = br.ReadInt32();

            if (dto.DealerCardCount >= 0)
            {
                for (int i = 0; i < dto.DealerCardCount; i++)
                {
                    byte[] byteshand = br.ReadBytes(2);
                    var cardDto = new CardSerializer().Deserialize(byteshand);
                    dto.DealerCards.Add(cardDto);
                }
            }

            dto.CurrentPlayerIndex = br.ReadInt32();

            return dto;
        }


    }
}
