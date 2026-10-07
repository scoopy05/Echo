# Echo – Progress Record

## Project setup (inspected 2026-10-07)
- Unity: 6000.6.4f1 (Unity 6)
- Render pipeline: URP 17.6.0 (`PC_RPAsset`, `Mobile_RPAsset` in `Assets/Settings`)
- Input: new Input System 1.20.0 only (`activeInputHandler: 1`); default actions in `Assets/InputSystem_Actions.inputactions`
- Color space: Linear
- Asset serialization: Force Text (good for Git)
- Dev machine: macOS 26.6, Apple Silicon (arm64)
- Target device: **TBD** (recommended: Mac desktop, 60 FPS)
- Camera perspective: **TBD – must confirm before movement/camera lessons**
- Scenes: only the template `SampleScene`
- Assets: none imported yet
- Git: not initialised yet
- Reference image (wooden study): not yet attached

## Roadmap
1. Project inspection, setup, Git, asset audit  <- we are here
2. Test scene, player movement, collisions, camera
3. Lighting practice room + early build
4. Player animation + interaction
5. Echo movement, actions, feedback
6. Security behaviour + perception
7. Noise, power, cooperation
8. Level blockout + playtesting
9. Environment art, lighting, sound, UI
10. Validated natural-language commands
11. Profiling, polish, tests, final build
12. Demo, README, portfolio

## Completed features / concepts practised
- (none yet)

## Current checkpoint
- Lesson 1: Git checkpoint. Test result: **pending**

## Known bugs
- (none)

## Asset inventory
| Need | Asset / source | Licence | Clips | Import status | Remaining |
|------|----------------|---------|-------|---------------|-----------|
| (audit not started) | | | | | |

## Decisions and reasons
- Input System (new) is already active, so we use it, not the old `Input.GetKey`.

## Next small step
- Finish Lesson 1, then audit assets and confirm camera perspective.
