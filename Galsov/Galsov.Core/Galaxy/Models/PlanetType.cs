namespace Galsov.Core.Galaxy.Models;

/// <summary>
/// Basic planet categories.
/// Design choice: gameplay-centric, not scientific.
/// </summary>
public enum PlanetType
{
    Unknown = 0,
    Rocky,
    GasGiant,
    Ice,
    Desert,
    Ocean,
    Barren,
    AsteroidBelt
}