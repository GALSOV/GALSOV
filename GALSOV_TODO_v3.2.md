
## GALSOV Development Roadmap v3.2

---

### ✅ Project Structure (Unchanged)
```
GALSOV.sln
├─ GALSOV.Core        # Game logic (rules, sim, AI, persistence)
├─ GALSOV.UI.Desktop  # WPF UI (MVVM)
├─ GALSOV.TurnRunner  # Headless console for batch sims
├─ GALSOV.Tests       # xUnit tests
└─ Content            # JSON data (tech, AI, galaxy params, planet types)
```

---

### ✅ Steps 1–5 (Completed)
1) **Environment Setup** ✅  
2) **Solution Structure** ✅  
3) **Deterministic PRNG (XorShift64*)** ✅  
4) **Galaxy Generator** ✅  
5) **WPF UI Shell** ✅

---

### ▶ Steps 6–10 (Detailed)

#### **6. Turn Engine Skeleton**
- **WHAT:** Add `TurnEngine.cs` with phases: **Orders → Movement → Combat → Economy → Research → AI** (include events placeholders).
- **WHO:** Developer.
- **HOW:** `GALSOV.Core.Systems/TurnEngine.cs`; define an interface like `ITurnPhase` or simple methods first; ensure phases accept/return `GameState`.
- **WHY:** Defines deterministic loop; ensures all mechanics have a home.
- **Includes:** Explicit placeholders for **Colonization**, **Ship/Station building**, **Troop recruitment**, **Research**, **Diplomacy**, and **Event dispatch**.

#### **7. Save/Load System (Early)**
- **WHAT:** JSON serialization for `GameState` using `System.Text.Json`; versioned header.
- **WHO:** Developer.
- **HOW:** `GALSOV.Core.Persistence/SaveLoad.cs`; add `GameVersion`, `SchemaVersion`; provide `Save(string path, GameState)` and `Load(string path)`.
- **WHY:** Persistence, debugging, reproducibility.
- **Notes:** Add round-trip unit tests (see Step 16).

#### **8. AI Personality JSON (Scaffold)**
- **WHAT:** Create `Content/AI/Personalities/*.json` with utility weights (stub content is fine for now).
- **WHO:** Developer.
- **HOW:** Define schema (e.g., `name`, `weights`, `biases`, `riskTolerance`); loader in `GALSOV.Core.AI`.
- **WHY:** Data-driven, moddable AI.

#### **9. Economy & Research Basics**
- **WHAT:** `EconomySystem` (resource tick) and `ResearchSystem` (queued tech progress).
- **WHO:** Developer.
- **HOW:** Minimal structs/classes + deterministic updates per turn.
- **WHY:** Foundation for strategy and feedback loops.

#### **10. TurnRunner Batch Simulation (Seeded)**
- **WHAT:** Enhance console app to run **N seeds**, output CSV of metrics (e.g., system distribution, economy totals).
- **WHO:** Developer.
- **HOW:** Args: `--seeds <count> --systems <n> --out <file.csv>`; log minimalist KPIs.
- **WHY:** System validation at scale; regression tracking.

---

### ▶ **New/Adjusted Items Merged from Discussion**

#### **11. WPF: Show 600 Systems by Default**
- **WHAT:** Update `MainWindow.xaml.cs` call to `GalaxyGenerator.CreateNew(seed, 600)`.
- **HOW:** Replace hardcoded `200` with `600`.
- **WHY:** Align UI with target scale and console runner behavior.

#### **12. WPF: Configurable Seed & System Count**
- **WHAT:** Add simple UI inputs (TextBox/Slider) or app settings to control **seed** and **system count** at runtime.
- **HOW:**  
  - Minimal: read from `appsettings.json` or environment variables.  
  - Better: add a small **control panel** (seed TextBox, system count Slider/UpDown, “Generate” button).
- **WHY:** Faster iteration, reproducible test cases.

#### **13. WPF Rendering Performance @ 600+**
- **WHAT:** Improve rendering path to handle 600–2000 systems smoothly.
- **HOW (phased):**
  1) Replace `Ellipse` per system with a **`DrawingVisual`** batch draw (single `OnRender` pass).
  2) Optional: cache bitmap with `WriteableBitmap` for static frames.
  3) Defer hit testing/selection to later (not needed now).
- **WHY:** Avoid UI thread overhead from thousands of WPF elements.

#### **14. Input Validation & Error Handling**
- **WHAT:** Harden seed parsing (console & UI) and parameter bounds.
- **HOW:**  
  - Console: try-parse for `ulong`; fallback + message.  
  - UI: numeric-only inputs; min/max for system count (e.g., 10–10,000).
- **WHY:** Prevent runtime errors and invalid states.

#### **15. Save/Load Hooks in UI**
- **WHAT:** Add “Save” & “Load” buttons in WPF that call persistence layer.
- **HOW:** Bind commands; default path to a `Saves` folder; remember last path.
- **WHY:** UX for testing and demos; validates persistence.

#### **16. Unit Tests (Initial Coverage)**
- **WHAT:** Add xUnit tests in `GALSOV.Tests`.
- **HOW:**  
  - `XorShift64Star` determinism: same seed → same sequence.  
  - `GalaxyGenerator` determinism: seed/systemCount stable positions.  
  - Save/Load **round-trip**: serialize+deserialize equality on key fields.  
  - Input guardrails: invalid args handled gracefully.
- **WHY:** Locks determinism and prevents regressions.

---

### ▶ Steps 17–26 (Existing, kept for context)
17) **Space combat resolution** (deterministic battle model).  
18) **Planetary warfare zones & reinforcement logic**.  
19) **Tech chaining & rare breakthroughs**.  
20) **Diplomacy stances & treaties**.  
21) **UI overlays** (influence, supply, threat maps).  
22) **Autosave & logging** (Serilog).  
23) **Performance optimization** (profiling, caching).  
24) **Modding support** (JSON schemas, hot reload).  
25) **Difficulty presets** (resource modifiers, perception noise).  
26) **Packaging & installer** (self-contained publish).

---

## Actionable Checklist (Next 3–5 Days)

1. **Turn Engine skeleton** (`TurnEngine.cs`) with phase methods and stubs.  
2. **Save/Load** (`SaveLoad.cs` in Core.Persistence) + UI hooks.  
3. **WPF**: switch to **600 systems**; add **seed/system inputs**.  
4. **Rendering perf**: swap to **DrawingVisual** render path.  
5. **Tests**: PRNG determinism, galaxy determinism, save/load round-trip, input parsing.
