# Quick Start Guide for GALSOV

## 1. Unzip and Organize Files
- Extract all files from `GALSOV_CompletePackage.zip`.
- Place files into the following structure:
```
GALSOV.sln
├─ GALSOV.Core          # Add XorShift64Star.cs, GalaxyGenerator.cs, GameState.cs, StarSystem.cs
├─ GALSOV.UI.Desktop    # Add MainWindow.xaml, MainWindow.xaml.cs
├─ GALSOV.TurnRunner    # Add Program.cs
└─ Content              # Reserved for JSON data later
```

## 2. Open in Visual Studio
- Open Visual Studio → Open Project/Solution → Select `GALSOV.sln`.
- Ensure projects are named as above.

## 3. Build and Run
- Set `GALSOV.TurnRunner` as Startup Project → Press Ctrl+F5 → Expect `Galaxy created: 600 systems`.
- Set `GALSOV.UI.Desktop` as Startup Project → Press F5 → See galaxy map with stars.

## 4. Key Concepts
- **WPF:** Windows Presentation Foundation for UI.
- **MVVM:** Model-View-ViewModel pattern (later phases).
- **PRNG:** XorShift64Star ensures deterministic randomness.
- **Turn Engine:** Skeleton defines game loop phases.

