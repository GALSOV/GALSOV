# TIMS 4X SPACE GAME — Starter Solution Outline (C#/.NET 8, WPF)
Date: 2025-11-18

---

## 1) Solution Structure
```
Tims4X.sln
  ├─ Tims4X.Core               # Pure logic (no UI): rules, sim, AI, content, persistence
  ├─ Tims4X.UI.Desktop         # WPF app (MVVM) — Windows
  ├─ Tims4X.TurnRunner         # Console app for headless sims/batch testing
  └─ Tims4X.Tests              # xUnit tests
/Content                      # JSON data (tech, AI, galaxy params, planet types, hulls, weapons)
```

---

## 2) Project Purposes
- **Core**: deterministic turn engine, data models, systems (Galaxy, Economy, Research, Combat, AI).
- **UI.Desktop**: views, viewmodels, services/adapters to Core, theming.
- **TurnRunner**: batch simulation, AI tuning, regression tests; CSV/log outputs.
- **Tests**: unit + property-based + regression (fixed seeds).

---

## 3) .csproj Templates
**Tims4X.Core.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Serilog" Version="3.*" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.*" />
    <PackageReference Include="System.Text.Json" Version="8.*" />
  </ItemGroup>
</Project>
```

**Tims4X.UI.Desktop.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Tims4X.Core\Tims4X.Core.csproj" />
  </ItemGroup>
</Project>
```

**Tims4X.TurnRunner.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Tims4X.Core\Tims4X.Core.csproj" />
  </ItemGroup>
</Project>
```

**Tims4X.Tests.csproj**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
    <PackageReference Include="FluentAssertions" Version="6.*" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Tims4X.Core\Tims4X.Core.csproj" />
  </ItemGroup>
</Project>
```

---

## 4) Namespaces & Folders (Core)
```
Tims4X.Core
  /Common          # PRNG, math helpers, extensions
  /Content         # Loaders, schemas, validators
  /Model           # POCOs: GameState, Empire, System, Planet, Fleet, Tech ...
  /Systems         # Turn phases: Movement, Economy, Research, Combat, Diplomacy
  /AI              # Utility AI, personalities, planners
  /Persistence     # Save/load, version converters
```

---

## 5) Minimal Code Stubs
**PRNG (XorShift64*)**
```csharp
namespace Tims4X.Core.Common
{
    public sealed class XorShift64Star
    {
        private ulong _state;
        public XorShift64Star(ulong seed) => _state = seed == 0 ? 0x9E3779B97F4A7C15UL : seed;
        public ulong NextULong()
        {
            ulong x = _state;
            x ^= x >> 12; x ^= x << 25; x ^= x >> 27; _state = x;
            return x * 2685821657736338717UL;
        }
        public int NextInt(int min, int max) => (int)(NextULong() % (ulong)(max - min)) + min;
        public double NextDouble() => (NextULong() >> 11) * (1.0 / (1UL << 53));
    }
}
```

**GameState Skeleton**
```csharp
namespace Tims4X.Core.Model
{
    public sealed class GameState
    {
        public ulong GameSeed { get; init; }
        public int Turn { get; set; }
        public List<StarSystem> Systems { get; init; } = new();
        public List<Empire> Empires { get; init; } = new();
        public DiplomacyMatrix Diplomacy { get; init; } = new();
    }
}
```

**TurnEngine Skeleton**
```csharp
namespace Tims4X.Core.Systems
{
    using Tims4X.Core.Common;
    using Tims4X.Core.Model;

    public sealed class TurnEngine
    {
        public GameState ProcessTurn(GameState s)
        {
            var rng = new XorShift64Star(Combine(s.GameSeed, (ulong)s.Turn));
            OrdersSystem.LockIn(s);
            MovementSystem.Apply(s, rng);
            SupplySystem.Apply(s, rng);
            EncounterSystem.Resolve(s, rng);
            CombatSystem.Resolve(s, rng);
            EconomySystem.Tick(s, rng);
            ProductionSystem.Tick(s, rng);
            ResearchSystem.Tick(s, rng);
            ColonizationSystem.Tick(s, rng);
            PopulationSystem.Grow(s, rng);
            DiplomacySystem.Tick(s, rng);
            EventSystem.Tick(s, rng);
            AISystem.Plan(s, rng);
            s.Turn++;
            return s;
        }

        private static ulong Combine(ulong a, ulong b) => (a ^ 0x9E3779B97F4A7C15UL) + b + (a << 6) + (a >> 2);
    }
}
```

**GalaxyGenerator Skeleton**
```csharp
namespace Tims4X.Core.Systems
{
    using Tims4X.Core.Common;
    using Tims4X.Core.Model;

    public static class GalaxyGenerator
    {
        public static GameState CreateNew(ulong seed, int systems)
        {
            var rng = new XorShift64Star(seed);
            var state = new GameState { GameSeed = seed, Turn = 0 };
            for (int i = 0; i < systems; i++)
            {
                state.Systems.Add(new StarSystem { Id = i, X = rng.NextDouble(), Y = rng.NextDouble() });
            }
            GraphBuilder.ConnectKNearest(state.Systems, k: 3);
            return state;
        }
    }
}
```

**WPF App Shell**
```xml
<!-- App.xaml -->
<Application x:Class="Tims4X.UI.Desktop.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <SolidColorBrush x:Key="BackgroundBrush" Color="#111722" />
            <SolidColorBrush x:Key="ForegroundBrush" Color="#E6EAF0" />
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

```xml
<!-- MainWindow.xaml -->
<Window x:Class="Tims4X.UI.Desktop.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="TIMS 4X" Height="768" Width="1280" Background="{StaticResource BackgroundBrush}">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="3*"/>
            <ColumnDefinition Width="2*"/>
        </Grid.ColumnDefinitions>
        <ScrollViewer Grid.Column="0">
            <Canvas x:Name="GalaxyCanvas" />
        </ScrollViewer>
        <DockPanel Grid.Column="1" Margin="8">
            <TextBlock Text="System Inspector" FontSize="18"/>
            <!-- Bind to ViewModel properties -->
        </DockPanel>
    </Grid>
</Window>
```

**TurnRunner Program.cs**
```csharp
using Tims4X.Core.Model;
using Tims4X.Core.Systems;

var seed = args.Length > 0 ? ulong.Parse(args[0]) : 123456789UL;
var gs = GalaxyGenerator.CreateNew(seed, systems: 600);
var engine = new TurnEngine();
for (int i = 0; i < 200; i++)
{
    engine.ProcessTurn(gs);
    if (i % 50 == 0) Console.WriteLine($"Turn {gs.Turn}");
}
```

**xUnit Example**
```csharp
using Tims4X.Core.Model;
using Tims4X.Core.Systems;
using FluentAssertions;

public class DeterminismTests
{
    [Fact]
    public void SameSeedSameOutcome()
    {
        var a = GalaxyGenerator.CreateNew(42, 200);
        var b = GalaxyGenerator.CreateNew(42, 200);
        a.Systems.Count.Should().Be(b.Systems.Count);
    }
}
```

---

## 6) JSON Content Examples
**Planet Type**
```json
{
  "id": "terrestrial",
  "habitability": 0.8,
  "base_production": { "credits": 1.0, "ore": 1.2, "gas": 0.3 }
}
```

**AI Personality (excerpt)**
```json
{
  "id": "balanced",
  "weights": {
    "colonize.distance": -0.5,
    "colonize.quality": 0.9,
    "colonize.border_threat": -0.7
  },
  "difficulty": {
    "intel_noise": 0.05,
    "action_cooldown": 1,
    "search_depth": 2,
    "resource_modifier": 1.0
  }
}
```

---

## 7) Build & Run
1. Create solution and projects per structure above.
2. Add references and copy csproj templates.
3. Implement stubs in Core; wire UI minimal shell.
4. Run **Tims4X.TurnRunner** to validate deterministic loop.
5. Launch **Tims4X.UI.Desktop** to render the galaxy and inspect systems.

---

## 8) Immediate Backlog (Next 2–3 weeks)
- **M1**: PRNG, content loader, galaxy gen (k‑NN lanes), WPF canvas render + zoom/pan, save/load.
- **M2**: Turn engine phases, orders, economy tick, research scaffold, autosave + Serilog.
- **M3**: Utility AI (colonize/build) with JSON personalities; TurnRunner param sweeps; battle sim prototype.
- **M4**: Planetary zones abstraction, diplomacy stances, difficulty presets, UI overlays (influence/threat/supply).

---

## 9) Options & Variants
- **Avalonia UI** if cross‑platform becomes priority (ViewModels remain).
- **PBM Branch** later: TurnRunner accepts uploaded orders; merges; processes; distributes results.
- **Combat Visualizer** (optional): simple WPF timeline + event list.
