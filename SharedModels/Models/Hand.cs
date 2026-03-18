using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{

    public class Hand 
    {
        public List<Card> Cards { get; set; }

        public Hand()
        {
            Cards = new List<Card>();
        }

        public override string ToString()
        {
            string result = "";

            foreach (Card card in Cards)
            {
                result += card.ToString();
                
                if (Cards.IndexOf(card) != (Cards.Count() - 1)) {
                    result += ", ";
                }
            }

            return result;
        }
    }
}
