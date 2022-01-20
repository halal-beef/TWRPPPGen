# Github workflows
# Made by SmallPP420
# the size of this program is gonna be huge xd (Yes it is.)

# Build Debug & Release
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build\\" -r win-x64 -c release # win build

dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build-debug\\" -r win-x64 -c debug # win build

# Move debug build
mv "/home/runner/work/TWRPPPGen/TWRPPPGen/build-debug/TWRPPPGen.exe" "/home/runner/work/TWRPPPGen/TWRPPPGen/build/DEBUG-TWRPPPGen.exe"
