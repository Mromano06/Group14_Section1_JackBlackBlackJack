using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Actions.ActionTypes;
using Jables_Protocol.DTOs;
using Jables_Protocol.Serializers;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.EventHandlers;
using SharedModels.Core;
using GameLogic.Core;

namespace Jables_Protocol.Tests
{
    [TestClass]
    public class CardDtoTests
    {
        [TestMethod]
        public void CardDtoSerializer()
        {
            // Arrange
            CardSerializer cardSerializer = new CardSerializer();
            CardDto card = new CardDto { Rank = 'A', Suit = 'S' };
            char expectedRank = 'A';
            char expectedSuit = 'S';

            // Act
            byte[] cardBuf = cardSerializer.Serialize(card);
            char actualRank = (char)cardBuf[0];
            char actualSuit = (char)cardBuf[1];

            // Assert
            Assert.AreEqual(expectedRank, actualRank);
            Assert.AreEqual(expectedSuit, actualSuit);
        }

        [TestMethod]
        public void CardDtoDeserializer()
        {
            // Arrange
            CardSerializer cardSerializer = new CardSerializer();
            CardDto card = new CardDto { Rank = 'A', Suit = 'S' };
            char expectedRank = 'A';
            char expectedSuit = 'S';
            char actualRank = '\0';
            char actualSuit = '\0';

            // Act
            byte[] cardBuf = cardSerializer.Serialize(card);
            CardDto deserializedCard = CardSerializer.Deserialize(cardBuf);
            actualRank = deserializedCard.Rank;
            actualSuit = deserializedCard.Suit;

            // Assert
            Assert.AreEqual(expectedRank, actualRank);
            Assert.AreEqual(expectedSuit, actualSuit);
        }
    }

    [TestClass]
    public class HandDtoTests
    {
        [TestMethod]
        public void HandDtoSerializer()
        {
            // Arrange
            HandSerializer handSerializer = new HandSerializer();
            int count = 2;
            CardDto card1 = new CardDto { Rank = 'A', Suit = 'S' };
            CardDto card2 = new CardDto { Rank = 'J', Suit = 'H' };
            HandDto handDto = new HandDto();
            handDto.Count = count;
            handDto.Cards = new List<CardDto> { card1, card2 };

            // Act
            byte[] handBuf = handSerializer.Serialize(handDto);
            int handCount = handBuf[0];
            char card1Rank = (char)handBuf[4];
            char card1Suit = (char)handBuf[5];
            char card2Rank = (char)handBuf[6];
            char card2Suit = (char)handBuf[7];


            // Assert
            Assert.AreEqual(count, handCount);
            Assert.AreEqual(card1.Rank, card1Rank);
            Assert.AreEqual(card1.Suit, card1Suit);
            Assert.AreEqual(card2.Rank, card2Rank);
            Assert.AreEqual(card2.Suit, card2Suit);
        }

        [TestMethod]
        public void HandDtoDeserializer()
        {
            // Arrange
            HandSerializer handSerializer = new HandSerializer();
            int count = 2;
            CardDto card1 = new CardDto { Rank = 'A', Suit = 'S' };
            CardDto card2 = new CardDto { Rank = 'J', Suit = 'H' };
            HandDto handDto = new HandDto();
            handDto.Count = count;
            handDto.Cards = new List<CardDto> { card1, card2 };

            // Act
            byte[] handBuf = handSerializer.Serialize(handDto);
            //char card1Suit = (char)handBuf[1];
            //char card1Rank = (char)handBuf[2];
            //char card2Suit = (char)handBuf[3];
            //char card2Rank = (char)handBuf[4];

            HandDto deserializedHand = HandSerializer.Deserialize(handBuf);
            int handCount = deserializedHand.Count;

            // Assert
            Assert.AreEqual(count, deserializedHand.Count);
            Assert.AreEqual(card1.Rank, deserializedHand.Cards[0].Rank);
            Assert.AreEqual(card1.Suit, deserializedHand.Cards[0].Suit);
            Assert.AreEqual(card2.Rank, deserializedHand.Cards[1].Rank);
            Assert.AreEqual(card2.Suit, deserializedHand.Cards[1].Suit);
        }
    }

    [TestClass]
    public class PlayerSerializerTests
    {
        [TestMethod]
        public void PlayerDtoSerializer()
        {
            // Arrange
            PlayerSerializer playerSerializer = new PlayerSerializer();

            PlayerDto playerDto = new PlayerDto
            {
                Name = "CoolHand Luke",
                CardCount = 2,
                Hand = new List<CardDto>
            {
                new CardDto { Rank = 'A', Suit = 'S' },
                new CardDto { Rank = 'J', Suit = 'H' }
            },
                CurrentBet = 50.0,
                HasDoubled = false,
                HasInsured = true,
                ActionCount = 1,
                Balance = 1000.0
            };

            // Act
            byte[] playerBuf = playerSerializer.Serialize(playerDto);

            // Assert
            using var ms = new MemoryStream(playerBuf);
            using var br = new BinaryReader(ms);

            // Name: read until null terminator
            List<byte> nameBytes = new List<byte>();
            byte currentByte;
            while ((currentByte = br.ReadByte()) != 0x00)
            {
                nameBytes.Add(currentByte);
            }

            string name = Encoding.UTF8.GetString(nameBytes.ToArray());

            int cardCount = br.ReadInt32();

            char card1Rank = (char)br.ReadByte();
            char card1Suit = (char)br.ReadByte();
            char card2Rank = (char)br.ReadByte();
            char card2Suit = (char)br.ReadByte();

            double currentBet = br.ReadDouble();
            bool hasDoubled = br.ReadBoolean();
            bool hasInsured = br.ReadBoolean();
            int actionCount = br.ReadInt32();
            double balance = br.ReadDouble();

            Assert.AreEqual(playerDto.Name, name);
            Assert.AreEqual(playerDto.CardCount, cardCount);

            Assert.AreEqual(playerDto.Hand[0].Rank, card1Rank);
            Assert.AreEqual(playerDto.Hand[0].Suit, card1Suit);
            Assert.AreEqual(playerDto.Hand[1].Rank, card2Rank);
            Assert.AreEqual(playerDto.Hand[1].Suit, card2Suit);

            Assert.AreEqual(playerDto.CurrentBet, currentBet);
            Assert.AreEqual(playerDto.HasDoubled, hasDoubled);
            Assert.AreEqual(playerDto.HasInsured, hasInsured);
            Assert.AreEqual(playerDto.ActionCount, actionCount);
            Assert.AreEqual(playerDto.Balance, balance);
        }

        [TestMethod]
        public void PlayerDtoDeserializer()
        {
            // Arrange
            PlayerSerializer playerSerializer = new PlayerSerializer();
            PlayerDto playerDto = new PlayerDto()
            {
                Name = "CoolHand Luke",
                CardCount = 2,
                Hand = new List<CardDto>
            {
                new CardDto { Rank = 'A', Suit = 'S' },
                new CardDto { Rank = 'J', Suit = 'H' }
            },
                CurrentBet = 50.0,
                HasDoubled = false,
                HasInsured = true,
                ActionCount = 1,
                Balance = 1000.0
            };

            // Act
            byte[] playerBuf = playerSerializer.Serialize(playerDto);
            PlayerDto deserPlayerDto = PlayerSerializer.Deserialize(playerBuf);

            // Assert
            Assert.AreEqual(playerDto.Name, deserPlayerDto.Name);
            Assert.AreEqual(playerDto.CardCount, deserPlayerDto.CardCount);
            Assert.AreEqual(playerDto.Hand.Count, deserPlayerDto.Hand.Count);
            Assert.AreEqual(playerDto.Hand[0].Rank, deserPlayerDto.Hand[0].Rank);
            Assert.AreEqual(playerDto.Hand[0].Suit, deserPlayerDto.Hand[0].Suit);
            Assert.AreEqual(playerDto.Hand[1].Rank, deserPlayerDto.Hand[1].Rank);
            Assert.AreEqual(playerDto.Hand[1].Suit, deserPlayerDto.Hand[1].Suit);
            Assert.AreEqual(playerDto.CurrentBet, deserPlayerDto.CurrentBet);
            Assert.AreEqual(playerDto.HasDoubled, deserPlayerDto.HasDoubled);
            Assert.AreEqual(playerDto.HasInsured, deserPlayerDto.HasInsured);
            Assert.AreEqual(playerDto.ActionCount, deserPlayerDto.ActionCount);
            Assert.AreEqual(playerDto.Balance, deserPlayerDto.Balance);
        }
    }

    [TestClass]
    public class PlayerCommandDtoTests
    {
        [TestMethod]
        public void PlayerCommandDto_Serialize_Deserialize()
        {
            // Arrange
            PlayerCommandSerializer serializer = new PlayerCommandSerializer();
            PlayerCommandDto dto = new PlayerCommandDto()
            {
                Action = PlayerAction.Bet,
                BetAmount = 50
            };

            // Act
            byte[] playerComBuf = serializer.Serialize(dto);
            PlayerCommandDto deserPlayerComDto = PlayerCommandSerializer.Deserialize(playerComBuf);

            // Assert
            Assert.AreEqual(dto.Action, deserPlayerComDto.Action);
            Assert.AreEqual(dto.BetAmount, deserPlayerComDto.BetAmount);
        }
    }

    [TestClass]
    public class GameUpdateDtoTests
    {
        [TestMethod]
        public void GameUpdateDto_Serializer_Deserialize()
        {
            // Arrange
            GameUpdateSerializer serializer = new GameUpdateSerializer();
            GameUpdateDto dto = new GameUpdateDto()
            {
                Player = new PlayerDto()
                {
                    Name = "Sam",
                    CardCount = 2,
                    Hand = new List<CardDto>
                    {
                        new CardDto { Rank = 'A', Suit = 'S' },
                        new CardDto { Rank = 'J', Suit = 'H' }
                    },
                    CurrentBet = 50.0,
                    HasDoubled = false,
                    HasInsured = true,
                    ActionCount = 1,
                    Balance = 1000.0
                },
                IsEndRound = false,
                GameState = GameStateEnum.PLAYING,
                DealerCardCount = 2,
                DealerCards = new List<CardDto>
                {
                    new CardDto { Rank = 'K', Suit = 'D' },
                    new CardDto { Rank = '5', Suit = 'C' }
                },
                CurrentPlayerIndex = 0,
                ActionResult = true,
                RoundWin = ROUND_RESULT.WIN
            };

            // Act
            byte[] dtoBuf = serializer.Serialize(dto);
            GameUpdateDto deserDto = GameUpdateSerializer.Deserialize(dtoBuf);

            // Assert
            Assert.AreEqual(dto.Player.Name, deserDto.Player.Name);
            Assert.AreEqual(dto.ActionResult, deserDto.ActionResult);
        }
    }

    [TestClass]
    public class GameStateDtoTests
    {
        [TestMethod]
        public void GameStateDto_Serializer_Deserialize()
        {
            // Arrange
            GameStateSerializer serializer = new GameStateSerializer();
            GameStateDto dto = new GameStateDto()
            {
                GameState = GameStateEnum.PLAYING,
            };
            // Act
            byte[] dtoBuf = serializer.Serialize(dto);
            GameStateDto deserDto = GameStateSerializer.Deserialize(dtoBuf);
            // Assert
            Assert.AreEqual(dto.GameState, deserDto.GameState);
        }
    }
}
