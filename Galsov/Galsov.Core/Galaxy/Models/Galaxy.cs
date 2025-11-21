// File: Galsov.Core/Galaxy/Models/Galaxy.cs
using System.Collections.Generic;

namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Root model representing the generated galaxy map.
    /// </summary>
    public class Galaxy
    {
        /// <summary>
        /// Seed used for deterministic generation.
        /// Storing this allows us to re-run or debug the same galaxy later.
        /// </summary>
        public ulong Seed { get; set; }

        /// <summary>
        /// Width of the galaxy in map units (grid or coordinate system).
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the galaxy in map units (grid or coordinate system).
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// All star systems within this galaxy.
        /// </summary>
        public List<StarSystem> StarSystems { get; set; } = new();
    }
}