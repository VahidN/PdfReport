NuGet.exe pack Package.nuspec
copy "%~dp0*.nupkg" "%localappdata%\NuGet\Cache"
pause