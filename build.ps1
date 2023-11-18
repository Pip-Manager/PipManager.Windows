param(
    [string] $Architecture = "x64",
    [string] $Version = "1.0.0.1"
)

$ErrorActionPreference = "Stop";

Write-Output "Start building singleWithRuntime...";

dotnet publish src/PipManager -c Release -r "win-$Architecture" -o "build/$Version/singleWithRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=true -p:PublishSingleFile=true -p:SelfContained=true -p:AssemblyVersion=$Version;

Write-Output "Start building singleWithoutRuntime...";

dotnet publish src/PipManager -c Release -r "win-$Architecture" -o "build/$Version/singleWithoutRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=false -p:EnableCompressionInSingleFile=false -p:PublishSingleFile=true -p:SelfContained=false -p:AssemblyVersion=$Version;

Write-Output "Start building extractedWithRuntime...";

dotnet publish src/PipManager -c Release -r "win-$Architecture" -o "build/$Version/extractedWithRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=false -p:PublishSingleFile=false -p:SelfContained=true -p:AssemblyVersion=$Version;

Write-Output "Start building extractedWithoutRuntime...";

dotnet publish src/PipManager -c Release -r "win-$Architecture" -o "build/$Version/extractedWithoutRuntime" -p:Platform=$Architecture -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=false -p:PublishSingleFile=false -p:SelfContained=false -p:AssemblyVersion=$Version;

Write-Output "Build Finished";

[Console]::ReadKey()