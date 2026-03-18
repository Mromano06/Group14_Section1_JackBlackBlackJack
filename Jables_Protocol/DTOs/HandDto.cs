using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    public class HandDto
    {
        public int Count { get; set; }
        public List<CardDto>? Cards { get; set; }   // the ? declares this as nullable
    }
}
