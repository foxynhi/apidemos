param(
  [int]$Port = 4723
)

$ErrorActionPreference = "Stop"

try { $Port = [int]$Port } catch { throw "APPIUM Port must be numeric. Got: '$Port'" }


# Start Appium in background
$cmd = "cmd.exe"
$args = @("/c", "npx", "appium", "--base-path", "/", "--port", $Port.ToString())

# Kill if already listening
$inUse = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
if ($inUse) {
  Write-Host "Port $Port in use. Attempting to free it..."
  $inUse | ForEach-Object { taskkill /PID $_.OwningProcess /F 2>$null | Out-Null }
  Start-Sleep -Seconds 2
}

Write-Host "Starting Appium on port $Port with command: $cmd $($args -join ' ')"
Start-Process -FilePath $cmd -ArgumentList $args -NoNewWindow
Write-Host "Appium start command issued..."

# Wait for port
$deadline = (Get-Date).AddSeconds(20)
do {
  Start-Sleep -Seconds 1
  $up = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
} while (-not $up -and (Get-Date) -lt $deadline)

if (-not $up) { throw "Appium did not start on port $Port." }
Write-Host "Appium is up."