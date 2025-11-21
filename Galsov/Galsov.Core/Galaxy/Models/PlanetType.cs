// File: Galsov.Core/Galaxy/Models/PlanetType.cs
namespace Galsov.Core.Galaxy.Models
{
    /// <summary>
    /// Simple classification for planets.
    /// This can be expanded as game design evolves.
    /// </summary>
    public enum PlanetType
    {
        Unknown = 0,
        Terran,
        Desert,
        Ice,
        GasGiant,
        Barren
    }
}