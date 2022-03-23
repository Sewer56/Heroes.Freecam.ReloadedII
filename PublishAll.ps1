
# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

./Publish.ps1 -ProjectPath "sonicheroes.utils.freecam/sonicheroes.utils.freecam.csproj" `
              -PackageName "sonicheroes.utils.freecam" `

Pop-Location