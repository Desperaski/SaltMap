# Removes SaltMap: puts the original salt.exe back and deletes the mod's files from Mods.
# Your SaltMap.ini, the log and any SSMap folder in Mods are left alone.
param([string]$GameDir)

$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
Get-ChildItem -LiteralPath $here -Recurse -File | Unblock-File
. ([IO.Path]::Combine($here, 'Find-GameDir.ps1'))
$game = Find-GameDir $GameDir
if (-not $game) { throw "Salt and Sanctuary not found. Run: uninstall.ps1 -GameDir `"D:\path\to\Salt and Sanctuary`"" }
if (Get-Process salt -ErrorAction SilentlyContinue) { throw "Close Salt and Sanctuary first." }

# 1. Восстанавливаем оригинальный EXE
Write-Host "Restoring original salt.exe..."
& ([IO.Path]::Combine($here, 'Patcher', 'SaltPatcher.exe')) $game --restore
if ($LASTEXITCODE) { 
    Write-Warning "Could not restore salt.exe; use Steam's 'Verify integrity of game files' instead." 
} else {
    Write-Host "  salt.exe successfully restored from salt.exe.orig."
}

# 2. Удаляем ТОЛЬКО исполняемые файлы мода
$mods = [IO.Path]::Combine($game, 'Mods')
$filesToDelete = @('SaltMap.dll', 'SaltMap.pdb', '0Harmony.dll')
$deletedFiles = 0

foreach ($f in $filesToDelete) {
    $p = [IO.Path]::Combine($mods, $f)
    if ([IO.File]::Exists($p)) { 
        Remove-Item -LiteralPath $p -Force 
        $deletedFiles++
    }
}

Write-Host ""
Write-Host "SaltMap mod code removed ($deletedFiles files)."
Write-Host "Your user data was KEPT intact:"
Write-Host "  - Mods\SSMap (map data)"
Write-Host "  - Mods\SaltMap.ini (settings)"
Write-Host "  - Mods\SaltMapControls.json (keybinds)"
Write-Host "  - Mods\lang\ (translations)"
Write-Host "  - Mods\fog_*.dat (fog of war progress)"
Write-Host "  - Mods\SaltMap.log (log)"
Write-Host ""
Write-Host "Done."