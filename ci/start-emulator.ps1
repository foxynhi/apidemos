param(
  [string]$AvdName = "Pixel_9",
  [int]$BootTimeoutSec = 180
)

$ErrorActionPreference = "Stop"

# Resolve SDK tools
$androidHome = $env:ANDROID_HOME
if ([string]::IsNullOrWhiteSpace($androidHome)) { $androidHome = $env:ANDROID_HOME }
if ([string]::IsNullOrWhiteSpace($androidHome)) { throw "ANDROID_HOME is not set." }

$emulatorExe = Join-Path $androidHome "emulator\emulator.exe"
$adbExe      = Join-Path $androidHome "platform-tools\adb.exe"

if (!(Test-Path $emulatorExe)) { throw "emulator.exe not found at $emulatorExe" }
if (!(Test-Path $adbExe)) { throw "adb.exe not found at $adbExe" }

# Kill any leftovers
Get-Process -Name "qemu-system-x86_64" -ErrorAction SilentlyContinue |
  Stop-Process -Force -ErrorAction SilentlyContinue
& $adbExe kill-server 2>$null | Out-Null
Start-Sleep -Seconds 1

# Start emulator headless & faster boot flags
$emuArgs = @("-avd", $AvdName, "-no-snapshot", "-no-boot-anim", "-no-audio", "-gpu", "off")
Start-Process -FilePath $emulatorExe -ArgumentList $emuArgs -WindowStyle Hidden

# Wait for device to be ready
& $adbExe start-server | Out-Null
$deadline = (Get-Date).AddSeconds($BootTimeoutSec)
Write-Host "Waiting for emulator to boot..."

& $adbExe wait-for-device

do {
  $prop = & $adbExe shell getprop sys.boot_completed
  if ($prop -match "1") { break }
  Start-Sleep -Seconds 3
} while ((Get-Date) -lt $deadline)

if ($prop -notmatch "1") {
  throw "Emulator failed to boot within $BootTimeoutSec seconds."
}

# Unlock (some images need it)
& $adbExe shell input keyevent 82
Write-Host "Emulator is ready."
