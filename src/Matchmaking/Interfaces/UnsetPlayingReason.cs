namespace SS.Matchmaking.Interfaces
{
    /// <summary>
    /// Reason a player is being removed from the 'Playing' state.
    /// </summary>
    /// <remarks>
    /// Only <see cref="MatchCancelled"/> changes implementation behavior — it triggers restoration of prior queue position(s)
    /// using the original timestamps. Other values are informational and may be surfaced in callbacks/logging.
    /// </remarks>
    public enum UnsetPlayingReason
    {
        /// <summary>Normal end-of-match cleanup.</summary>
        MatchFinished,

        /// <summary>Match was cancelled before it could start; restores prior queue position(s) when requeuing.</summary>
        MatchCancelled,

        /// <summary>Player voluntarily quit or forfeited mid-match.</summary>
        PlayerLeftMatch,

        /// <summary>KO'd player was freed early so they can find a new match.</summary>
        EarlyRequeue,

        /// <summary>A substitute took the player's slot.</summary>
        ReplacedBySub,

        /// <summary>Player failed to ready up during match start.</summary>
        NotReadiedUp,

        /// <summary>Player disconnected or abandoned during match start.</summary>
        Disconnected,
    }
}
