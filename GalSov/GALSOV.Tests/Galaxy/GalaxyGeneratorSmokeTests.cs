// File: Galsov.Tests/Galaxy/GalaxyGeneratorSmokeTests.cs
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
                Seed = 12345UL,
                Width = 100,
                Height = 100,
                StarSystemCount = 0
            };

            var result = generator.Generate(options);

            Assert.NotNull(result);
            Assert.Equal(12345UL, result.Seed);
            Assert.Empty(result.StarSystems);
        }

        [Fact]
        public void Generate_Creates_Requested_Number_Of_Systems_Within_Bounds()
        {
            var generator = new GalaxyGenerator();

            var options = new GalaxyGeneratorOptions
            {
                Seed = 1UL,
                Width = 200,
                Height = 150,
                StarSystemCount = 25
            };

            var galaxy = generator.Generate(options);

            Assert.Equal(options.StarSystemCount, galaxy.StarSystems.Count);

            // All systems inside bounds.
            Assert.All(galaxy.StarSystems, s =>
            {
                Assert.InRange(s.X, 0, options.Width - 1);
                Assert.InRange(s.Y, 0, options.Height - 1);
            });

            // Ids are unique.
            var ids = galaxy.StarSystems.Select(s => s.Id).ToList();
            Assert.Equal(ids.Count, ids.Distinct().Count());

            // Coordinates are unique.
            var coords = galaxy.StarSystems
                .Select(s => (s.X, s.Y))
                .ToList();
            Assert.Equal(coords.Count, coords.Distinct().Count());
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
                StarSystemCount = 30
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
    }
}