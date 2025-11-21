// File: Galsov.Core/Galaxy/Generation/GalaxyGenerator.cs
using System;
using System.Collections.Generic;
using Galsov.Core.Common.Random;
using Galsov.Core.Galaxy.Interfaces;
using StarSystemModel = Galsov.Core.Galaxy.Models.StarSystem;
using GalaxyModel = Galsov.Core.Galaxy.Models.Galaxy;
using GalaxyGeneratorOptions = Galsov.Core.Galaxy.Models.GalaxyGeneratorOptions;

namespace Galsov.Core.Galaxy.Generation
{
    /// <summary>
    /// Galaxy generator responsible for creating star systems
    /// deterministically based on a seed and options.
    /// </summary>
    public sealed class GalaxyGenerator : IGalaxyGenerator
    {
        public GalaxyModel Generate(GalaxyGeneratorOptions options)
        {
            if (options.StarSystemCount < 0)
                throw new ArgumentOutOfRangeException(nameof(options.StarSystemCount),
                    "StarSystemCount cannot be negative.");

            if (options.Width <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.Width),
                    "Width must be positive.");

            if (options.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.Height),
                    "Height must be positive.");

            var rng = new XorShift64Star(options.Seed);

            var galaxy = new GalaxyModel
            {
                Seed = options.Seed,
                Width = options.Width,
                Height = options.Height,
                StarSystems = new List<StarSystemModel>()
            };

            // Track used coordinates to avoid duplicate star positions.
            var usedPositions = new HashSet<(int X, int Y)>();

            for (int i = 0; i < options.StarSystemCount; i++)
            {
                int x;
                int y;

                // Keep sampling until we find a unique coordinate.
                // For reasonable densities this is fine; we can optimise later if needed.
                do
                {
                    x = rng.NextInt(0, options.Width);
                    y = rng.NextInt(0, options.Height);
                }
                while (!usedPositions.Add((x, y)));

                var system = new StarSystemModel
                {
                    Id = i + 1,
                    Name = $"SYS-{i + 1:D4}",
                    X = x,
                    Y = y
                    // Planets list stays empty for now; later steps will populate it.
                };

                galaxy.StarSystems.Add(system);
            }

            return galaxy;
        }
    }
}