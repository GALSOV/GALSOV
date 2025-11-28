// File: Galsov.Core/Sessions/GameSession.cs
using System;
using System.Collections.Generic;
using Galsov.Core.Galaxy.Models;

namespace Galsov.Core.Sessions
{
    /// <summary>
    /// Root per-game state for a single run of GALSOV.
    /// v0.1 focuses on a single human-controlled PlayerEmpire, but
    /// the model supports multiple empires from the start.
    /// </summary>
    public sealed class GameSession
    {
        private readonly List<Empire> _empires = new();
        private readonly List<TurnLogEntry> _turnLog = new();

        public GameSession(Galsov.Core.Galaxy.Models.Galaxy galaxy, Empire playerEmpire)
        {
            Galaxy = galaxy ?? throw new ArgumentNullException(nameof(galaxy));
            PlayerEmpire = playerEmpire ?? throw new ArgumentNullException(nameof(playerEmpire));

            _empires.Add(playerEmpire);

            TurnNumber = 1;
            _turnLog.Add(TurnLogEntry.Create(TurnNumber, "TURN", "Game session created (Turn 1)."));
        }

        /// <summary>
        /// Generated galaxy map used by this session.
        /// </summary>
        public Galsov.Core.Galaxy.Models.Galaxy Galaxy { get; }

        /// <summary>
        /// All empires participating in this session.
        /// </summary>
        public IReadOnlyList<Empire> Empires => _empires;

        /// <summary>
        /// The local human player's empire for v0.1.
        /// </summary>
        public Empire PlayerEmpire { get; }

        /// <summary>
        /// Current turn number. Starts at 1 after creation.
        /// </summary>
        public int TurnNumber { get; private set; }

        /// <summary>
        /// Simple structured log of turn-level events. For now this
        /// just tracks TurnStarted entries so we can see that
        /// AdvanceTurn() is being called.
        /// </summary>
        public IReadOnlyList<TurnLogEntry> TurnLog => _turnLog;

        public void AdvanceTurn()
        {
            TurnNumber++;

            _turnLog.Add(TurnLogEntry.Create(TurnNumber, "TURN", $"Turn {TurnNumber} started."));

            // ---- TURN STRUCTURE STUBS ----

            // TODO: MOVEMENT PHASE
            // - Resolve fleet movement (jumps, deep-space, wormholes).
            // - Update fleet positions and travel progress.

            // TODO: SUPPLY & LOGISTICS
            // - Consume supplies based on fleet activity.
            // - Apply penalties for out-of-supply fleets.

            // TODO: EXPLORATION & INTEL
            // - Update EmpireKnowledgeMap based on sensors and events.
            // - Compute presence bands and detection levels.
            // - Append INTEL UPDATE entries to the log.

            // TODO: ECONOMY
            // - Tick colony outputs to generate Credits/Production/Science/Supplies.

            // TODO: RESEARCH
            // - Advance active research projects, check for completions.

            // TODO: CHARACTERS
            // - Update Admiral XP, traits, and statuses.
        }

        /// <summary>
        /// Very lightweight structured log record. This will likely
        /// evolve into a richer log system, but for AdvanceTurn()
        /// scaffolding this is enough.
        /// </summary>
        public sealed record TurnLogEntry(int TurnNumber, string Category, string Message)
        {
            public static TurnLogEntry Create(int turnNumber, string category, string message) =>
                new(turnNumber, category, message);
        }
    }
}
