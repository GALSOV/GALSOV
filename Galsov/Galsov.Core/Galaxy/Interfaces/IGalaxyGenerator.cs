// File: Galsov.Core/Galaxy/Interfaces/IGalaxyGenerator.cs
using GalaxyModel = Galsov.Core.Galaxy.Models.Galaxy;
using GalaxyGeneratorOptions = Galsov.Core.Galaxy.Models.GalaxyGeneratorOptions;

namespace Galsov.Core.Galaxy.Interfaces
{
    /// <summary>
    /// Defines the contract for generating a galaxy from options.
    /// </summary>
    public interface IGalaxyGenerator
    {
        /// <summary>
        /// Generates a galaxy using the specified options.
        /// Implementations should be deterministic for a given seed.
        /// </summary>
        GalaxyModel Generate(GalaxyGeneratorOptions options);
    }
}