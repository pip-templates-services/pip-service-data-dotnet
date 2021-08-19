#!/usr/bin/env pwsh

##Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

$component = Get-Content -Path "component.json" | ConvertFrom-Json

$buildImage = "$($component.registry)/$($component.name):$($component.version)-$($component.build)-protos"
$container=$component.name

# Remove build files
if (Test-Path "./src/Protos") {
    Remove-Item -Recurse -Force -Path "./src/Protos/*V1.cs"
} else {
    New-Item -ItemType Directory -Force -Path "./src/Protos"
}

# Build docker image
docker build -f docker/Dockerfile.proto -t $buildImage .

# Create and copy compiled files, then destroy
docker create --name $container $buildImage
docker cp "$($container):/app/src/Protos/" ./src/
docker rm $container

if (!(Test-Path "./src/Protos")) {
    Write-Host "protos folder doesn't exist in root dir. Build failed. Watch logs above."
    exit 1
}
