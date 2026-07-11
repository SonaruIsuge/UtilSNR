# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2026-07-11
### Fixed
- Separated single root `UtilSNR.asmdef` into two assembly definitions:
  - `Runtime/UtilSNR.Runtime.asmdef` for runtime scripts.
  - `Editor/UtilSNR.Editor.asmdef` with `includePlatforms: ["Editor"]` for editor scripts.
- Resolved `CS0103` compile errors (`EditorGUILayout`, `EditorGUI` not found) caused by
  editor-only types being stripped when compiled under a platform-unrestricted assembly.

## [1.1.0] - 2026-06-12
### Added
- **Runtime Utilities:**
  - Object Pooling system: `IPoolable`, `PooledObject`, `PoolManager`.
  - Math utilities: `MathUtils`.
  - Common patterns: `TSingleton`, `TSingletonBehaviour`, `TSceneSingletonBehaviour`.
  - Extensions: `CancellationTokenSourceExtensions`.
- **Editor Utilities:**
  - `ScriptTemplatePreprocessor` for Unity script templates.
  - `EditorHelper` and `GizmoHelper`.
  - `ShowIfAttribute` for conditional inspector display.
