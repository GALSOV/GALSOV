// File: Galsov.Core/Sessions/EmpireDomain.cs
using System.Collections.Generic;

namespace Galsov.Core.Sessions
{
    /// <summary>
    /// Top-level state for a single empire in a GameSession.
    /// </summary>
    public sealed class Empire
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Colonies controlled by this empire.
        /// </summary>
        public List<Colony> Colonies { get; } = new();

        /// <summary>
        /// Mobile fleets controlled by this empire.
        /// </summary>
        public List<Fleet> Fleets { get; } = new();

        // --- Economy snapshot (v0.1 minimal set) ---

        public int Credits { get; set; }
        public int Production { get; set; }
        public int Science { get; set; }

        // --- Research state (string IDs, to be wired to data later) ---

        public List<string> KnownTechIds { get; } = new();
        public List<string> ActiveTechIds { get; } = new();

        // --- Per-empire knowledge / fog-of-war ---

        public EmpireKnowledgeMap Knowledge { get; } = new();
    }

    /// <summary>
    /// A colony on a specific planet (or moon) in a star system.
    /// </summary>
    public sealed class Colony
    {
        public int StarSystemId { get; set; }
        public int PlanetId { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Abstract population value; tuned later for real numbers.
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Simple stand-in for colony output per turn.
        /// </summary>
        public int ProductionPerTurn { get; set; }
    }

    /// <summary>
    /// Fleet movement mode stub. These map onto the FTL rules in the
    /// paper design but are intentionally coarse for v0.1.
    /// </summary>
    public enum FleetMovementMode
    {
        Idle = 0,
        Jump = 1,
        DeepSpace = 2,
        WormholeTransit = 3
    }

    /// <summary>
    /// A group of ships moving together and sharing supplies.
    /// </summary>
    public sealed class Fleet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// If set, the fleet is located in a star system.
        /// If null, the fleet is assumed to be in deep space.
        /// </summary>
        public int? CurrentStarSystemId { get; set; }

        /// <summary>
        /// Optional deep-space coordinates (grid aligned with the galaxy).
        /// These are placeholders; real pathing comes later.
        /// </summary>
        public double? X { get; set; }
        public double? Y { get; set; }

        public FleetMovementMode MovementMode { get; set; } = FleetMovementMode.Idle;

        public List<Ship> Ships { get; } = new();

        // --- Supply & logistics (very rough stub) ---

        public int SupplyCapacity { get; set; }
        public int SuppliesCurrent { get; set; }
    }

    /// <summary>
    /// Individual ship instance inside a fleet.
    /// </summary>
    public sealed class Ship
    {
        public int Id { get; set; }

        /// <summary>
        /// Identifier for the ship class / design. The actual stats will
        /// be defined in data tables later.
        /// </summary>
        public string ShipClassId { get; set; } = string.Empty;

        public int Hull { get; set; }
        public int Speed { get; set; }

        /// <summary>
        /// Supplies consumed per turn when operating.
        /// </summary>
        public int SupplyUsePerTurn { get; set; }
    }
}
