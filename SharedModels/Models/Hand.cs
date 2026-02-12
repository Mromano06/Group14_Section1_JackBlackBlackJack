using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels.Models 
{

    [Serializable]
    public class Hand 
    {
        public List<Card> Cards { get; set; }

        public Hand()
        {
            Cards = new List<Card>();
        }

        public override string ToString()
        {
            return string.Join(", ", Cards);
        }
    }
}
