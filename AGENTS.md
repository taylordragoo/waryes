# Repository Guidelines

## Project Structure & Module Organization

This is a Unity 6.3 project (`6000.3.19f1`). Core RTS gameplay, prefabs, and enabled scenes (`Loader`, `MainMenu`, `Example`, and `Example_2`) live in `Assets/uSim_RTS/`. URP assets are in `Assets/Settings/`, terrain content in `Assets/MicroVerse-Presets/`, and imported art in `Assets/tripolygon/`. `Assets/warno-hud/` is a separate Vite/React HUD prototype. Package versions are tracked under `Packages/`.

## Build, Test, and Development Commands

- Open the repository root with Unity Hub using Unity `6000.3.19f1`; press Play from an enabled RTS scene for the normal smoke test.
- Run Unity tests from **Window > General > Test Runner**, or headlessly in PowerShell:
  `& $env:UNITY_EXE -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/EditMode.xml -quit`
- `npm --prefix Assets/warno-hud install` installs HUD dependencies.
- `npm --prefix Assets/warno-hud run dev` starts the HUD at port 3000.
- `npm --prefix Assets/warno-hud run lint` checks TypeScript; replace `lint` with `build` for a production bundle.

There is currently no repository-specific automated Unity player build script; use **File > Build Profiles** and keep build outputs under ignored `Build/` or `Builds/` directories.

## Coding Style & Naming Conventions

Use four spaces and Allman braces for C#. Preserve the `uSimRTS` namespace and `uSimRTS_` type prefix. Use PascalCase for types and methods, camelCase for locals and serialized fields, and avoid renaming serialized fields without migration attributes. For the HUD, use TypeScript, PascalCase React components, and camelCase props/functions. Do not edit generated `.csproj` or `.slnx` files. Commit every Unity asset with its `.meta` file.

## Testing Guidelines

The Unity Test Framework `1.6.0` is installed. Existing EditMode and PlayMode coverage is limited to `Assets/uSim_RTS/NavMeshComponents/Tests/`. Put new project-owned tests under `Assets/Tests/EditMode/` or `Assets/Tests/PlayMode/`, with filenames ending in `Tests.cs`. Test input, selection, movement orders, and scene loading after gameplay changes. No coverage threshold is currently enforced.

## Commit & Pull Request Guidelines

History currently contains only `Initial check-in`, so no formal convention exists. Use short, imperative subjects such as `Fix RTS drag selection`. Keep commits scoped and exclude `Library/`, `Temp/`, `Logs/`, generated IDE files, and builds. Pull requests should explain behavior changes, list tested scenes and commands, link relevant issues, and include screenshots or clips for scene, terrain, or HUD changes.

## Security & Configuration

Never commit API keys or `.env.local`; copy `Assets/warno-hud/.env.example` for local HUD configuration. Review package-lock changes and large binary assets before committing.
