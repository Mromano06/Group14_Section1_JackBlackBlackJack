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

            return ms.ToArray();
        }

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

            dto = new GameUpdateDto();

            return dto;
        }


    }
}
