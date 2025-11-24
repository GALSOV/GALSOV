namespace Galsov.Core.Galaxy.Models;

/// <summary>
/// Simplified stellar classification.
/// Design choice: grouped + "Other" for flexibility,
/// not a fully realistic spectral model.
/// </summary>
public enum StarClass
{
    Unknown = 0,
    BlueGiant,
    White,
    Yellow,
    Orange,
    Green,
    RedDwarf,
    Neutron,
    BlackHole,
    Other
}