namespace SharedModels.Core
{
    public enum PacketType : byte   // set the size of enum to byte
    {
        Error,
        Player,
        PlayerAction,
        StateUpdate,
        GameUpdate,
        CardDealt,
        HandDealt,
        JoinRequest,
        EndGame,
        Disconnect,
    }
}
