param(
    [string] $Architecture = "x64",
    [string] $Version = "0.1.4.0"
)

$ErrorActionPreference = "Stop";

Write-Output "Start building singleWithRuntime...";

dotnet publish src/PipManager.Windows.csproj -c Release -r "win-$Architecture" -o "build/$Version/singleWithRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=true -p:PublishSingleFile=true -p:SelfContained=true -p:AssemblyVersion=$Version -p:Configuration=Release;

Rename-Item -Path "build/$Version/singleWithRuntime/PipManager.Windows.exe" -NewName "PipManager_withRuntime.exe"

Remove-Item -Path "build/$Version/singleWithRuntime/*.xml" -Force -ErrorAction SilentlyContinue

Write-Output "Start building singleWithoutRuntime...";

dotnet publish  src/PipManager.Windows.csproj -c Release -r "win-$Architecture" -o "build/$Version/singleWithoutRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=false -p:EnableCompressionInSingleFile=false -p:PublishSingleFile=true -p:SelfContained=false -p:AssemblyVersion=$Version -p:Configuration=Release;

Rename-Item -Path "build/$Version/singleWithoutRuntime/PipManager.Windows.exe" -NewName "PipManager.exe"

Remove-Item -Path "build/$Version/singleWithoutRuntime/*.xml" -Force -ErrorAction SilentlyContinue

Write-Output "Build Finished";

[Console]::ReadKey()