using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    public class PlayerDto
    {
        public string? Name { get; set; }   // Made it nullable
        public int CardCount { get; set; }
        public List<CardDto>? Hand { get; set; }    // Made it nullable
        public double CurrentBet { get; set; }
        public bool HasDoubled { get; set; }
        public bool HasInsured { get; set; }
        public int ActionCount { get; set; }
        public double Balance { get; set; }

        //public PlayerDto() 
        //{
        //    Hand = new List<CardDto>();
        //}
    }
}
