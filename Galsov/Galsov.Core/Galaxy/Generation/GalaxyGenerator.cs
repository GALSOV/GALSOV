// File: Galsov.Core/Galaxy/Generation/GalaxyGenerator.cs
using Galsov.Core.Common.Random;
using Galsov.Core.Galaxy.Interfaces;
using Galsov.Core.Galaxy.Models;
using System;
using System.Collections.Generic;
using GalaxyDistributionPattern = Galsov.Core.Galaxy.Models.GalaxyDistributionPattern;
using GalaxyGeneratorOptions = Galsov.Core.Galaxy.Models.GalaxyGeneratorOptions;
using GalaxyModel = Galsov.Core.Galaxy.Models.Galaxy;
using StarSystemModel = Galsov.Core.Galaxy.Models.StarSystem;

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
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.StarSystemCount < 0)
                throw new ArgumentOutOfRangeException(nameof(options.StarSystemCount),
                    "StarSystemCount cannot be negative.");

            if (options.Width <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.Width),
                    "Width must be positive.");

            if (options.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(options.Height),
                    "Height must be positive.");

            // Capacity check: we require unique tiles per star system.
            long capacity = (long)options.Width * options.Height;
            if (options.StarSystemCount > capacity)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.StarSystemCount),
                    "StarSystemCount cannot exceed Width × Height when each system occupies a unique tile.");
            }

            if (options.EdgeMargin < 0)
                throw new ArgumentOutOfRangeException(nameof(options.EdgeMargin),
                    "EdgeMargin cannot be negative.");

            if (options.EdgeMargin * 2 >= options.Width || options.EdgeMargin * 2 >= options.Height)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.EdgeMargin),
                    "EdgeMargin is too large for the map dimensions.");
            }

            if (options.MinSystemSpacing < 0)
                throw new ArgumentOutOfRangeException(nameof(options.MinSystemSpacing),
                    "MinSystemSpacing cannot be negative.");

            const int MaxAttemptsPerSystem = 10_000;

            int minSpacing = options.MinSystemSpacing;
            int minDistanceSquared = minSpacing > 0 ? minSpacing * minSpacing : 0;

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

            // Precompute some geometric helpers for the patterns.
            int mapWidth = options.Width;
            int mapHeight = options.Height;

            double centerX = (mapWidth - 1) / 2.0;
            double centerY = (mapHeight - 1) / 2.0;

            double maxDiscRadiusX = (mapWidth - 1) / 2.0;
            double maxDiscRadiusY = (mapHeight - 1) / 2.0;
            double maxRingRadius = Math.Min(maxDiscRadiusX, maxDiscRadiusY);

            // Cluster centres for the Clustered pattern.
            var clusterCenters = new List<(double X, double Y)>();
            if (options.DistributionPattern == GalaxyDistributionPattern.Clustered &&
                options.StarSystemCount > 0)
            {
                int estimatedClusterCount = options.StarSystemCount / 8; // ~8–10 per cluster
                if (estimatedClusterCount < 1)
                    estimatedClusterCount = 1;
                if (estimatedClusterCount > 8)
                    estimatedClusterCount = 8;

                for (int i = 0; i < estimatedClusterCount; i++)
                {
                    int cx = rng.NextInt(0, mapWidth);
                    int cy = rng.NextInt(0, mapHeight);
                    clusterCenters.Add((cx, cy));
                }
            }

            for (int i = 0; i < options.StarSystemCount; i++)
            {
                bool placed = false;
                int attempts = 0;
                int x = 0;
                int y = 0;

                while (!placed && attempts < MaxAttemptsPerSystem)
                {
                    attempts++;

                    (x, y) = SamplePosition();

                    // Enforce edge margin for all patterns.
                    if (options.EdgeMargin > 0)
                    {
                        int margin = options.EdgeMargin;
                        if (x < margin || x >= mapWidth - margin ||
                            y < margin || y >= mapHeight - margin)
                        {
                            continue;
                        }
                    }

                    // Enforce uniqueness.
                    if (!usedPositions.Add((x, y)))
                    {
                        continue;
                    }

                    // Enforce minimum spacing between systems, if requested.
                    if (minSpacing > 0 &&
                        !IsFarEnoughFromExisting(x, y, galaxy.StarSystems, minDistanceSquared))
                    {
                        // Release the coordinate and try again.
                        usedPositions.Remove((x, y));
                        continue;
                    }

                    placed = true;
                }

                if (!placed)
                {
                    throw new InvalidOperationException(
                        $"Unable to place star system {i + 1} with the current galaxy generation options. " +
                        "The requested StarSystemCount, MinSystemSpacing, and pattern may be too dense for the map size.");
                }

                var starClass = SampleStarClass(rng);
                var planets = GeneratePlanetsForSystem(i + 1, rng, options.PlanetGeneration);

                var system = new StarSystemModel
                {
                    Id = i + 1,
                    Name = $"SYS-{i + 1:D4}",
                    X = x,
                    Y = y,
                    StarClass = starClass,
                    Planets = planets
                };

                galaxy.StarSystems.Add(system);

            }

            return galaxy;


            StarClass SampleStarClass(XorShift64Star rng)
            {
                var roll = rng.NextDouble();

                // Design choice: not physically accurate, tuned for gameplay variety.
                if (roll < 0.02) return StarClass.BlackHole;
                if (roll < 0.05) return StarClass.Neutron;
                if (roll < 0.15) return StarClass.BlueGiant;
                if (roll < 0.30) return StarClass.White;
                if (roll < 0.55) return StarClass.Yellow;
                if (roll < 0.75) return StarClass.Orange;
                if (roll < 0.90) return StarClass.Green;

                return StarClass.RedDwarf;
            }

            List<Planet> GeneratePlanetsForSystem(
                int systemId,
                XorShift64Star rng,
                PlanetGenerationOptions options)
            {
                // No planets possible at all.
                if (options.MaxPlanetsPerSystem <= 0)
                    return new List<Planet>();

                // Randomly decide to make this system empty.
                if (rng.NextDouble() < options.EmptySystemProbability)
                    return new List<Planet>();

                int min = Math.Max(0, options.MinPlanetsPerSystem);
                int max = Math.Max(min, options.MaxPlanetsPerSystem);

                // maxExclusive → add +1 to make it inclusive.
                int planetCount = rng.NextInt(min, max + 1);

                if (planetCount == 0)
                    return new List<Planet>();

                var planets = new List<Planet>(planetCount);

                for (int i = 0; i < planetCount; i++)
                {
                    int orbitIndex = i; // 0-based to match your current renderer.

                    var type = SamplePlanetType(rng, options);
                    double size = SamplePlanetSize(rng, type);
                    bool habitable = IsHabitable(type);

                    var planet = new Planet
                    {
                        // Design choice: planet Id is 1..N within the system.
                        Id = i + 1,
                        Type = type,
                        OrbitIndex = orbitIndex,
                        SizeEarthRadii = size,
                        IsHabitable = habitable
                    };

                    planets.Add(planet);
                }

                return planets;
            }

            PlanetType SamplePlanetType(XorShift64Star rng, PlanetGenerationOptions options)
            {
                double roll = rng.NextDouble();

                // Small chance to be an asteroid belt.
                if (roll < 0.05)
                    return PlanetType.AsteroidBelt;

                // "Habitable-like" slice: scaled by HabitableWorldWeight
                double habitableSlice = 0.10 * options.HabitableWorldWeight;
                if (roll < 0.05 + habitableSlice)
                {
                    // Split between rocky & ocean.
                    return rng.NextDouble() < 0.5 ? PlanetType.Rocky : PlanetType.Ocean;
                }

                // Gas/ice giants slice, scaled by GasGiantWeight.
                double gasSlice = 0.20 * options.GasGiantWeight;
                if (roll < 0.05 + habitableSlice + gasSlice)
                {
                    return rng.NextDouble() < 0.7 ? PlanetType.GasGiant : PlanetType.Ice;
                }

                // Tail: barren / desert / rocky.
                double tailRoll = rng.NextDouble();
                if (tailRoll < 0.4) return PlanetType.Barren;
                if (tailRoll < 0.7) return PlanetType.Desert;
                return PlanetType.Rocky;
            }

            double SamplePlanetSize(XorShift64Star rng, PlanetType type)
            {
                double roll = rng.NextDouble();

                // Very rough design choice – not realistic, just flavour.
                return type switch
                {
                    PlanetType.GasGiant => 5.0 + roll * 8.0,  // 5–13 Earth radii
                    PlanetType.Ice => 3.0 + roll * 4.0,  // 3–7
                    PlanetType.AsteroidBelt => 0.1 + roll * 0.3,  // 0.1–0.4
                    _ => 0.5 + roll * 2.0   // 0.5–2.5
                };
            }

            bool IsHabitable(PlanetType type)
            {
                // Game rule: only some classes are colonisable.
                return type == PlanetType.Rocky
                    || type == PlanetType.Ocean
                    || type == PlanetType.Desert;
            }


            // ---- Local helper functions ----

            (int X, int Y) SamplePosition()
            {
                switch (options.DistributionPattern)
                {
                    case GalaxyDistributionPattern.Disc:
                        return SampleDiscPosition();

                    case GalaxyDistributionPattern.Clustered:
                        return SampleClusteredPosition();

                    case GalaxyDistributionPattern.Ring:
                        return SampleRingPosition(inner: false);

                    case GalaxyDistributionPattern.Rings:
                        return SampleRingsPosition();

                    case GalaxyDistributionPattern.Uniform:
                    default:
                        return SampleUniformPosition();
                }
            }

            (int X, int Y) SampleUniformPosition()
            {
                int sx = rng.NextInt(0, mapWidth);
                int sy = rng.NextInt(0, mapHeight);
                return (sx, sy);
            }

            (int X, int Y) SampleDiscPosition()
            {
                double theta = rng.NextDouble() * 2.0 * Math.PI;
                double r = Math.Sqrt(rng.NextDouble());

                double rx = r * maxDiscRadiusX;
                double ry = r * maxDiscRadiusY;

                double px = centerX + rx * Math.Cos(theta);
                double py = centerY + ry * Math.Sin(theta);

                int sx = (int)Math.Round(px);
                int sy = (int)Math.Round(py);

                if (sx < 0) sx = 0;
                if (sx >= mapWidth) sx = mapWidth - 1;
                if (sy < 0) sy = 0;
                if (sy >= mapHeight) sy = mapHeight - 1;

                return (sx, sy);
            }

            (int X, int Y) SampleClusteredPosition()
            {
                if (clusterCenters.Count == 0)
                {
                    // Fallback to uniform if, for some reason, no clusters were created.
                    return SampleUniformPosition();
                }

                int clusterIndex = rng.NextInt(0, clusterCenters.Count);
                var center = clusterCenters[clusterIndex];

                double maxClusterRadius = Math.Max(1.0, Math.Min(mapWidth, mapHeight) / 10.0);

                double theta = rng.NextDouble() * 2.0 * Math.PI;
                double r = Math.Sqrt(rng.NextDouble()) * maxClusterRadius;

                double px = center.X + r * Math.Cos(theta);
                double py = center.Y + r * Math.Sin(theta);

                int sx = (int)Math.Round(px);
                int sy = (int)Math.Round(py);

                if (sx < 0) sx = 0;
                if (sx >= mapWidth) sx = mapWidth - 1;
                if (sy < 0) sy = 0;
                if (sy >= mapHeight) sy = mapHeight - 1;

                return (sx, sy);
            }

            (int X, int Y) SampleRingPosition(bool inner)
            {
                double baseRadius = inner ? maxRingRadius * 0.4 : maxRingRadius * 0.75;
                double radiusJitter = maxRingRadius * 0.05;

                double theta = rng.NextDouble() * 2.0 * Math.PI;
                double r = baseRadius + (rng.NextDouble() - 0.5) * 2.0 * radiusJitter;

                if (r < 0.0)
                    r = 0.0;

                double px = centerX + r * Math.Cos(theta);
                double py = centerY + r * Math.Sin(theta);

                int sx = (int)Math.Round(px);
                int sy = (int)Math.Round(py);

                if (sx < 0) sx = 0;
                if (sx >= mapWidth) sx = mapWidth - 1;
                if (sy < 0) sy = 0;
                if (sy >= mapHeight) sy = mapHeight - 1;

                return (sx, sy);
            }

            (int X, int Y) SampleRingsPosition()
            {
                bool useInner = rng.NextDouble() < 0.5;
                return SampleRingPosition(inner: useInner);
            }

            bool IsFarEnoughFromExisting(int candidateX, int candidateY, List<StarSystemModel> existingSystems, int requiredDistanceSquared)
            {
                if (requiredDistanceSquared <= 0)
                    return true;

                for (int i = 0; i < existingSystems.Count; i++)
                {
                    var existing = existingSystems[i];
                    int dx = candidateX - existing.X;
                    int dy = candidateY - existing.Y;
                    int distanceSquared = dx * dx + dy * dy;
                    if (distanceSquared < requiredDistanceSquared)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}