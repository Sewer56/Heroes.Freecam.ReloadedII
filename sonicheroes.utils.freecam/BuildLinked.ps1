# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/sonicheroes.utils.freecam/*" -Force -Recurse
dotnet publish "./sonicheroes.utils.freecam.csproj" -c Release -o "$env:RELOADEDIIMODS/sonicheroes.utils.freecam" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location