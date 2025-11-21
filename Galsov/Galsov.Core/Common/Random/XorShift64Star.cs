// File: Galsov.Core/Common/Random/XorShift64Star.cs
using Galsov.Core.Common.Interfaces;

namespace Galsov.Core.Common.Random
{
    /// <summary>
    /// Deterministic RNG based on the XorShift64* algorithm.
    /// Fast, seedable and suitable for procedural galaxy generation.
    /// </summary>
    public sealed class XorShift64Star : IGalaxyRng
    {
        private ulong _state;

        public XorShift64Star(ulong seed)
        {
            // Avoid a zero state (degenerate for xorshift).
            _state = seed == 0 ? 0xBAD0_BEEF_F00D_1234UL : seed;
        }

        /// <summary>
        /// Returns the next 64-bit unsigned integer in the sequence.
        /// </summary>
        public ulong NextUInt64()
        {
            var x = _state;
            x ^= x >> 12;
            x ^= x << 25;
            x ^= x >> 27;
            _state = x;
            return x * 2685821657736338717UL;
        }

        /// <summary>
        /// Returns an integer in the range [minInclusive, maxExclusive).
        /// </summary>
        public int NextInt(int minInclusive, int maxExclusive)
        {
            var range = (ulong)(maxExclusive - minInclusive);
            var value = (int)(NextUInt64() % range);
            return minInclusive + value;
        }

        /// <summary>
        /// Returns a double in the range [0, 1).
        /// </summary>
        public double NextDouble()
        {
            // Standard trick: use the top 53 bits to create a double in [0,1).
            return (NextUInt64() >> 11) * (1.0 / (1UL << 53));
        }
    }
}