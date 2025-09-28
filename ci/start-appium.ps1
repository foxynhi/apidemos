param(
  [int]$Port = 4723
)

$ErrorActionPreference = "Stop"

try { $Port = [int]$Port } catch { throw "APPIUM Port must be numeric. Got: '$Port'" }

# Start Appium in background
$cmd = "npx"
$args = @("appium", "--base-path", "--port", $Port.ToString())

# Kill if already listening
$inUse = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
if ($inUse) {
  Write-Host "Port $Port in use. Attempting to free it..."
  $inUse | ForEach-Object { taskkill /PID $_.OwningProcess /F 2>$null | Out-Null }
  Start-Sleep -Seconds 2
}

Start-Process -FilePath $cmd -ArgumentList $args -NoNewWindow
Write-Host "Starting Appium on port $Port..."

# Wait for port
$deadline = (Get-Date).AddSeconds(30)
do {
  Start-Sleep -Seconds 1
  $up = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
} while (-not $up -and (Get-Date) -lt $deadline)

if (-not $up) { throw "Appium did not start on port $Port." }
Write-Host "Appium is up."
param(
  [int]$Port = 4723
)

$ErrorActionPreference = "Stop"

# Start Appium in background
$cmd = "npx"
$args = @("appium", "--base-path", "--port", $Port.ToString())

# Kill if already listening
$inUse = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
if ($inUse) {
  Write-Host "Port $Port in use. Attempting to free it..."
  $inUse | ForEach-Object { taskkill /PID $_.OwningProcess /F 2>$null | Out-Null }
  Start-Sleep -Seconds 2
}

Start-Process -FilePath $cmd -ArgumentList $args -NoNewWindow
Write-Host "Starting Appium on port $Port..."

# Wait for port
$deadline = (Get-Date).AddSeconds(30)
do {
  Start-Sleep -Seconds 1
  $up = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
} while (-not $up -and (Get-Date) -lt $deadline)

if (-not $up) { throw "Appium did not start on port $Port." }
Write-Host "Appium is up."
