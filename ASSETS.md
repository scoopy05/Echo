# Echo – Asset Inventory

Raw downloads live OUTSIDE the project: `~/Desktop/unity/EchoAssets_Raw` (audited 2026-10-07).
Only files we actually use get copied into `Assets/`.

## Packs downloaded
| Pack | Version | Licence (from file in download) | Size |
|------|---------|----------------------------------|------|
| Quaternius Universal Base Characters | Standard (free) | CC0 1.0 | 127 MB |
| Quaternius Universal Animation Library (UAL1) | Standard (free) | CC0 1.0 | 61 MB |
| Quaternius Modular Sci-Fi MegaKit | Standard (free) | CC0 1.0 | 64 MB |

CC0 = public domain; credit "Quaternius" in the game credits as courtesy.

## Inventory
| Need | Selected asset | Source | Licence | Clips | Import status | Remaining work |
|------|----------------|--------|---------|-------|---------------|----------------|
| Player model | `Superhero_Male_FullBody.fbx` or `Superhero_Female_FullBody.fbx` (only these 2 in free version) | Base Characters | CC0 | – | Not imported | Choose one; set Humanoid rig; URP materials |
| Player animations | `UAL1_Standard.fbx` (in-place) and `UAL1_Standard_RM.fbx` (root motion), 23 MB each | Animation Library | CC0 | see list below | Not imported | Import one file as Humanoid; verify retarget |
| Walls, floors, doors, props | MegaKit FBX: 84 walls, 37 platforms (incl. `Door_Simple`, `Door_DarkMetal`), 28 props (`Prop_Computer`, `Prop_AccessPoint`, `Prop_Light_*`, `Prop_Fan_Small`, crates, vents), 8 columns, 29 decals | MegaKit | CC0 | – | Not imported | Import a subset when building lighting room |
| Echo (floating companion) | **None** | – | – | – | Gap | Build from primitives + emissive "eye"; procedural hover |
| Security robots (x2) | **None** (MegaKit has 3 alien models, not robots) | – | – | – | Gap | Primitive-based robot, or audit another pack later |
| Sounds | Kenney Sci-fi Sounds | – | – | – | Not downloaded | Download when audio lesson starts |
| UI | Kenney UI Pack – Sci-Fi | – | – | – | Not downloaded | Download when UI lesson starts |

## Rig compatibility (checked by parsing FBX node names)
- Character and animation files share the **same skeleton**: identical bone names
  (`pelvis`, `spine_*`, `clavicle_l`, `upperarm_l`, `lowerarm_l`, `hand_l`, `thigh_l`, `calf_l`, `foot_l`, `ball_l`, fingers...).
- Only differences are mesh nodes (`Mannequin` vs `SuperHero_Male`, `Eyes`, `Eyebrows`).
- Not yet verified inside Unity (Humanoid Avatar mapping).

## Animation clips in UAL1 Standard (43 incl. T-pose)
- **Locomotion:** Idle_Loop, Walk_Loop, Walk_Formal_Loop, Jog_Fwd_Loop, Sprint_Loop
- **Stealth:** Crouch_Idle_Loop, Crouch_Fwd_Loop
- **Jump/roll:** Jump_Start, Jump_Loop, Jump_Land, Roll
- **Interaction:** Interact, PickUp_Table, Fixing_Kneeling, Push_Loop, Idle_Torch_Loop, Idle_Talking_Loop
- **Failure:** Hit_Chest, Hit_Head, Death01
- **Sitting:** Sitting_Enter, Sitting_Idle_Loop, Sitting_Talking_Loop, Sitting_Exit
- **Not needed for this game:** Pistol_* (6), Punch_* (2), Sword_* (2), Spell_Simple_* (4), Swim_* (2), Dance_Loop, Driving_Loop, A_TPose

## Animation gaps and their design consequence
| Missing | Consequence / workaround |
|---------|--------------------------|
| Walk backwards / strafe left-right | Character should turn to face movement direction (no strafing) |
| Crouch backwards / strafe | Same as above |
| Turn-in-place | Rotate smoothly while idle; acceptable for stylized game |
| Door open, button press, climb/vault | Use `Interact` for doors/terminals/buttons; no climbing in level design |
