using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Jables_Protocol.Serializers
{
    public class PlayerSerializer : ISerializer<DTOs.PlayerDto>
    {
        public byte[] Serialize(PlayerDto dto)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            // player name
            byte[] nameBytes = Encoding.UTF8.GetBytes(dto.Name ?? string.Empty);
            bw.Write(nameBytes);
            bw.Write((byte)0x00); // adding a null terminator 

            // player hand

            if (dto.Hand == null || dto.CardCount == 0)
            {
                bw.Write(dto.CardCount);
            }
            else
            {
                bw.Write(dto.CardCount);
                foreach (var card in dto.Hand)
                {
                    var cardBytes = new CardSerializer().Serialize(card);
                    bw.Write(cardBytes);        // Write the card bytes
                }
            }

            // player current bet
            bw.Write(dto.CurrentBet);

            // player has doubled
            bw.Write(dto.HasDoubled);

            // player has insured
            bw.Write(dto.HasInsured);

            // player action count
            bw.Write(dto.ActionCount);

            // player balance
            bw.Write(dto.Balance);

            return ms.ToArray();
        }

        public static PlayerDto Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            var dto = new PlayerDto();

            // player name (Very scuffed)
            var nameBytes = new List<byte>();
            byte b;

            while ((b = br.ReadByte()) != 0) {
                nameBytes.Add(b);
            }

            dto.Name = Encoding.UTF8.GetString(nameBytes.ToArray());

            // player hand - can handle a max of
            dto.CardCount = br.ReadInt32();

            //byte[] byteHand = br.ReadBytes(2 * dto.CardCount);
            //var handDto = new HandSerializer().Deserialize(byteHand);
            //dto.Hand = handDto;
            dto.Hand = new List<CardDto>(); // Initialized before we attempt to use it
            if (dto.CardCount > 0)
            {
                for (int i = 0; i < dto.CardCount; i++)
                {
                    byte[] bytesHand = br.ReadBytes(2);
                    var cardDto = CardSerializer.Deserialize(bytesHand);
                    dto.Hand.Add(cardDto);
                }
            }

            // player current bet
            dto.CurrentBet = br.ReadDouble();

            // player has doubled
            dto.HasDoubled = br.ReadBoolean();

            // player has insured
            dto.HasInsured = br.ReadBoolean();

            // player action count
            dto.ActionCount = br.ReadInt32();

            // player balance
            dto.Balance = br.ReadDouble();

            return dto;
        }
    }
}
