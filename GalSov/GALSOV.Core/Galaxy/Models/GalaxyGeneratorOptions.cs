// File: Galsov.Core/Galaxy/Models/GalaxyGeneratorOptions.cs
namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Options for generating a galaxy.
    /// This is the main input to IGalaxyGenerator.
    /// </summary>
    public class GalaxyGeneratorOptions
    {
        /// <summary>
        /// Seed for deterministic generation.
        /// Using the same seed and options should produce the same galaxy.
        /// </summary>
        public ulong Seed { get; set; }

        /// <summary>
        /// Desired width of the galaxy map.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Desired height of the galaxy map.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Number of star systems to generate.
        /// </summary>
        public int StarSystemCount { get; set; }
    }
}