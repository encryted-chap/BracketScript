dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained true
del "bin\Debug\net5.0\win-x64\publish\BracketScript.pdb"