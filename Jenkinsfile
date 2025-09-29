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

    stage('Start Emulator & Appium') {
      steps {
        powershell '''
          ./ci/start-emulator.ps1 -AvdName "${env:AVD_NAME}" -BootTimeoutSec 240
          ./ci/start-appium.ps1 -Port $env:APPIUM_PORT
        '''
      }
    }

    stage('Run Tests') {
      steps {
        powershell '''
          $env:APPIUM_SERVER_URL = "http://127.0.0.1:${env:APPIUM_PORT}"

          Write-Host "APPIUM_SERVER_URL = $env:APPIUM_SERVER_URL"

          cd "Auto Test/APIDemos"
          ls -la

          dotnet test --configuration Release `
            --filter "LongPressMenuTest" `
            --no-build `
            --logger "nunit;LogFilePath=TestResults/TestResult.xml" `
            -- NUnit.NumberOfTestWorkers=1
        '''
      }
      post {
        always {
          junit allowEmptyResults: true, testResults: 'TestResults/TestResult.xml'
          publishHTML(target: [
            reportDir: 'TestResults',
            reportFiles: '**/index.html',
            reportName: 'Extent Report',
            keepAll: true,
            alwaysLinkToLastBuild: true,
            allowMissing: true
          ])
          archiveArtifacts artifacts: "TestResults/**", fingerprint: true, onlyIfSuccessful: false
        }
      }
    }
  }

  post {
    always {
      script {
        node('windows-android') {
          catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
            powershell 'ci/stop-mobile.ps1'
            archiveArtifacts artifacts: 'TestResults/**', fingerprint: true, onlyIfSuccessful: false
          }
        }
      }
    }
    success {
      echo 'Tests passed.'
    }
    unstable {
      echo 'Some tests failed or were flaky.'
    }
    failure {
      echo 'Build failed.'
    }
  }
}
