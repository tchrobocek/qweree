rm -rf bin
rm -rf obj
dotnet publish -r osx.10.15-x64 -c Release -p:PublishSingleFile=True;AssemblyName=qweree
dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=True;AssemblyName=qweree
dotnet publish -r win-x64 -c Release -p:PublishSingleFile=True;AssemblyName=qweree

mv ./bin/Release/net5.0/osx.10.15-x64/publish/Qweree.ConsoleApplication ./bin/Release/net5.0/osx.10.15-x64/publish/qweree
mv ./bin/Release/net5.0/linux-x64/publish/Qweree.ConsoleApplication ./bin/Release/net5.0/linux-x64/publish/qweree
mv ./bin/Release/net5.0/win-x64/publish/Qweree.ConsoleApplication.exe ./bin/Release/net5.0/win-x64/publish/qweree.exe