param(
  [int]$Port = 4723
)

$ErrorActionPreference = "Stop"

try { $Port = [int]$Port } catch { throw "APPIUM Port must be numeric. Got: '$Port'" }

# Kill if already listening
$inUse = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
if ($inUse) {
  Write-Host "Port $Port in use. Attempting to free it..."
  $inUse | ForEach-Object { taskkill /PID $_.OwningProcess /F 2>$null | Out-Null }
  Start-Sleep -Seconds 2
}

$job = Start-Job -ScriptBlock {
  param($p)
  npx appium --base-path / --port $p
} -ArgumentList $Port

# Wait for port
$deadline = (Get-Date).AddSeconds(20)
do {
  Start-Sleep -Seconds 1
  $up = (Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue)
} while (-not $up -and (Get-Date) -lt $deadline)

if (-not $up) { 
  Stop-Job -Job $job -ErrorAction SilentlyContinue
  Remove-Job -Job $job -ErrorAction SilentlyContinue
  throw "Appium did not start on port $Port." 
}
Write-Host "Appium is up."