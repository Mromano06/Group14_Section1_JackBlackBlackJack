using System;
using System.Diagnostics.CodeAnalysis;

namespace Jables_Protocol.DTOs
{
    /// <summary>
    /// Data transfer object sent by the server in response to a login request.
    /// Tells the client whether access was granted or denied, with a reason.
    /// </summary>
    
    [ExcludeFromCodeCoverage]
    public class LoginResponseDto
    {
        /// <summary>
        /// True if the passcode was correct and the client may proceed to the game.
        /// False if the passcode was wrong and the client should be denied.
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// A human-readable message explaining the result (e.g. "Welcome!" or "Wrong passcode.").
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}