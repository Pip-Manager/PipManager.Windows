param(
    [string] $Architecture = "x64",
    [string] $Version = "0.1.0.0"
)

$ErrorActionPreference = "Stop";

Write-Output "Start building singleWithRuntime...";

dotnet publish src/PipManager.Windows.csproj -c Release -r "win-$Architecture" -o "build/$Version/singleWithRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=true -p:PublishSingleFile=true -p:SelfContained=true -p:AssemblyVersion=$Version -p:Configuration=Release;

Rename-Item -Path "build/$Version/singleWithRuntime/PipManager.exe" -NewName "PipManager_withRuntime.exe"

Write-Output "Start building singleWithoutRuntime...";

dotnet publish  src/PipManager.Windows.csproj -c Release -r "win-$Architecture" -o "build/$Version/singleWithoutRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=false -p:EnableCompressionInSingleFile=false -p:PublishSingleFile=true -p:SelfContained=false -p:AssemblyVersion=$Version -p:Configuration=Release;

Write-Output "Build Finished";

[Console]::ReadKey()