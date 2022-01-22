# GitHub CI Builder
# Credits:
# - SmallPP420
# - TriDiscord

## Build Debug & Release
# Windows Builds
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build\\" -r win-x64 -c release --self-contained
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build-debug\\" -r win-x64 -c debug --self-contained
# Linux Builds
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build-linux\\" -r linux-x64 -c release --self-contained
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build-linux-debug\\" -r linux-x64 -c debug --self-contained
