# TIMS 4X SPACE GAME — Design Document (v0.1)
Date: 2025-11-18
Owner: Tim Bullock (Design lead)

---

## 1) Vision & Pillars
**Vision:** A grand‑scale, single‑player, deterministic 4X set in a plausibly near‑future universe (more *The Expanse* than *Star Trek*), designed for very long campaigns (hundreds of turns, potentially years of real‑time play) without breaking saves across updates.

**Pillars**
- **Longevity:** Save compatibility, deterministic simulation, versioned content.
- **Discovery:** Thorough galaxy generation at start; limited initial intel; meaningful exploration.
- **Plausible Tech:** Layered, field‑driven research; engineering constraints; late shields if any.
- **Operational Warfare:** Expanse‑style space combat (kinetics, missiles, point defense, repair, boarding).
- **Protracted Planetary Wars:** Multi‑turn ground campaigns with shifting space control.
- **Data‑Driven:** JSON content, tunable AI personalities, mod‑friendly schemas.

---

## 2) Scope (v1 Single‑Player)
- Single‑player only (PBM/multiplayer considered later).
- 2D strategic presentation with data‑rich panels.
- Focus on colonization, logistics, economy, research, diplomacy, and strategic combat resolution.
- No tactical real‑time control; combat is simulated but inspectable and deterministic.

**Non‑Goals (v1)**
- Real‑time battles; detailed ship interiors; multiplayer; atmospheric flight.

---

## 3) Scale Targets
- **Systems:** Small 200, Medium 600, Large 1,200 (stretch 2,000).
- **Empires:** 6–16 AI opponents.
- **Fleets:** Thousands of ships total; per‑system combat resolves in < 200 ms on mid hardware.
- **Planets:** Numeric size 0.01–10.0 controlling capacity & production scaling.

---

## 4) Save Compatibility & Determinism
- **Deterministic Turn Engine:** Single seeded PRNG per turn; no system time or unordered iteration.
- **Versioned Saves:** Save header `{ gameVersion, dataVersion }`; converters for forward‑compat minor patching.
- **Content Hashing:** Saved with content versions to enable safe patching; warning on mismatch with auto‑converter where possible.

---

## 5) Core Loops
**Player Loop (each turn)**
1. Survey intel; set policies and priorities.
2. Queue research, production, colonization, military orders.
3. End Turn → engine simulates movement, combat, economy, research, events.

**Engine Phases**
1) Orders lock‑in → 2) Movement & Logistics → 3) Encounters & Combat → 4) Economy & Production → 5) Research → 6) Colonization & Pop → 7) Diplomacy & Events → 8) AI Planning.

---

## 6) Galaxy & Exploration
- **Generation:** Thorough at new game: positions, hyperlane graph (k‑NN/Delaunay), stars, planets, resources, anomalies.
- **Fog of Space:** Player sees only origin and scanned areas; sensor range & probes reveal gradually.
- **Lore Justification:** Humanity can seed colonies/probes but lacks galaxy‑wide sensing.

---

## 7) Colonization & Growth
- Automated colony ships (multiple per planet possible) establish rapid‑growth infrastructure (clone‑vat–inspired but not literal clones).
- Planet **type** (e.g., terrestrial, oceanic, desert, ice, gas giant) × **size** (0.01–10.0) × **habitability** determines capacity, growth, upkeep.
- Colonization requires logistics (ships, supplies); early outposts → developed colonies.

---

## 8) Economy & Resources
- **Credits:** Trade & taxation; upkeep, diplomacy, projects.
- **Ore:** Metals/minerals for construction.
- **Gas:** Propellants/exotics for drives, reactors, missiles.
- Optional later: advanced materials/luxuries (v2).
- Trade routes (v1.1+) and diminishing returns knobs for balance.

---

## 9) Research & Technology
- **Fields‑first:** Invest in Physics/Engineering/Materials/Computing/Biology; accumulate progress → reveal concepts → unlock applied techs.
- **Chaining:** Concept dependencies unlock targeted items (e.g., T2 Drives require Field thresholds + Concepts).
- **Rarity:** Optional rare breakthroughs for replay variety (seeded; deterministic per game).
- Late shields (if any) as advanced composite defenses; emphasis on armor, repair, ECM, PD.

---

## 10) Space Combat (Simulated)
- **Model:** Deterministic resolution with initiative, thrust/turn limits, weapon envelopes.
- **Weapons:** Railguns, missiles/torpedoes, point defense, lasers for PD/comm.
- **Damage:** Armor penetration, component damage, repair crews.
- **Boarding:** Special ops to disable/capture; rules for defender advantage & countermeasures.
- **Outputs:** Battle report, timeline, key events; reproducible with seed.

---

## 11) Planetary Warfare (Protracted)
- **Zone Model:** Planet partitioned into strategic zones (landing ports, military centers, government hubs, population zones, undeveloped regions, oceans).
- **Tempo:** Attacker chooses focus (wide shallow gains vs narrow deep thrusts). Defender reallocates to hinder/deny.
- **Interplay with Space:** Space control affects reinforcement and special operations; roles can flip if orbital control changes.

---

## 12) Diplomacy & Events (v1 basic)
- Stances (peace, tense, war), treaties, tensions; events tied to exploration and borders.

---

## 13) AI & Difficulty
- **Utility AI:** Weighted features for actions (colonize, build, research, war) from JSON personalities.
- **Planner:** Goal phases (expand, secure, tech up) pick actions; search depth bounded for determinism.
- **Difficulty Levers:** Intel noise, action cooldown, search depth, modest economy modifiers; no unfair omniscience.
- **Telemetry:** Log decisions + feature values per turn for tuning.

---

## 14) UI/UX (WPF, MVVM)
- Galaxy map (zoom/pan), overlays (influence, supply, threat), system inspector, production/research queues, turn summary, battle/war reports.
- Minimalist, data‑forward presentation; themes for readability.

---

## 15) Data, Modding & Tooling
- JSON content for tech, hulls, weapons, planet types, AI personalities, galaxy params.
- Hot‑reload in dev builds; schema validation on load.
- Save games include content version references.

---

## 16) Performance & Testing
- Data‑oriented hot paths (arrays/lists), caching maps (threat, supply), batch combat per system.
- Tests: unit (xUnit), property‑based (FsCheck‑style), regression (fixed seeds), and micro‑benchmarks (BenchmarkDotNet).

---

## 17) Roadmap (Draft)
**Milestone 1:** Project skeleton, PRNG, content loader, galaxy gen (k‑NN lanes), WPF shell map, save/load.

**Milestone 2:** Turn engine phases, orders, economy, research scaffold; Serilog; autosaves.

**Milestone 3:** Utility AI (colonize/build), threat map, personalities; TurnRunner for batch sims; balance logs.

**Milestone 4:** Combat sim (space) + report; planetary zones (abstract); diplomacy scaffold; difficulty presets.

**Milestone 5:** UX polish, overlays, tooltips; content completeness; performance pass; packaging.

---

## 18) Open Questions
1. Final galaxy connectivity: hyperlanes only, or hybrid with fuel‑limited open travel?
2. Exact planet type list and habitability model (tech‑dependent or fixed modifiers)?
3. Resource scarcity tuning: finite nodes vs infinite with diminishing yields?
4. Zone count per planet and terrain effects; weathering of defenses over time?
5. Boarding: capture conditions, crew morale, counter‑boarding, legal/diplomatic consequences?
6. Diplomacy breadth for v1: espionage now or later?
7. Victory conditions: domination, tech ascendancy, federation vote, score?
8. Modding boundaries in v1 (safe ranges for JSON, script hooks?)
