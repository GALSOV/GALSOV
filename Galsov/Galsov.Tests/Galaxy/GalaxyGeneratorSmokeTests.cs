// File: Galsov.Tests/Galaxy/GalaxyGeneratorSmokeTests.cs
using System;
using System.Linq;
using Galsov.Core.Galaxy.Generation;
using Galsov.Core.Galaxy.Models;
using Xunit;

namespace Galsov.Tests.Galaxy
{
    public class GalaxyGeneratorSmokeTests
    {
        [Fact]
        public void Generate_Returns_Empty_Galaxy_With_Seed_When_Count_Is_Zero()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 42UL,
                Width = 100,
                Height = 80,
                StarSystemCount = 0
            };

            var galaxy = generator.Generate(options);

            Assert.NotNull(galaxy);
            Assert.Equal(options.Seed, galaxy.Seed);
            Assert.Equal(options.Width, galaxy.Width);
            Assert.Equal(options.Height, galaxy.Height);
            Assert.Empty(galaxy.StarSystems);
        }

        [Fact]
        public void Generate_Creates_Requested_Number_Of_Systems_Within_Bounds_And_Unique()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 1234UL,
                Width = 120,
                Height = 80,
                StarSystemCount = 50,
                DistributionPattern = GalaxyDistributionPattern.Uniform
            };

            var galaxy = generator.Generate(options);

            Assert.Equal(options.StarSystemCount, galaxy.StarSystems.Count);

            // All systems are within bounds.
            Assert.All(galaxy.StarSystems, s =>
            {
                Assert.InRange(s.X, 0, options.Width - 1);
                Assert.InRange(s.Y, 0, options.Height - 1);
            });

            // IDs are unique and sequential starting at 1.
            var ids = galaxy.StarSystems.Select(s => s.Id).ToArray();
            Assert.Equal(ids.Length, ids.Distinct().Count());
            Assert.Equal(Enumerable.Range(1, options.StarSystemCount), ids.OrderBy(x => x));

            // Positions are unique.
            var coords = galaxy.StarSystems.Select(s => (s.X, s.Y)).ToArray();
            Assert.Equal(coords.Length, coords.Distinct().Count());
        }

        [Fact]
        public void Same_Seed_Produces_Identical_Layouts()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 42UL,
                Width = 120,
                Height = 80,
                StarSystemCount = 30,
                DistributionPattern = GalaxyDistributionPattern.Uniform
            };

            var galaxy1 = generator.Generate(options);
            var galaxy2 = generator.Generate(options);

            Assert.Equal(galaxy1.StarSystems.Count, galaxy2.StarSystems.Count);

            for (int i = 0; i < galaxy1.StarSystems.Count; i++)
            {
                var s1 = galaxy1.StarSystems[i];
                var s2 = galaxy2.StarSystems[i];

                Assert.Equal(s1.X, s2.X);
                Assert.Equal(s1.Y, s2.Y);
            }
        }

        [Fact]
        public void Generate_Throws_When_StarSystemCount_Exceeds_Capacity()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 1UL,
                Width = 5,
                Height = 5,
                StarSystemCount = 26 // 5×5 = 25, so this should fail.
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(options));
        }

        [Fact]
        public void Uniform_Distribution_Respects_EdgeMargin()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 999UL,
                Width = 40,
                Height = 20,
                StarSystemCount = 60,
                DistributionPattern = GalaxyDistributionPattern.Uniform,
                EdgeMargin = 2
            };

            var galaxy = generator.Generate(options);

            Assert.All(galaxy.StarSystems, s =>
            {
                Assert.InRange(s.X, options.EdgeMargin, options.Width - options.EdgeMargin - 1);
                Assert.InRange(s.Y, options.EdgeMargin, options.Height - options.EdgeMargin - 1);
            });
        }

        [Fact]
        public void MinSystemSpacing_Of_Two_Prevents_Adjacent_Systems()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 2024UL,
                Width = 40,
                Height = 40,
                StarSystemCount = 50,
                DistributionPattern = GalaxyDistributionPattern.Uniform,
                MinSystemSpacing = 2
            };

            var galaxy = generator.Generate(options);

            var systems = galaxy.StarSystems;

            // For MinSystemSpacing = 2, we expect Euclidean distance^2 >= 4 between any two systems.
            for (int i = 0; i < systems.Count; i++)
            {
                for (int j = i + 1; j < systems.Count; j++)
                {
                    int dx = systems[i].X - systems[j].X;
                    int dy = systems[i].Y - systems[j].Y;
                    int distanceSquared = dx * dx + dy * dy;

                    Assert.True(distanceSquared >= 4,
                        $"Systems at ({systems[i].X},{systems[i].Y}) and ({systems[j].X},{systems[j].Y}) are too close.");
                }
            }
        }

        [Fact]
        public void Disc_Distribution_Generates_Systems_Within_Bounds()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 7UL,
                Width = 120,
                Height = 80,
                StarSystemCount = 100,
                DistributionPattern = GalaxyDistributionPattern.Disc
            };

            var galaxy = generator.Generate(options);

            Assert.Equal(options.StarSystemCount, galaxy.StarSystems.Count);

            Assert.All(galaxy.StarSystems, s =>
            {
                Assert.InRange(s.X, 0, options.Width - 1);
                Assert.InRange(s.Y, 0, options.Height - 1);
            });
        }

        [Fact]
        public void Clustered_Ring_And_Rings_Patterns_Produce_Correct_Count_Within_Bounds()
        {
            var generator = new GalaxyGenerator();

            var patterns = new[]
            {
                GalaxyDistributionPattern.Clustered,
                GalaxyDistributionPattern.Ring,
                GalaxyDistributionPattern.Rings
            };

            foreach (var pattern in patterns)
            {
                var options = new GalaxyGeneratorOptions
                {
                    Seed = 123UL,
                    Width = 120,
                    Height = 80,
                    StarSystemCount = 80,
                    DistributionPattern = pattern
                };

                var galaxy = generator.Generate(options);

                Assert.Equal(options.StarSystemCount, galaxy.StarSystems.Count);

                Assert.All(galaxy.StarSystems, s =>
                {
                    Assert.InRange(s.X, 0, options.Width - 1);
                    Assert.InRange(s.Y, 0, options.Height - 1);
                });
            }
        }
    }
}