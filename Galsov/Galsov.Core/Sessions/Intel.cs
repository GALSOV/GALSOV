// File: Galsov.Core/Sessions/Intel.cs
using System.Collections.Generic;

namespace Galsov.Core.Sessions
{
    /// <summary>
    /// High-level knowledge state for a star system from the
    /// perspective of a single empire.
    /// </summary>
    public enum KnowledgeState
    {
        Unknown = 0,
        Detected = 1,
        Surveyed = 2
    }

    /// <summary>
    /// Coarse-grained classification of how much "stuff" seems to be
    /// present in a system according to intel, with deliberate fuzziness.
    /// </summary>
    public enum PresenceBand
    {
        None = 0,
        Trace = 1,
        Minor = 2,
        Moderate = 3,
        Major = 4,
        Overwhelming = 5
    }

    /// <summary>
    /// Snapshot of what an empire believes about a given star system.
    /// </summary>
    public sealed class SystemKnowledge
    {
        public int StarSystemId { get; set; }

        public KnowledgeState KnowledgeState { get; set; } = KnowledgeState.Unknown;

        /// <summary>
        /// Fuzzy banded estimate of activity in the system.
        /// Null means "no meaningful reading yet".
        /// </summary>
        public PresenceBand? PresenceBand { get; set; }

        /// <summary>
        /// Integer detection level (0–4) for more granular logic later.
        /// </summary>
        public int DetectionLevel { get; set; }

        /// <summary>
        /// Turn number on which this knowledge snapshot was last updated.
        /// </summary>
        public int LastUpdatedTurn { get; set; }
    }

    /// <summary>
    /// Per-empire map of system knowledge.
    /// </summary>
    public sealed class EmpireKnowledgeMap
    {
        private readonly Dictionary<int, SystemKnowledge> _systems = new();

        public IReadOnlyDictionary<int, SystemKnowledge> Systems => _systems;

        public SystemKnowledge GetOrCreate(int starSystemId)
        {
            if (!_systems.TryGetValue(starSystemId, out var knowledge))
            {
                knowledge = new SystemKnowledge
                {
                    StarSystemId = starSystemId,
                    KnowledgeState = KnowledgeState.Unknown,
                    DetectionLevel = 0,
                    LastUpdatedTurn = 0
                };

                _systems[starSystemId] = knowledge;
            }

            return knowledge;
        }
    }
}
