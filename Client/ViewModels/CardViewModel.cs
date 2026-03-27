using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.ViewModels
{
    public class CardViewModel : BaseModel
    {
        private string _cardCode;

        public string CardCode 
        {
            get => _cardCode;
            set
            {
                _cardCode = value;

                OnPropertyChanged(nameof(CardCode));
                OnPropertyChanged(nameof(CardImagePath));
                OnPropertyChanged(nameof(BackImagePath));
            }
        }

        public string BackImagePath => $"/Assets/BackOfCard.png";
        public string CardImagePath => $"/Assets/Cards/{CardCode}.png";

        public CardViewModel(CardDto cardDto)
        {
            CardCode = cardDto?.Rank.ToString() + cardDto?.Suit.ToString();
        }

    }
}
