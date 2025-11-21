namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Defines the high-level spatial pattern used when placing star systems
    /// within a generated galaxy.
    /// </summary>
    public enum GalaxyDistributionPattern
    {
        /// <summary>
        /// Systems are scattered approximately uniformly across the map bounds.
        /// This corresponds to the current "simple random" behaviour.
        /// </summary>
        Uniform = 0,

        /// <summary>
        /// Systems are concentrated into a roughly circular or elliptical disc
        /// centred within the map.
        /// </summary>
        Disc = 1,

        /// <summary>
        /// Systems are grouped into several localised clusters with gaps between them.
        /// </summary>
        Clustered = 2,

        /// <summary>
        /// Systems are placed around a single ring-like structure.
        /// </summary>
        Ring = 3,

        /// <summary>
        /// Systems are placed into multiple ring-like structures (for example,
        /// an inner and an outer ring, optionally with a central concentration).
        /// </summary>
        Rings = 4
    }
}