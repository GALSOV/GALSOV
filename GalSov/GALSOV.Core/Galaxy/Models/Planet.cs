// File: Galsov.Core/Galaxy/Models/Planet.cs
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
        /// </summary>
        public int OrbitIndex { get; set; }
    }
}