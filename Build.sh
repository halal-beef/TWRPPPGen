# Github workflows
# Made by SmallPP420
# the size of this program is gonna be huge xd
dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build\\" --arch x64 --os win -c release --self-contained true # win build

dotnet publish "/home/runner/work/TWRPPPGen/TWRPPPGen/TWRPPPGen/TWRPPPGen.csproj" --output "build-debug\\" --arch x64 --os win -c debug --self-contained true # win build

# Move debug build
mv "/home/runner/work/TWRPPPGen/TWRPPPGen/build-debug/TWRPPPGen.exe" "/home/runner/work/TWRPPPGen/TWRPPPGen/build/DEBUG-TWRPPPGen.exe"
