pipeline {
  agent { label 'windows-android' }
  
  options {
    timestamps()
    ansiColor('xterm')
    durabilityHint('PERFORMANCE_OPTIMIZED')
  }
  
  environment {
    ANDROID_HOME     = "${env.ANDROID_HOME}"
    JAVA_HOME        = "${env.JAVA_HOME}"
    PATH             = "${env.PATH};C:\\Program Files\\dotnet;${env.ANDROID_HOME}\\platform-tools"
    AVD_NAME         = "Pixel_9"
    APPIUM_PORT      = "4723"
  }
  
  stages {
    stage('Checkout') {
      steps {
        checkout scm
        echo "Checkout successfully."
      }
    }
    
    stage('Restore & Build') {
      steps {
        powershell '''
          dotnet --info
          dotnet restore
          dotnet build --configuration Release --no-restore
        '''
      }
    }
    
    stage('Start Emulator & Appium & Run Tests') {
      steps {
        powershell '''
          $ErrorActionPreference = "Stop"
          
          # ============================================
          # START EMULATOR
          # ============================================
          Write-Host "=== Starting Android Emulator ==="
          
          $androidHome = $env:ANDROID_HOME
          if ([string]::IsNullOrWhiteSpace($androidHome)) { 
            throw "ANDROID_HOME is not set." 
          }
          
          $emulatorExe = Join-Path $androidHome "emulator\\emulator.exe"
          $adbExe      = Join-Path $androidHome "platform-tools\\adb.exe"
          
          if (!(Test-Path $emulatorExe)) { 
            throw "emulator.exe not found at $emulatorExe" 
          }
          if (!(Test-Path $adbExe)) { 
            throw "adb.exe not found at $adbExe" 
          }
          
          # Kill any leftovers
          Write-Host "Cleaning up existing processes..."
          Get-Process -Name "qemu-system-x86_64" -ErrorAction SilentlyContinue | 
            Stop-Process -Force -ErrorAction SilentlyContinue
          & $adbExe kill-server 2>$null | Out-Null
          Start-Sleep -Seconds 2
          
          # Start emulator
          Write-Host "Starting emulator: $env:AVD_NAME"
          $emuArgs = @("-avd", $env:AVD_NAME, "-no-snapshot", "-no-boot-anim", "-no-audio", "-gpu", "off")
          Start-Process -FilePath $emulatorExe -ArgumentList $emuArgs -WindowStyle Hidden
          
          # Wait for device to be ready
          & $adbExe start-server | Out-Null
          $bootTimeout = 240
          $deadline = (Get-Date).AddSeconds($bootTimeout)
          
          Write-Host "Waiting for emulator to boot (timeout: ${bootTimeout}s)..."
          & $adbExe wait-for-device
          
          do {
            $prop = & $adbExe shell getprop sys.boot_completed 2>$null
            if ($prop -match "1") { 
              Write-Host "Boot completed!"
              break 
            }
            Write-Host "Still booting... (sys.boot_completed: $prop)"
            Start-Sleep -Seconds 3
          } while ((Get-Date) -lt $deadline)
          
          if ($prop -notmatch "1") {
            throw "Emulator failed to boot within $bootTimeout seconds."
          }
          
          # Unlock screen
          & $adbExe shell input keyevent 82
          Write-Host "Emulator is ready."
          
          # ============================================
          # START APPIUM
          # ============================================
          Write-Host "`n=== Starting Appium Server ==="
          
          $appiumPort = [int]$env:APPIUM_PORT
          
          # Kill any process using the port
          $inUse = Get-NetTCPConnection -LocalPort $appiumPort -ErrorAction SilentlyContinue
          if ($inUse) {
            Write-Host "Port $appiumPort in use. Killing process..."
            $inUse | ForEach-Object { 
              taskkill /PID $_.OwningProcess /F 2>$null | Out-Null 
            }
            Start-Sleep -Seconds 2
          }
          
          # Start Appium as background job
          Write-Host "Starting Appium on port $appiumPort..."
          $appiumJob = Start-Job -ScriptBlock {
            param($port)
            npx appium --base-path / --port $port 2>&1
          } -ArgumentList $appiumPort
          
          # Wait for Appium to be ready
          $appiumDeadline = (Get-Date).AddSeconds(30)
          $appiumReady = $false
          
          do {
            Start-Sleep -Seconds 2
            $listening = Get-NetTCPConnection -LocalPort $appiumPort -ErrorAction SilentlyContinue
            if ($listening) {
              $appiumReady = $true
              break
            }
            Write-Host "Waiting for Appium to start..."
          } while ((Get-Date) -lt $appiumDeadline)
          
          if (-not $appiumReady) {
            Write-Host "Appium job output:"
            Receive-Job -Job $appiumJob
            Stop-Job -Job $appiumJob -ErrorAction SilentlyContinue
            Remove-Job -Job $appiumJob -ErrorAction SilentlyContinue
            throw "Appium did not start on port $appiumPort within 30 seconds."
          }
          
          Write-Host "Appium is up and running (Job ID: $($appiumJob.Id))"
          
          # ============================================
          # RUN TESTS
          # ============================================
          Write-Host "`n=== Running Tests ==="
          
          $env:APPIUM_SERVER_URL = "http://127.0.0.1:${appiumPort}"
          Write-Host "APPIUM_SERVER_URL = $env:APPIUM_SERVER_URL"
          Write-Host "Current directory: $(Get-Location)"
          
          # Navigate to test directory
          $testPath = Join-Path (Get-Location) "Auto Test\\APIDemos"
          if (!(Test-Path $testPath)) {
            throw "Test directory not found: $testPath"
          }
          
          Set-Location -Path $testPath
          Write-Host "Changed to: $(Get-Location)"
          Write-Host "`nTest directory contents:"
          Get-ChildItem | Format-Table Name, Length
          
          # Run tests
          Write-Host "`nExecuting dotnet test..."
          dotnet test --configuration Release `
            --filter "LongPressMenuTest" `
            --no-build `
            --logger "nunit;LogFilePath=TestResults/TestResult.xml" `
            -- NUnit.NumberOfTestWorkers=1
          
          Write-Host "`nTests completed!"
        '''
      }
      post {
        always {
          script {
            // Change back to workspace root for artifacts
            dir("${env.WORKSPACE}") {
              junit allowEmptyResults: true, testResults: '**/TestResults/TestResult.xml'
              
              publishHTML(target: [
                reportDir: 'Auto Test/APIDemos/TestResults',
                reportFiles: '**/index.html',
                reportName: 'Extent Report',
                keepAll: true,
                alwaysLinkToLastBuild: true,
                allowMissing: true
              ])
              
              archiveArtifacts artifacts: '**/TestResults/**', 
                               fingerprint: true, 
                               onlyIfSuccessful: false,
                               allowEmptyArchive: true
            }
          }
        }
      }
    }
  }
  
  post {
    always {
      powershell '''
        Write-Host "`n=== Cleanup: Stopping Appium and Emulator ==="
        
        # Stop Appium (all node processes)
        Get-Process node -ErrorAction SilentlyContinue | 
          Where-Object { $_.Path -like "*\\node.exe" } | 
          Stop-Process -Force -ErrorAction SilentlyContinue
        
        # Stop emulator
        taskkill /IM "qemu-system-x86_64.exe" /F 2>$null | Out-Null
        
        # Kill adb server
        $adbExe = Join-Path $env:ANDROID_HOME "platform-tools\\adb.exe"
        if (Test-Path $adbExe) {
          & $adbExe kill-server 2>$null | Out-Null
        }
        
        Write-Host "Cleanup completed."
      '''
    }
    success {
      echo 'Tests passed successfully!'
    }
    unstable {
      echo 'Some tests failed or were flaky.'
    }
    failure {
      echo 'Build failed.'
    }
  }
}