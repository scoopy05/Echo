# Echo – Progress Record

## Project setup (inspected 2026-10-07)
- Unity: 6000.6.4f1 (Unity 6)
- Render pipeline: URP 17.6.0 (`PC_RPAsset`, `Mobile_RPAsset` in `Assets/Settings`)
- Input: new Input System 1.20.0 only (`activeInputHandler: 1`); default actions in `Assets/InputSystem_Actions.inputactions`
- Color space: Linear
- Asset serialization: Force Text (good for Git)
- Dev machine: macOS 26.6, Apple Silicon (arm64)
- Target device: Mac desktop, 60 FPS (assumed; Imran did not object)
- Camera perspective: **A – free third-person orbit** (chosen 2026-10-07)
- Scenes: only the template `SampleScene`
- Assets: 3 Quaternius Standard packs downloaded to `~/Desktop/unity/EchoAssets_Raw` (none imported yet)
- Git: initialised, remote `github.com/scoopy05/Echo`, branch `main`, first commit `a2bbc52`
- Version control: GitHub only. `com.unity.collab-proxy` (Unity Version Control) is installed but unused.
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
- Git setup: `.gitignore`, `.meta` files, first commit and push. Verified: no Library/Temp/csproj tracked.

## Current checkpoint
- Lesson 1: Git checkpoint. Test result: **passed** (working tree clean, pushed)
- Lesson 2: Asset download + audit. Test result: **passed** (commit `a2ef13a`)
- Lesson 3: Sandbox scene + Player hierarchy (root + Visual child). Test result: **pending**

## Known bugs
- (none)

## Asset inventory
See [ASSETS.md](ASSETS.md). Key gaps: no Echo model, no security-robot model, no strafe/backward clips.

## Decisions and reasons
- Input System (new) is already active, so we use it, not the old `Input.GetKey`.
- Raw asset packs live outside the project (`~/Desktop/unity/EchoAssets_Raw`); only used files get imported.
- Use one version-control system (GitHub) to avoid two systems fighting over the same files.
- Camera A (free orbit): most immersive. Costs: camera-wall collision needed; stealth readability must come from UI/markers/sound rather than overview.
- Work with current free assets; swap models later. To make swapping cheap, gameplay components live on the Player root, the model lives in a `Visual` child.

## Next small step
- Finish Lesson 3, then choose CharacterController vs Rigidbody and read input.
