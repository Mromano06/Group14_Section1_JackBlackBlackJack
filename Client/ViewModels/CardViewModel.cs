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

                if (_cardCode == "BACK")
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackImagePath));

                OnPropertyChanged();
                OnPropertyChanged(nameof(CardImagePath));
            }
        }

        public string BackImagePath => $"/Assets/BackOfCard.png";
        public string CardImagePath => $"/Assets/Cards/{CardCode}";

        public CardViewModel(string cardCode)
        {
            _cardCode = cardCode;
        }

    }
}
