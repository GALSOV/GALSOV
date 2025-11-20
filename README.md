# GALSOV — Galactic Sovereignty
![Teaser Banner](https://raw.githubusercontent.com/GALSOV/GALSOV/main/teaserbanner.png)
## Overview
GALSOV is a single-player, grand-scale 4X strategy game built in **C#/.NET 8** with a **WPF UI**.  
Using entirely AI guidance to create the project (Using a mix of Copilot, ChatGPT and Firefly),
I have extremely limited coding skills, and this is a combination of a boredom project and a learning
exercise as well as a curiosity driven project to test AI capabilities.
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
