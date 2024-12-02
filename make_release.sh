rm -rf obj
rm -rf bin

dotnet build RF5ItemDropRate.csproj -f net6.0 -c Release

zip -j 'RF5ItemDropRate_v1.0.0.zip' './bin/Release/net6.0/RF5ItemDropRate.dll' './bin/Release/net6.0/RF5ItemDropRate.cfg'
cp './bin/Release/net6.0/RF5ItemDropRate.dll' '/data/Steam/steamapps/common/Rune Factory 5/BepInEx/plugins'