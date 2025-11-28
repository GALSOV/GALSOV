// File: Galsov.Core/Sessions/GameSessionFactory.cs
using System;
using System.Linq;
using Galsov.Core.Galaxy.Generation;
using Galsov.Core.Galaxy.Models;

namespace Galsov.Core.Sessions
{
    /// <summary>
    /// Factory responsible for creating a new GameSession based on
    /// GalaxyGeneratorOptions. DI can later wrap this in an interface.
    /// </summary>
    public static class GameSessionFactory
    {
        public static GameSession Create(GalaxyGeneratorOptions galaxyOptions)
        {
            if (galaxyOptions is null)
                throw new ArgumentNullException(nameof(galaxyOptions));

            // 1. Generate the galaxy deterministically.
            var generator = new GalaxyGenerator();
            var galaxy = generator.Generate(galaxyOptions);

            if (galaxy.StarSystems.Count == 0)
                throw new InvalidOperationException("Generated galaxy contains no star systems.");

            // For v0.1 we pick the first star system as the home system.
            var homeSystem = galaxy.StarSystems[0];

            // Prefer a habitable world if one exists, otherwise just pick the innermost planet.
            var homePlanet = homeSystem.Planets
                .OrderByDescending(p => p.IsHabitable)
                .ThenBy(p => p.OrbitIndex)
                .FirstOrDefault();

            if (homePlanet is null)
                throw new InvalidOperationException("Home system has no planet to colonise. Check generator settings.");

            var playerEmpire = CreatePlayerEmpire(homeSystem, homePlanet);

            return new GameSession(galaxy, playerEmpire);
        }

        private static Empire CreatePlayerEmpire(StarSystem homeSystem, Planet homePlanet)
        {
            var empire = new Empire
            {
                Id = 1,
                Name = "Player Empire",

                // Simple starting economy numbers – tuned later.
                Credits = 500,
                Production = 20,
                Science = 10
            };

            // Starting colony.
            var colony = new Colony
            {
                StarSystemId = homeSystem.Id,
                PlanetId = homePlanet.Id,
                Name = "Homeworld",
                Population = 100,
                ProductionPerTurn = 20
            };
            empire.Colonies.Add(colony);

            // Starting fleets (1–2 small fleets for now).
            var defenceFleet = new Fleet
            {
                Id = 1,
                Name = "Home Defence Fleet",
                CurrentStarSystemId = homeSystem.Id,
                MovementMode = FleetMovementMode.Idle,
                SupplyCapacity = 100,
                SuppliesCurrent = 100
            };
            defenceFleet.Ships.Add(new Ship
            {
                Id = 1,
                ShipClassId = "corvette",
                Hull = 100,
                Speed = 10,
                SupplyUsePerTurn = 1
            });
            defenceFleet.Ships.Add(new Ship
            {
                Id = 2,
                ShipClassId = "corvette",
                Hull = 100,
                Speed = 10,
                SupplyUsePerTurn = 1
            });
            empire.Fleets.Add(defenceFleet);

            var scoutFleet = new Fleet
            {
                Id = 2,
                Name = "Scout Wing",
                CurrentStarSystemId = homeSystem.Id,
                MovementMode = FleetMovementMode.Idle,
                SupplyCapacity = 50,
                SuppliesCurrent = 50
            };
            scoutFleet.Ships.Add(new Ship
            {
                Id = 3,
                ShipClassId = "scout",
                Hull = 60,
                Speed = 12,
                SupplyUsePerTurn = 1
            });
            empire.Fleets.Add(scoutFleet);

            // Basic tech set – string IDs that will hook into data later.
            empire.KnownTechIds.Add("tech_basic_jump");
            empire.KnownTechIds.Add("tech_basic_sensors");
            empire.KnownTechIds.Add("tech_basic_colony");

            // For now, treat known techs as “active” as well.
            empire.ActiveTechIds.AddRange(empire.KnownTechIds);

            // Knowledge: home system is fully surveyed, others unknown.
            var homeKnowledge = empire.Knowledge.GetOrCreate(homeSystem.Id);
            homeKnowledge.KnowledgeState = KnowledgeState.Surveyed;
            homeKnowledge.PresenceBand = PresenceBand.Minor;
            homeKnowledge.DetectionLevel = 4;
            homeKnowledge.LastUpdatedTurn = 1;

            return empire;
        }
    }
}
