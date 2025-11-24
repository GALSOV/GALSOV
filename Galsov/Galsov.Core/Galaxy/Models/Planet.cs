// File: Galsov.Core/Galaxy/Models/Planet.cs
using Galsov.Core.Galaxy.Models;

namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Represents a planet within a star system.
    /// </summary>
    public class Planet
    {
        /// <summary>
        /// Identifier of the planet within its system.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Optional display name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Classification of the planet (e.g. Terran, GasGiant).
        /// </summary>
        public PlanetType Type { get; set; } = PlanetType.Unknown;

        /// <summary>
        /// Orbital index or order from the star (0 = closest).
        /// Design choice: we will switch this to 1-based *later*
        /// during Step 5C (game layer), but preserve your current 0-based
        /// for now so we don't break your renderer.
        /// </summary>
        public int OrbitIndex { get; set; }

        /// <summary>
        /// Rough size in Earth radii (0.5–13 depending on planet type).
        /// </summary>
        public double SizeEarthRadii { get; set; }

        /// <summary>
        /// Basic habitability flag—game rule placeholder.
        /// </summary>
        public bool IsHabitable { get; set; }
    }
}