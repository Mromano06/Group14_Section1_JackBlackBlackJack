using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Represents a player's hand in the game, including the count of cards and the list of card details.
    /// </summary>
    public class HandDto
    {
        public int Count { get; set; }
        public List<CardDto>? Cards { get; set; }   // the ? declares this as nullable
    }
}
