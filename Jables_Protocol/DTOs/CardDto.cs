using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a playing card, including its rank and suit as single characters.
    /// Used for transmitting card data between client and server.
    /// </summary>
    public class CardDto
    {
        public char Rank { get; set; }
        public char Suit { get; set; }
    }
}
