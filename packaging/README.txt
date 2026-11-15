SaltMap - minimap and map page for Salt and Sanctuary (Windows, Steam)

Installation Instructions
=====================================

Install
  1. Make sure Salt and Sanctuary is CLOSED.
  2. Double-click "install.cmd".
  3. Wait for the installer to finish (it will download the map data, which takes a few minutes).
  4. Launch the game.


Uninstall
  1. Make sure the game is CLOSED.
  2. Double-click "uninstall.cmd".
    This will restore the original salt.exe and remove the mod's core files.
    Your settings (SaltMap.ini), map data (SSMap), and fog of war progress will be kept safely.

Troubleshooting:
----------------
If the installer cannot find your game folder, open PowerShell in this folder and run:
  powershell -ExecutionPolicy Bypass -File install.ps1 -GameDir "D:\Path\To\Salt and Sanctuary"

  Options (run from PowerShell, or add them after Install.cmd):
    install.ps1 -GameDir "D:\...\Salt and Sanctuary"   if the game is not found
    install.ps1 -SsMap "D:\SSMap"                       use a copy of SSMap you already have
    install.ps1 -NoMapDownload                          skip the download

  The log is <game folder>\Mods\SaltMap.log.

If the map download fails, you can manually download the map 
from https://github.com/Kaszub09/SSMap, extract it, and place the "tiles", "markers.js", 
and "images" folders into: Mods\SSMap\

After a game update, run Install.cmd again: it re-patches the new salt.exe.

Controls, settings and details: see the project README on GitHub.

Map credits: the SSMap interactive map (https://github.com/Kaszub09/SSMap) by Kaszub09 and
contributors, made from the screenshot map of the world by u/magicofgames
(https://www.reddit.com/r/saltandsanctuary/comments/f0t1sa/). SSMap is not part of this
package; the installer downloads it from its own repository.

SaltMap is MIT licensed (LICENSE). Third-party parts in this package: Harmony
(0Harmony.dll, MIT licence, https://github.com/pardeike/Harmony) and Mono.Cecil
(Patcher\Mono.Cecil*.dll, MIT licence, https://github.com/jbevain/cecil).
