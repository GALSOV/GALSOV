---
name: Custom issue template
about: Describe this issue template's purpose here.
title: ''
labels: ''
assignees: ''

---

## WHAT
<!-- Describe the task clearly -->
Example: Implement Turn Engine skeleton with phases (Orders → Movement → Combat → Economy → Research → AI).

## HOW
<!-- Outline steps or approach -->
- Create `TurnEngine.cs` in `GALSOV.Core.Systems`.
- Define phases as methods or `ITurnPhase` interface.
- Ensure deterministic updates using `GameState`.

## WHY
<!-- Explain why this matters -->
Defines the deterministic loop and provides structure for all mechanics.
