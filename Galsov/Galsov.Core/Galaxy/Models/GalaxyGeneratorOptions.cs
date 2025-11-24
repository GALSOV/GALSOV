namespace Galsov.Core.Galaxy.Models
{
    public class GalaxyGeneratorOptions
    {
        public ulong Seed { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int StarSystemCount { get; set; }

        /// <summary>
        /// Controls the high-level spatial pattern used when placing star systems.
        /// Defaults to Uniform, which matches the existing behaviour.
        /// </summary>
        public GalaxyDistributionPattern DistributionPattern { get; set; } =
            GalaxyDistributionPattern.Uniform;

        /// <summary>
        /// Optional margin (in tiles) to keep star systems away from the outer map borders.
        /// 0 means no margin and allows systems to appear anywhere within the bounds.
        /// </summary>
        public int EdgeMargin { get; set; } = 0;

        /// <summary>
        /// Minimum allowed distance between any two star systems, in tiles.
        /// 0 means "no additional spacing rule" and allows systems to be adjacent.
        ///
        /// Distance is interpreted as Euclidean distance between tile centres.
        /// For example, a value of 2 ensures that no two systems occupy adjacent
        /// tiles, even diagonally.
        /// </summary>
        public int MinSystemSpacing { get; set; } = 0;

        /// <summary>
        /// Options for generating planets within each star system.
        /// </summary>
        public PlanetGenerationOptions PlanetGeneration { get; init; } = new();
    }

    public sealed class PlanetGenerationOptions
    {
        /// <summary>
        /// Minimum number of planets allowed in a system.
        /// </summary>
        public int MinPlanetsPerSystem { get; init; } = 0;

        /// <summary>
        /// Maximum number of planets allowed in a system.
        /// </summary>
        public int MaxPlanetsPerSystem { get; init; } = 8;

        /// <summary>
        /// Probability that a system has no planets at all (0.0–1.0).
        /// </summary>
        public double EmptySystemProbability { get; init; } = 0.2;

        /// <summary>
        /// Probability weight for gas giants: higher = more giants
        /// (relative to other types).
        /// </summary>
        public double GasGiantWeight { get; init; } = 1.5;

        /// <summary>
        /// Probability weight for habitable-type worlds (Earth-like, ocean, etc).
        /// </summary>
        public double HabitableWorldWeight { get; init; } = 0.5;
    }
}