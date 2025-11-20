# GALSOV — Galactic Sovereignty
https://github.com/galsov/galsov/blob/main/teaserbanner.png
## Overview
GALSOV is a single-player, grand-scale 4X strategy game built in **C#/.NET 8** with a **WPF UI**.  
It focuses on deterministic simulation, long campaigns, and moddable JSON-driven content.

### Key Features
- Deterministic PRNG for reproducibility.
- Galaxy generation with seeded randomness.
- Modular architecture:
```
GALSOV.sln
├─ GALSOV.Core        # Game logic (rules, sim, AI, persistence)
├─ GALSOV.UI.Desktop  # WPF UI (MVVM)
├─ GALSOV.TurnRunner  # Headless console for batch sims
├─ GALSOV.Tests       # xUnit tests
└─ Content            # JSON data (tech, AI, galaxy params, planet types)
```

### Roadmap
See **GALSOV_TODO_v3.2.md** for detailed development steps.

### Quick Start
Refer to **QuickStart_GALSOV.md** for setup and run instructions.
