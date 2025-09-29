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
          # Start Emulator
          Write-Host "`n=== Starting Emulator: $env:AVD_NAME ==="
          $emuArgs = @("-avd", $env:AVD_NAME, "-no-snapshot", "-no-boot-anim", "-no-audio", "-gpu", "off")
          Start-Process -FilePath $emulatorExe -ArgumentList $emuArgs -WindowStyle Hidden
          
          & $adbExe start-server | Out-Null
          & $adbExe wait-for-device | Out-Null
          
          $bootTimeout = (Get-Date).AddSeconds(240)
          Write-Host "Waiting for boot..."
          do {
            $bootComplete = & $adbExe shell getprop sys.boot_completed 2>$null
            if ($bootComplete -eq "1") { break }
            Start-Sleep -Seconds 3
          } while ((Get-Date) -lt $bootTimeout)
          
          if ($bootComplete -ne "1") { throw "Emulator failed to boot" }
          & $adbExe shell input keyevent 82  # Unlock
          Write-Host "Emulator ready!"
          
          # Start Appium
          Write-Host "`n=== Starting Appium on port $env:APPIUM_PORT ==="
          $port = [int]$env:APPIUM_PORT
          
          # Kill process on port if exists
          $inUse = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
          if ($inUse) {
            $inUse | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
            Start-Sleep -Seconds 2
          }
          
          # Start Appium job
          $appiumJob = Start-Job -ScriptBlock {
            param($p)
            npx appium --base-path / --port $p
          } -ArgumentList $port
          
          # Wait for Appium
          $appiumTimeout = (Get-Date).AddSeconds(30)
          do {
            $listening = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
            if ($listening) { break }
            Start-Sleep -Seconds 2
          } while ((Get-Date) -lt $appiumTimeout)
          
          if (!$listening) {
            Receive-Job -Job $appiumJob
            Stop-Job -Job $appiumJob
            throw "Appium failed to start"
          }
          Write-Host "Appium ready! (Job $($appiumJob.Id))"
          
          # Run Tests
          Write-Host "`n=== Running Tests ==="
          $env:APPIUM_SERVER_URL = "http://127.0.0.1:${port}"
          Write-Host "APPIUM_SERVER_URL = $env:APPIUM_SERVER_URL"
          Write-Host "Workspace: $(Get-Location)"
          
          # Create TestResults directory in workspace root
          New-Item -ItemType Directory -Path "TestResults" -Force | Out-Null
          
          dotnet test --configuration Release `
            --filter "LongPressMenuTest" `
            --no-build `
            --logger "nunit;LogFilePath=./TestResults/TestResult.xml" `
            -- NUnit.NumberOfTestWorkers=1
          
          Write-Host "`nTests completed!"
        '''
      }
      post {
        always {
          junit allowEmptyResults: true, testResults: '**/TestResults/TestResult.xml'
          
          archiveArtifacts artifacts: 'TestResults/**', allowEmptyArchive: true
        }
      }
    }
  }
  
  post {
    always {
      powershell '''
        Write-Host "`n=== Cleanup: Stopping Appium and Emulator ==="
        $ErrorActionPreference = "SilentlyContinue"
        
        # Stop Appium (all node processes)
        Get-Process node | 
          Where-Object { $_.Path -like "*\\node.exe" } | 
          Stop-Process -Force
        
        # Stop emulator
        Get-Process -Name "qemu-system-x86_64" | Stop-Process -Force
        taskkill /IM "qemu-system-x86_64.exe" /F *>$null
        
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