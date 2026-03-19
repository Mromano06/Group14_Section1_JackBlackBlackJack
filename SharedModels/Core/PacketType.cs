namespace SharedModels.Core
{
    public enum PacketType : byte   // set the size of enum to byte
    {
        Error,
        PlayerAction,
        StateUpdate,
        GameUpdate,
        CardDealt,
        HandDealt,
        JoinRequest
    }
}
