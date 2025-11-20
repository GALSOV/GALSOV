# Project Setup Guide for TIMS 4X
Date: 2025-11-18

---

## 1. Prepare Your Machine
### Install .NET SDK
- Download from [dotnet.microsoft.com](https://dotnet.microsoft.com).
- Verify: `dotnet --version`.

### Install an IDE
- **Visual Studio 2022** (Community Edition): Full-featured.
- Or **VS Code** (lightweight) + C# extension.

### Install Git
- Download from [git-scm.com](https://git-scm.com).

### Recommended Plugins
- **Visual Studio:** GitHub extension, Productivity Power Tools.
- **VS Code:** C# extension, GitLens.

---

## 2. Folder Structure (and WHY)
```
Tims4X.sln                # Solution file (groups projects)
  ├─ Tims4X.Core          # Game logic (rules, AI, galaxy gen)
  ├─ Tims4X.UI.Desktop    # WPF UI (presentation layer)
  ├─ Tims4X.TurnRunner    # Console app for batch sims
  ├─ Tims4X.Tests         # Automated tests
/Content                  # JSON data (tech, planets, AI personalities)
```
**Why this structure?**
- **Separation of concerns:** Core logic is independent of UI.
- **Testability:** Core can be tested without UI.
- **Flexibility:** Add PBM or multiplayer later without breaking UI.

---

## 3. Tools for Building
- **.NET CLI:** Create projects (`dotnet new console`, `dotnet new wpf`).
- **NuGet Packages:** Serilog (logging), System.Text.Json (serialization).

---

## 4. Steps to Create Solution
```bash
dotnet new sln -n Tims4X
cd Tims4X
dotnet new classlib -n Tims4X.Core
dotnet new wpf -n Tims4X.UI.Desktop
dotnet new console -n Tims4X.TurnRunner
dotnet new xunit -n Tims4X.Tests
dotnet sln add Tims4X.Core Tims4X.UI.Desktop Tims4X.TurnRunner Tims4X.Tests
```

---

## 5. Why .gitignore?
- Prevents committing build artifacts (`bin/`, `obj/`) and secrets.

Example:
```
bin/
obj/
*.user
*.suo
```

---

## 6. Next Steps
- Commit initial structure to GitHub.
- Add README.md with build instructions.
- Start implementing stubs (PRNG, GameState, TurnEngine).

