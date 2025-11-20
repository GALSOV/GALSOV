# TIMS 4X SPACE GAME — Project Summary
Date: 2025-11-19

This file summarizes everything **decided** and **undecided** about the TIMS 4X game project. Paste this into a new chat to give full context.

---

## ✅ Decided

### Vision & Pillars
- Grand-scale single-player 4X strategy game.
- Inspired by *The Expanse* (plausible tech, gritty realism).
- Designed for **long campaigns** (hundreds of turns, possibly years).
- Deterministic simulation for save compatibility.
- Data-driven architecture (JSON content, mod-friendly).

### Language & Platform
- **Language:** C#
- **Framework:** .NET 8
- **IDE:** Visual Studio Community (2022–2026)
- **UI:** WPF (Windows-only for v1)

### Architecture
- Solution structure:
```
Tims4X.sln
├─ Tims4X.Core          # Game logic (rules, sim, AI, persistence)
├─ Tims4X.UI.Desktop    # WPF UI (MVVM)
├─ Tims4X.TurnRunner    # Headless console for batch sims
├─ Tims4X.Tests         # xUnit tests
└─ Content              # JSON data (tech, AI, galaxy params, planet types)
```

### Core Decisions
- **PRNG:** XorShift64* for deterministic randomness.
- **Galaxy Generation:** Seeded, positions + hyperlane graph (k-NN).
- **Tech Tree:** Field-based progression (Physics, Engineering, etc.), chaining unlocks.
- **Combat:** Simulated (not real-time), deterministic resolution.
- **UI:** Minimalist, data-forward with galaxy map and inspector.
- **AI:** Utility-based, personalities in JSON, difficulty scaling via resource modifiers and perception noise.
- **Resources:** Credits, Ore, Gas (luxuries optional later).
- **Save/Load:** JSON with version headers for compatibility.

### Roadmap Highlights
- Milestone 1: Solution setup, PRNG, galaxy generator, WPF shell.
- Milestone 2: Turn engine skeleton, save/load, basic economy & research.
- Milestone 3: AI foundations, TurnRunner batch sims.
- Milestone 4: Combat sim, planetary zones, diplomacy scaffold.
- Milestone 5: UI overlays, performance pass, packaging.

---

## ❓ Undecided / TBD
- **Galaxy connectivity:** Hyperlanes only or hybrid with fuel-limited open travel?
- **Planet types:** Final list and habitability model (fixed vs tech-dependent).
- **Resource scarcity:** Finite nodes vs infinite with diminishing returns.
- **Combat depth:** Boarding rules, morale, sabotage mechanics.
- **Planetary warfare:** Zone count, terrain effects, reinforcement pacing.
- **Diplomacy:** Espionage in v1 or later?
- **Victory conditions:** Domination only or add tech ascendancy, federation vote, score?
- **Late-game events:** Galaxy-wide crises or rare tech breakthroughs?
- **Modding boundaries:** JSON only or allow script hooks?
- **Cross-platform:** Stick to WPF or plan Avalonia later?

---

## 🔍 How to Use This File for Future Chats
- Paste this entire file into the first message of a new conversation.
- Add any **updates** or **answers to TBD items** before pasting.
- Mention what you want to focus on (e.g., "Implement Milestone 1" or "Decide combat depth").

