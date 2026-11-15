# Builds the mod into the game's Mods folder and creates the release ZIP.
# Usage:  .\build.ps1
#         .\build.ps1 "D:\Games\Salt and Sanctuary"
param([string]$GameDir)

$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\scripts\Find-GameDir.ps1"
$found = Find-GameDir $GameDir
if (-not $found) {
    Write-Error "Salt and Sanctuary not found. Pass its folder: .\build.ps1 `"D:\path\to\Salt and Sanctuary`" (or set SALT_DIR)."
}
$env:SALT_DIR = $found
Write-Host "Game folder: $found"

# Вызываем сборку основного проекта. 
# tasks.targets внутри сам запустит патчер, скопирует файлы в игру и создаст ZIP.
dotnet build "$PSScriptRoot\Mod\SaltMap.csproj" -c Release --nologo
if ($LASTEXITCODE) { exit $LASTEXITCODE }

Write-Host "Done. Mod installed and release ZIP created in dist\"