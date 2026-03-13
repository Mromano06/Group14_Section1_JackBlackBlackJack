using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Models;

namespace Jables_Protocol.DTOs
{
    internal class HandDto
    {
        public List<CardDto>? Cards { get; set; }   // the ? declares this as nullable
    }
}
