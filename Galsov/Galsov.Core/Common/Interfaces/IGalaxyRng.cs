// File: Galsov.Core/Common/Interfaces/IGalaxyRng.cs
namespace Galsov.Core.Common.Interfaces
{
    /// <summary>
    /// Abstraction over a deterministic random number generator
    /// used for procedural galaxy generation.
    /// </summary>
    public interface IGalaxyRng
    {
        /// <summary>
        /// Returns the next 64-bit unsigned integer in the RNG sequence.
        /// </summary>
        ulong NextUInt64();

        /// <summary>
        /// Returns an integer in the range [minInclusive, maxExclusive).
        /// </summary>
        int NextInt(int minInclusive, int maxExclusive);

        /// <summary>
        /// Returns a double in the range [0, 1).
        /// </summary>
        double NextDouble();
    }
}