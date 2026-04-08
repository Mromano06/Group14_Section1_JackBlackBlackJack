using Jables_Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

// Matthew Romano - March 12th, 2026 - BetPlacingViewModel implementation
// This creates the card to display based on the code

namespace Client.ViewModels
{
    /// <summary>
    /// ViewModel acting as a playing card for UI display.
    /// </summary>
    /// <remarks>
    /// Converts the data inside a <see cref="CardDto"/> into UI-friendly properties
    /// such as image paths for both the front and back of a card.
    /// 
    /// Designed for data binding in the UI to dynamically display card images.
    /// </remarks>
    public class CardViewModel : BaseModel
    {
        /// <summary>
        /// Internal representation of the card code (e.g., "AS", "10H").
        /// </summary>
        private string _cardCode;

        /// <summary>
        /// Gets or sets the card code used to determine the card image.
        /// </summary>
        /// <remarks>
        /// Updating this property triggers UI updates for:
        /// <list type="bullet">
        /// <item><description><see cref="CardCode"/></description></item>
        /// <item><description><see cref="CardImagePath"/></description></item>
        /// <item><description><see cref="BackImagePath"/></description></item>
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Gets the image path for the back of the card.
        /// </summary>
        /// <remarks>
        /// Used when the card should be hidden (e.g., dealer's face-down card).
        /// </remarks>
        public string BackImagePath => $"/Assets/BackOfCard.png";

        /// <summary>
        /// Gets the image path for a specific card.
        /// </summary>
        /// <remarks>
        /// Used when the card should be displayed face-up.
        /// </remarks>
        public string CardImagePath => $"/Assets/Cards/{CardCode}.png";


        /// <summary>
        /// Initializes a new instance of the <see cref="CardViewModel"/> class.
        /// </summary>
        /// <param name="cardDto">The data transfer object holding the card.</param>
        /// <remarks>
        /// Converts the card's rank and suit into a string code used for image lookup.
        /// </remarks>
        public CardViewModel(CardDto cardDto)
        {
            CardCode = cardDto?.Rank.ToString() + cardDto?.Suit.ToString();
        }

    }
}
