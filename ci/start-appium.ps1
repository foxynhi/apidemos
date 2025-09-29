param(
  [int]$Port = 4723
)

$ErrorActionPreference = "Stop"

try { $Port = [int]$Port } catch { throw "APPIUM Port must be numeric. Got: '$Port'" }


# Start Appium in background
$AppiumCmd = "C:/Users/khanh/AppData/Roaming/npm/appium.cmd"
$NpxCmd    = "C:/Program Files/nodejs/npx.cmd"
if (-not $AppiumCmd -and -not $NpxCmd) {
  throw "Could not find appium.cmd or npx.cmd on PATH."
}


# Kill if already listening
$inUse = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
if ($inUse) {
  Write-Host "Port $Port in use. Attempting to free it..."
  $inUse | ForEach-Object { taskkill /PID $_.OwningProcess /F 2>$null | Out-Null }
  Start-Sleep -Seconds 2
}

$log = 'C:/jenkins-agent/Appium.log'
if (Test-Path $log) { Remove-Item $log -Force -ErrorAction SilentlyContinue }

if ($AppiumCmd) {
  $cmd = $AppiumCmd
  $args = @("/c","start","""AppiumServer""","""$AppiumCmd"" --base-path / --port $Port --log `"$log`"") `
} else {
  
  $cmd = $NpxCmd
  $args = @("/c","start","""AppiumServer""","""$NpxCmd"" appium --base-path / --port $Port --log `"$log`"") `
}

Write-Host "Starting Appium on port $Port with command: $cmd $($args -join ' ')"
#Start-Process -FilePath $cmd -ArgumentList $args -NoNewWindow
#Write-Host "Appium start command issued..."

# Wait for port
$deadline = (Get-Date).AddSeconds(20)
$ready = $false
do {
  Start-Sleep -Seconds 1
  try {
    $r = Invoke-WebRequest -UseBasicParsing "http://127.0.0.1:$Port/status" -TimeoutSec 2
    if ($r.StatusCode -in 200,304) { $ready = $true }
  } catch { }
} while (-not $ready -and (Get-Date) -lt $deadline)
if (-not $ready) { throw "Appium did not report ready on http://127.0.0.1:$Port/status. See $log" }
Write-Host "Appium is up. Logs: $log"