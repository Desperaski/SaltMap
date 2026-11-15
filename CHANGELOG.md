# Changelog

Each release's section below is also its GitHub release notes.

## 0.5.0 (Enhanced Fork)

This version includes major additions and QoL improvements made by the fork maintainer.

### New Features
- Fog of War: Map starts hidden and is revealed as you explore. Features smooth gradient edges, large reveals on boss kills, and separate save data for each character slot (`fog_<slot>.dat`).
- Full Localization: The mod's UI (filters, hints, guide) now auto-syncs with the game's language (ID 9 = Russian) via a Harmony hook. Includes translated boss names and region names.
- JSON Hotkeys: All mod hotkeys are moved to `Mods/SaltMapControls.json`, supporting both keyboard and gamepad.
- Minimap Opacity: Added `minimapOpacity` setting in `.ini`.
- UDP Tracker: Restored the original UDP position tracker (for external map apps) and moved it to an opt-in setting (`udpTracker=off` by default).

### UI Fixes & Improvements
- Gamepad Icons: Fixed missing stick and D-pad icons in the map menu hints by splitting hints into two lines and using the native `replaceIcons = 2` render mode.
- Menu Speedup: Removed the sluggish ~1 second input delay when opening menus (`MenuSpeedup.cs`).
- Larger Map Area: Reduced UI padding to give the map more screen space.
- Two-line Hints: Control hints are split into two lines to prevent text overflow in Russian.

### Build & Installer
- New Build Pipeline: `tasks.targets` and `SaltMap.csproj` rewritten for automatic copying, patching, and ZIP creation.
- Build.cmd: Added a one-click build script for users without Visual Studio.
- Installer Fixes: `install.ps1` now correctly copies the `lang` folder. `uninstall.ps1` safely removes only mod files while keeping user data (settings, map tiles, fog progress, translations).
- Progress Bar: Fixed the map download progress bar to use native `Write-Progress` so it doesn't break when resizing the console window.

## 0.4.4

Everything since 0.2.1 (0.3.0 and the 0.4.x steps were not released separately).

### Where next
- A guide to the next objective, after the Fextralife wiki's Game Progress Route: the next
  main-route boss or brand gets a pulsing gold ring on the map, and an arrow at the edge of
  the minimap or map page when it is out of view.
- Optional objectives once they open up: Ronin Cran, Carsejaw the Cruel, The Forgotten King;
  the Masterless Knight's, Despondent Thief's and Black Sands Sorcerer's stories (pointing at
  where the NPC stands now); the Pitchfork, Stone Sellsword, Kureimoa, Haymaker, Vile Vines
  Ring and Bag of Earth; the House of Splendor and Order of the Betrayer sanctuaries. They
  get pale blue rings.
- Progress is read from your save, so the guide starts where you are. The prologue (the boat
  and the Unspeakable Deep) is left out.
- G (keyboard) or Y (controller) on the map page cycles: main and optional, main only, off.

### Map page
- A panel under the menu with completion counts (items, bosses, claimed sanctuaries), the
  next objective and the open optional ones.
- Area names on the map.
- D-pad left/right (or F, Shift+F) picks what to show: all, items, bosses, NPCs,
  sanctuaries, enemies, or just the map. The game's own prompts are replaced by the map's
  controls, so there is only one "B" prompt.
- Hover an enemy for its name, health and whether it has noticed you; hover a ring for its
  objective.

### Icons
- NPCs show only where they stand now in your save (they move as the story goes on).
- Sanctuaries are named after their area and the creed that holds them in your save,
  instead of "Empty Sanctuary", with a creed icon once claimed.
- Brands disappear once you own them; items NPCs hand over are tracked too.
- Enemies near you are red dots, bosses ringed; zoomed far out (the three widest levels)
  only the objectives' icons stay.
- Your marker is a magenta diamond; a co-op partner shows in blue.
- SSMap's boat markers, which sat off the edge of the map, are gone.

### Diagnostics
- Opening the map page writes `Mods\marker-status.tsv`, `Mods\save-flags.txt` and
  `Mods\guide-status.txt`, for tracing an icon or objective that looks wrong.

## 0.2.1

- While the map page is open, the bar under the Escape menu shows the map's controls
  instead of the game's own prompts, so there is only one "B" prompt.
- Installer: deleting the temporary map download can no longer fail the install (it
  ended with a yellow warning before); download and extraction failures are reported
  separately; extraction shows progress; folders with `[ ]` in their names work.

## 0.2.0

First release.

- Minimap in the top-right corner, inside the HUD's corner ornament, from the SSMap web
  map's colour tiles.
- A Map page in the Escape menu, with pan and zoom.
- Item, NPC, boss and sanctuary icons; item and boss icons disappear once collected or
  beaten.
- Enemy dots.
- Keyboard, mouse and controller (Back/Select opens the map page).
- One-step installer that downloads the map data from the SSMap project.
