# SaltMap (Enhanced Fork)

A minimap and a full map page for **Salt and Sanctuary** (Steam, Windows).

> **Note:** This is an enhanced fork of the original [SaltMap](https://github.com/Buzzed-Lightyaw/SaltMap) project by **Buzzed-Lightyaw**. It includes several major improvements, including a Fog of War, full localization, JSON-configurable hotkeys, and UI fixes. See [Fork Features](#enhanced-fork-features) below.

## Enhanced Fork Features

- **Fog of War:** The map starts black and is revealed as you explore, with smooth gradient edges. Defeating bosses reveals large areas. Progress is saved separately for each character save slot (`fog_<slot>.dat`).
- **Full Localization:** The mod's UI (map filters, hints, guide) is fully translated. It automatically syncs with the game's language (e.g., ID 9 = Russian) via a Harmony hook. Includes translated boss names and region names.
- **JSON Hotkeys:** All mod hotkeys (minimap toggle, zoom) are moved to `Mods/SaltMapControls.json`. You can easily change them without recompiling.
- **Menu Speedup:** Removed the sluggish ~1 second input delay when opening the game's main menu. Menus are now instantly responsive.
- **Native UI Icons:** Fixed missing gamepad stick and D-pad icons in the map's control hints by using the game's native `replaceIcons = 2` render mode.

## Original Features

- **Minimap** in the top-right corner, tucked into the HUD's corner ornament, following
  your character, with zoom levels.
  - ![minimap](docs/mini-map.png)
- **Map page** in the Escape menu (a new map icon after the bag, skull and gear): pan,
  zoom, centre on yourself, and see the name of any icon.
  - ![map](docs/menu-map.png)
- **Icons** for items, NPCs, bosses and sanctuaries. Item and boss icons **disappear once
  you have collected the item or beaten the boss**, and brands once you own them, read from
  your save's own progress. NPCs show only where they currently are. Sanctuaries are named
  after their area and the creed that holds them in your save.
- **Filters** on the map page: all icons, items, bosses, NPCs, sanctuaries, enemies, or
  just the map.
- **Enemies** near you as red dots, bosses ringed. Hover one on the map page for its name
  and health.
- **Your marker** is a magenta diamond; a co-op partner shows in blue.
- **Zoomed far out**, only the objectives' icons stay, so the whole map reads clearly.
- **Where next:** the next objective on the main route (after the Fextralife wiki's Game
  Progress Route) gets a gold ring on the map, and an arrow at the minimap's edge when it is
  out of view. Optional bosses, NPC stories, notable items and hidden sanctuaries get silver
  rings once they open up. Can be set to main route only, or off.
- **Map page extras:** area names, and completion counts for items, bosses and sanctuaries.
- Keyboard, mouse and **controller**.

The map pictures and icon positions come from the community **SSMap** web map; the
installer downloads it from its own repository (see [Map data](#map-data)).

## Install

Download `SaltMap-<version>.zip` from the [Releases](../../releases) page, extract it,
close the game and double-click **`Install.cmd`**. That's it: start the game.

The installer finds the game through Steam, patches `salt.exe` once (the original is kept
as `salt.exe.orig`), copies the mod (including translations) into the game's `Mods` folder, 
and on the first install downloads the map data (about 120 MB) into `Mods\SSMap`. Options, from PowerShell
or after `Install.cmd`:

```powershell
install.ps1 -GameDir "D:\Games\Salt and Sanctuary"   # if the game is not found
install.ps1 -SsMap "D:\SSMap"                          # use a copy of SSMap you already have
install.ps1 -NoMapDownload                             # skip the download
```

The mod writes `Mods\SaltMap.log` in the game folder. After a game update, run
`Install.cmd` again: it patches the new `salt.exe`.

**Uninstall:** run `uninstall.ps1` from the same zip (it puts the original `salt.exe`
back), or use Steam's "Verify integrity of game files" and delete the `Mods` folder.

## Map data

SaltMap draws the [SSMap](https://kaszub09.github.io/SSMap/) interactive map, made by
**Kaszub09** and contributors from the screenshot map of the whole world assembled by
**[u/magicofgames](https://www.reddit.com/r/saltandsanctuary/comments/f0t1sa/ultimate_map_still_wip_well_no_but_actually_yes/)**.
Its tiles, marker list and icons are read from a local copy at run time and are not part
of this repository or its releases: SSMap has no licence, so SaltMap does not redistribute
it. The installer fetches it straight from
[github.com/Kaszub09/SSMap](https://github.com/Kaszub09/SSMap), pinned to the version
SaltMap was tested with, and keeps `tiles\`, `markers.js` and `images\markerIcons\` in
`Mods\SSMap` (with a `SOURCE.txt` saying where it came from). To use a copy elsewhere,
set `ssmap=` in `Mods\SaltMap.ini` or pass `-SsMap`. Keep it for personal use.

## Controls

*Note: Default hotkeys are listed below. You can fully remap the mod's controls in `Mods/SaltMapControls.json`.*

| | Keyboard / mouse | Controller |
|---|---|---|
| Show / hide the minimap | M | Select |
| Zoom the minimap | `=` and `-` | RB / LB |
| Open the map page | Esc, then the map icon | Back / Select (in play or in the Esc menu) |
| Pan | Arrows / WASD / drag | Left stick |
| Zoom | Q E, mouse wheel | Right stick (smooth); LB RB, LT RT |
| Change what is shown | F (Shift+F back) | D-pad left / right |
| Guide: all, main only, off | G | Y |
| Centre on yourself | Space | A |
| Back to the menu | Esc | B |
| Close the menu | | Back / Select |

## Settings

`Mods\SaltMap.ini` is created on first start:

| Key | Meaning |
|---|---|
| `ssmap` | Folder of the SSMap copy; relative paths count from `Mods`. Default `SSMap`. |
| `minimap` | `on` or `off` (also toggled in game). |
| `minimapZoom` | 0 (far) to 7 (near) (also changed in game). |
| `minimapOpacity` | 0.1 to 1.0 (semi-transparent minimap). |
| `mapFilter` | What the map page shows, 0 (all) to 6 (map only) (also changed in game). |
| `guide` | `all`, `main` or `off` (also changed in game). |
| `modLang` | Force language (en, ru). Leave empty to auto-detect from game. |
| `fogOfWar` | `on` or `off`. |
| `fogRadius` | How far the fog is revealed around the player (in map pixels). |
| `fogBossRadius` | How far the fog is revealed when a boss is defeated. |
| `udpTracker` | `on` or `off`. Sends player coords to `localhost:47800` for external map trackers. |

## How it works

- `Patcher/` builds `SaltPatcher.exe`, which adds one guarded call to the start of
  `Game1.Initialize` in `salt.exe` that loads `Mods\SaltMap.dll`. If the mod is missing or
  fails, the game starts anyway and writes `SaltMap-loader-error.log` in the game folder.
- `Mod/` builds `SaltMap.dll`, which hooks the game at run time with
  [Harmony](https://github.com/pardeike/Harmony). Nothing else in the game is changed.
- Game positions are matched to SSMap's map by fitting the level's collision geometry to
  the map pictures; icons are tied to the chests, bags and bosses they stand for to know
  when they are done. Details in [docs/minimap.md](docs/minimap.md) and
  [docs/rendering.md](docs/rendering.md).
- The mod can optionally send the player position to UDP port 47800 on this computer, for an external live web-map tracker. This feature is disabled by default and can be enabled via `udpTracker=on` in `SaltMap.ini`. Nothing leaves the machine.
- Details about the new fork features (Fog of War, Localization, etc.) can be found in [docs/custom_features.md](docs/custom_features.md).

## Building from source

**Option 1: Visual Studio**
Open `SaltMap.slnx` (or `Mod/SaltMap.csproj`) in Visual Studio. Ensure `GameDir` is set in the project properties. Build in `Release` mode. The build script will automatically patch the game, copy the files, and create a release zip in the `dist/` folder.

**Option 2: CLI (Without Visual Studio)**
Needs Windows, the [.NET SDK](https://dotnet.microsoft.com/download) and the game. Close the game first.

The easiest way is to just double-click **`Build.cmd`** in the project root. It will automatically find the game through Steam, build the mod, install it, and create a release zip in the `dist/` folder.

If you prefer the terminal, you can run the PowerShell script directly:
```powershell
powershell -ExecutionPolicy Bypass -File build.ps1            # finds the game through Steam
powershell -ExecutionPolicy Bypass -File build.ps1 "D:\Games\Salt and Sanctuary"
```

Both methods use the same MSBuild targets (`Mod/tasks.targets`) to patch the game, build the mod, and create the release zip.

With the game installed the mod compiles against the real `salt.exe`. Without it (as on GitHub Actions) it compiles against `Reference/salt`, a stub that declares only the game members the mod uses (names and signatures, no game code), and MonoGame's NuGet package. After using new game members, update the stub and run `scripts\Check-Stubs.ps1`, which checks every reference and constant against the installed game.

Releases: push a tag `v<version>` matching `<Version>` in `Mod/SaltMap.csproj`; the
[release workflow](.github/workflows/release.yml) builds the zip and attaches it.

## A note on language

Look, the comments in the code, the `docs/` folder, and commit messages are in **Russian**. 

I know open-source is supposed to be in English, but honestly? I just couldn't be bothered. I did this for myself, in the language I actually think in, and I'm not trying to win a "good boy" award for following all the conventions. The actual code (variables, classes, methods) is still in English, so it won't break anything.

If you really want to read the comments, just throw them into a translator or use the "Comment Translate" extension in VS Code. It's not that hard. 

## Credits

- Original SaltMap Project by [Buzzed-Lightyaw](https://github.com/Buzzed-Lightyaw/SaltMap)
- [SSMap](https://github.com/Kaszub09/SSMap) by Kaszub09 and contributors: map tiles, markers and icons.
- The progression route: the [Fextralife Salt and Sanctuary wiki](https://saltandsanctuary.wiki.fextralife.com/Game_Progress_Route).
- The original screenshot map of the world by
  [u/magicofgames](https://www.reddit.com/r/saltandsanctuary/comments/f0t1sa/ultimate_map_still_wip_well_no_but_actually_yes/).
- [Harmony](https://github.com/pardeike/Harmony) (MIT) and
  [Mono.Cecil](https://github.com/jbevain/cecil) (MIT), included in the release zip.
- [MonoGame](https://github.com/MonoGame/MonoGame) (MIT), used by the game.

SaltMap is a fan project, not affiliated with Ska Studios. It contains no game files.

## License

SaltMap's own code is [MIT licensed](LICENSE). The SSMap map data it downloads is not
covered by this licence.
