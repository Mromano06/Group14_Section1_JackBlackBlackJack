using System;
using System.Collections.Generic;
using System.Text;
using SharedModels.Core;

namespace Jables_Protocol.DTOs
{
    public class ImageDto
    {
        GameResult gameResult { get; set; }       
        byte[] ImageData { get; set; } = Array.Empty<byte>();
    }
}
