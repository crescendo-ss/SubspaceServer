using SS.Core;

namespace SS.Matchmaking.Interfaces
{
    /// <summary>
    /// Interface for a service that keeps track of whether players are 'Playing' in a match 
    /// and whether players have 'Play Hold' penalties that prevent them from joining a match.
    /// </summary>
    public interface IPlayManager : IComponentInterface
    {
        /// <summary>
        /// Gets whether a <paramref name="player"/> is currently in the 'Playing' state (assigned to a match).
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns><see langword="true"/> if the player is playing in a match; otherwise, <see langword="false"/>.</returns>
        bool IsPlaying(Player player);

        #region SetPlaying* methods

        /// <summary>
        /// Marks a player as 'Playing'.
        /// </summary>
        /// <param name="player">The player to mark as 'Playing'.</param>
        void SetPlaying(Player player);

        /// <summary>
        /// Marks a set of players as 'Playing'.
        /// </summary>
        /// <param name="players">The players to mark as 'Playing'.</param>
        void SetPlaying<T>(T players) where T : IReadOnlyCollection<Player>;

        /// <summary>
        /// Marks a player as 'Playing' as a substitute player in an ongoing match.
        /// </summary>
        /// <remarks>
        /// Players that sub into existing matches do not lose their position in queues that they were searching on prior to subbing in.
        /// This method tells the service to remember that the player is playing as a substitute, so that when unset from playing, it will be able
        /// to restore the player's previous position(s) in those queue(s).
        /// </remarks>
        /// <param name="player">The player to mark as 'Playing'.</param>
        void SetPlayingAsSub(Player player);

        #endregion

        #region UnsetPlaying methods

        /// <summary>
        /// Removes the 'Playing' state of a player.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <paramref name="reason"/> is <see cref="UnsetPlayingReason.MatchCancelled"/>, the player is requeued into any queue(s)
        /// they were searching on prior to being set to 'Playing', preserving their original position (timestamp) in each queue.
        /// For all other reasons, automatic requeuing (when <paramref name="allowRequeuing"/> is <see langword="true"/>) honors the player's
        /// auto-requeue preference and uses the current time, so the player goes to the back of each queue.
        /// </para>
        /// <para>
        /// When <paramref name="holdDuration"/> is greater than <see cref="TimeSpan.Zero"/>, a play hold is applied (see
        /// <see cref="AddPlayHold(string, TimeSpan)"/>). The hold persists across disconnects: an offline player's hold is recorded and
        /// restored when they reconnect. While a hold is in effect, automatic requeuing is suppressed even if <paramref name="allowRequeuing"/> is <see langword="true"/>.
        /// </para>
        /// </remarks>
        /// <param name="playerName">The name of the player to unset from the 'Playing' state.</param>
        /// <param name="allowRequeuing">Whether to allow automatic re-queuing (search for another match).</param>
        /// <param name="reason">Why the player is being unset. Affects whether prior queue position is restored.</param>
        /// <param name="holdDuration">If greater than zero, applies a play hold for this duration. Default (<see cref="TimeSpan.Zero"/>) means no hold.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="holdDuration"/> cannot be negative.</exception>
        void UnsetPlaying(string playerName, bool allowRequeuing, UnsetPlayingReason reason = UnsetPlayingReason.MatchFinished, TimeSpan holdDuration = default);

        /// <summary>
        /// Removes the 'Playing' state of a set of players, by player name.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="UnsetPlaying(string, bool, UnsetPlayingReason, TimeSpan)" path="/remarks"/>
        /// <para>
        /// Players are processed in the order provided. Callers should preserve original ordering so that players who queued earlier stay in front.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">A collection of player names.</typeparam>
        /// <param name="playerNames">The names of players to unset from the 'Playing' state. Processed in order.</param>
        /// <param name="allowRequeuing">Whether to allow automatic re-queuing (search for another match).</param>
        /// <param name="reason">Why the players are being unset. Affects whether prior queue position is restored.</param>
        /// <param name="holdDuration">If greater than zero, applies a play hold of this duration to each player. Default (<see cref="TimeSpan.Zero"/>) means no hold.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="holdDuration"/> cannot be negative.</exception>
        void UnsetPlaying<T>(T playerNames, bool allowRequeuing, UnsetPlayingReason reason = UnsetPlayingReason.MatchFinished, TimeSpan holdDuration = default) where T : IReadOnlyCollection<string>;

        #endregion

        #region Play Hold methods

        /// <summary>
        /// Penalizes a player with a 'Play Hold' for a <paramref name="duration"/>.
        /// If there is an existing hold, the <paramref name="duration"/> is added to it.
        /// </summary>
        /// <param name="player">The player to penalize.</param>
        /// <param name="duration">The amount of time to add as a penalty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="player"/> cannot be <see langword="null">.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> must be greater than 0.</exception>
        void AddPlayHold(Player player, TimeSpan duration);

        /// <summary>
        /// Penalizes a player with a 'Play Hold' for a <paramref name="duration"/>.
        /// If there is an existing hold, the <paramref name="duration"/> is added to it.
        /// </summary>
        /// <param name="playerName">The name of the player to penalize.</param>
        /// <param name="duration">The amount of time to add as a penalty.</param>
        /// <exception cref="ArgumentException"><paramref name="playerName"/> cannot be <see langword="null"> or white-space.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> must be greater than 0.</exception>
        void AddPlayHold(string playerName, TimeSpan duration);

        /// <summary>
        /// Penalizes a player with a 'Play Hold' for a <paramref name="duration"/>.
        /// If there is an existing hold, this replaces it.
        /// Use <see cref="AddPlayHold(string, TimeSpan)"/> to add to an existing hold rather than replace it.
        /// </summary>
        /// <param name="player">The player to penalize.</param>
        /// <param name="duration">The amount of time to set as a penalty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="player"/> cannot be <see langword="null">.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> must be greater than 0.</exception>
        void SetPlayHold(Player player, TimeSpan duration);

        /// <summary>
        /// Penalizes a player with a 'Play Hold' for a <paramref name="duration"/>.
        /// If there is an existing hold, this replaces it.
        /// Use <see cref="AddPlayHold(string, TimeSpan)"/> to add to an existing hold rather than replace it.
        /// </summary>
        /// <param name="playerName">The name of the player to penalize.</param>
        /// <param name="duration">The amount of time to set as a penalty.</param>
        /// <exception cref="ArgumentException"><paramref name="playerName"/> cannot be <see langword="null"> or white-space.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> must be greater than 0.</exception>
        void SetPlayHold(string playerName, TimeSpan duration);

        /// <summary>
        /// Gets the timestamp a <paramref name="player"/>'s 'Play Hold' expires.
        /// </summary>
        /// <param name="player">The player to get play hold info for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="player"/> cannot be <see langword="null">.</exception>
        /// <returns><see langword="null"/> if there no 'Play Hold'. Otherwise, the expiration timestamp (UTC).</returns>
        DateTime? GetPlayHoldExpiration(Player player);

        #endregion
    }
}
