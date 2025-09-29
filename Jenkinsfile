pipeline {
  agent { label 'windows-android' }

  options {
    timestamps()
    ansiColor('xterm')
    // Prevent infinite hangs
    timeout(time: 25, unit: 'MINUTES')
  }

  environment {
    APPIUM_PORT = '4723'
    APPIUM_LOG  = 'appium.out.log'
    // Point tests to the local Appium; adjust if your tests read a different var name
    APPIUM_SERVER_URL = "http://127.0.0.1:${APPIUM_PORT}/"
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
        echo 'Checkout successfully.'
      }
    }

    stage('Restore & Build') {
      steps {
        powershell '''
          $ErrorActionPreference = "Stop"
          dotnet --info
          dotnet restore .
          dotnet build . -c Release
        '''
      }
    }

    stage('Ensure Emulator Device Online') {
      steps {
        powershell '''
          $ErrorActionPreference = "Stop"

          $adb = Join-Path $env:ANDROID_HOME "platform-tools\\adb.exe"
          if (!(Test-Path $adb)) { throw "adb.exe not found at $adb. Ensure ANDROID_HOME is set correctly." }

          & $adb start-server | Out-Null

          $deadline = (Get-Date).AddMinutes(3)
          Write-Host "Waiting for an online emulator device..."

          do {
            Start-Sleep -Seconds 2
            $devices = & $adb devices
            # Look for any emulator-* in "device" state
            $match = $devices | Select-String -Pattern "^emulator-\\d+\\s+device"
          } while (-not $match -and (Get-Date) -lt $deadline)

          if (-not $match) {
            Write-Host $devices
            throw "No online emulator device found. Ensure your emulator is running on the host."
          }

          $serial = ($match.Matches[0].Value -split '\\s+')[0]
          Write-Host "Using emulator: $serial"

          # Double-check boot completed
          $deadline = (Get-Date).AddMinutes(2)
          do {
            $state = (& $adb -s $serial get-state) 2>$null
            if ($state -match "device") {
              $prop = (& $adb -s $serial shell getprop sys.boot_completed).Trim()
              if ($prop -eq "1") { break }
            }
            Start-Sleep -Seconds 3
          } while ((Get-Date) -lt $deadline)

          if ($prop -ne "1") {
            throw "Emulator $serial did not finish booting."
          }

          # Unlock (harmless if already unlocked)
          & $adb -s $serial shell input keyevent 82 | Out-Null

          # Export serial to a file so the next step can inject it as env var
          Set-Content -Path "adb-serial.txt" -Value $serial -Encoding ASCII
        '''
        script {
          env.ADB_SERIAL = readFile('adb-serial.txt').trim()
          echo "ADB_SERIAL=${env.ADB_SERIAL}"
        }
      }
    }

    stage('Ensure Appium Server') {
      steps {
        powershell '''
          $ErrorActionPreference = "Stop"
          $port = [int]$env:APPIUM_PORT

          # Is Appium already listening?
          $listening = Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue
          if ($listening) {
            Write-Host "Appium already listening on :$port"
            return
          }

          Write-Host "Starting Appium on :$port ..."
          $workspace = $env:WORKSPACE
          if ([string]::IsNullOrWhiteSpace($workspace)) { $workspace = (Get-Location).Path }

          $outLog = Join-Path $workspace $env:APPIUM_LOG
          $errLog = Join-Path $workspace "appium.err.log"

          $args = @("/c","npx","appium","--base-path","/","--port",$port)
          Start-Process -FilePath "cmd.exe" -ArgumentList $args -WindowStyle Hidden `
            -RedirectStandardOutput $outLog -RedirectStandardError $errLog | Out-Null

          # Wait for the port to open
          $deadline = (Get-Date).AddSeconds(30)
          do {
            Start-Sleep -Seconds 1
            $up = Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue
          } while (-not $up -and (Get-Date) -lt $deadline)

          if (-not $up) {
            if (Test-Path $errLog) { Get-Content $errLog -Tail 80 | ForEach-Object { "APPIUM ERR: $_" } }
            throw "Appium did not start on port $port"
          }

          Write-Host "Appium is up on :$port"
        '''
      }
    }

    stage('Run Tests') {
      steps {
        // If your tests read different env names, adjust them here
        withEnv([
          "ADB_SERIAL=${env.ADB_SERIAL}",
          "APPIUM_SERVER_URL=${env.APPIUM_SERVER_URL}"
        ]) {
          powershell '''
            $ErrorActionPreference = "Stop"

            Write-Host "Verifying preconditions..."
            # Quick checks before running tests
            $port = [int]$env:APPIUM_PORT
            $listening = Get-NetTCPConnection -State Listen -LocalPort $port -ErrorAction SilentlyContinue
            if (-not $listening) { throw "Appium is not listening on :$port" }

            $adb = Join-Path $env:ANDROID_HOME "platform-tools\\adb.exe"
            if (!(Test-Path $adb)) { throw "adb.exe not found for test stage." }

            $serial = $env:ADB_SERIAL
            if ([string]::IsNullOrWhiteSpace($serial)) { throw "ADB_SERIAL not set." }
            $state = (& $adb -s $serial get-state).Trim()
            if ($state -ne "device") { throw "Target $serial is not in 'device' state (got '$state')." }

            Write-Host "Running tests against $serial via $env:APPIUM_SERVER_URL ..."
            # Adjust the path if your solution/test project lives elsewhere
            dotnet test -c Release --no-build --logger "trx;LogFileName=TestResults.trx" --filter "LongPressMenuTest" -- NUnit.NumberOfTestWorkers=1
          '''
        }
      }
    }
  }

  post {
    always {
      // Collect useful logs/results without failing the build if missing
      archiveArtifacts artifacts: '**/TestResults/**/*.trx, **/TestResult*.xml, appium*.log, **/appium*.log', allowEmptyArchive: true
      echo "Build done."
    }
  }
}
