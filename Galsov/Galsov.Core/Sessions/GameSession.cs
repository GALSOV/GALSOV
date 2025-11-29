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

            // EXPLORATION & INTEL (v0.1: simple spreading)
            ApplyExplorationAndIntel(PlayerEmpire);

            // TODO: ECONOMY
            // - Tick colony outputs to generate Credits/Production/Science/Supplies.

            // TODO: RESEARCH
            // - Advance active research projects, check for completions.

            // TODO: CHARACTERS
            // - Update Admiral XP, traits, and statuses.
        }

        private void ApplyExplorationAndIntel(Empire empire)
        {
            var galaxy = Galaxy;
            if (galaxy.StarSystems.Count == 0)
            {
                return;
            }

            // We start "spreading" from turn 2 onwards.
            var index = TurnNumber - 2;
            if (index < 0)
            {
                return;
            }

            index = index % galaxy.StarSystems.Count;

            var targetSystem = galaxy.StarSystems[index];

            // Guarantee a knowledge entry exists for this system.
            var knowledge = empire.Knowledge.GetOrCreate(targetSystem.Id);

            var oldState = knowledge.KnowledgeState;

            switch (knowledge.KnowledgeState)
            {
                case KnowledgeState.Unknown:
                    knowledge.KnowledgeState = KnowledgeState.Detected;
                    knowledge.PresenceBand = PresenceBand.Trace;
                    knowledge.DetectionLevel = Math.Max(knowledge.DetectionLevel, 1);
                    break;

                case KnowledgeState.Detected:
                    knowledge.KnowledgeState = KnowledgeState.Surveyed;
                    knowledge.PresenceBand = PresenceBand.Minor;
                    knowledge.DetectionLevel = Math.Max(knowledge.DetectionLevel, 3);
                    break;

                case KnowledgeState.Surveyed:
                    // Already fully explored – no change for now.
                    break;
            }

            knowledge.LastUpdatedTurn = TurnNumber;

            if (knowledge.KnowledgeState != oldState)
            {
                _turnLog.Add(TurnLogEntry.Create(
                    TurnNumber,
                    "INTEL",
                    $"Exploration updated system {targetSystem.Id} from {oldState} to {knowledge.KnowledgeState}."));
            }
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
