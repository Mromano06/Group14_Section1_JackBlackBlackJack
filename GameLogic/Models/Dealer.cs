using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace GameLogic.Models 
{

    [Serializable]
    public class Dealer 
    {
        private string _name;
        private Hand _hand;

        public Dealer()
        {
            _name = "Dealer";
            _hand = new Hand();
        }
        public Dealer(string name)
        {
            Name = name;
            _hand = new Hand();
        }
        public string Name
        {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Dealer name cannot be empty");
                _name = value;
            }
        }

        public Hand Hand
        {
            get => _hand;
            set {
                if (value == null)
                    throw new ArgumentNullException("Hand cannot be null");
                _hand = value;
            }
        }
    }
}
