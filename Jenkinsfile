pipeline {
  agent { label 'windows-android' }
  
  options {
    timestamps()
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

    stage('Start Appium Server') {
      steps {
        script {
          sh 'appium --log-level error > appium.log 2>&1 &'
          sh 'sleep 5' // Wait for Appium to start
        }
      }
    }

    stage('Start Emulator') {
      steps {
        script {
          sh 'emulator -avd $AVD_NAME -no-snapshot -no-audio -no-window &'
          sh 'adb wait-for-device shell "while [[ -z $(getprop sys.boot_completed) ]]; do sleep 1; done"'
        }
      }
    }

    
    stage('Run Tests') {
      steps {
        powershell '''
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