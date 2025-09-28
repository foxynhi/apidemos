# Stop Appium
Get-Process node -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*\node.exe" } | Stop-Process -Force -ErrorAction SilentlyContinue
# Stop emulator
taskkill /IM "qemu-system-x86_64.exe" /F 2>$null | Out-Null
adb kill-server 2>$null | Out-Null
Write-Host "Stopped Appium and emulator."
