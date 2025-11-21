// File: Galsov.Core/Galaxy/Models/StarSystem.cs
using System.Collections.Generic;
using System.Numerics;

namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Represents a single star system in the galaxy map.
    /// </summary>
    public class StarSystem
    {
        /// <summary>
        /// Identifier for the star system. This can be used
        /// for lookups, saves, etc. For now we use an int.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Optional display name for the system.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// X coordinate within the galaxy.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y coordinate within the galaxy.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Planets orbiting this star system.
        /// </summary>
        public List<Planet> Planets { get; set; } = new();
    }
}