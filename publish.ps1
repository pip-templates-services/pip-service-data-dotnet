#!/usr/bin/env pwsh

Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

# Get component data and set necessary variables
$component = Get-Content -Path "component.json" | ConvertFrom-Json
$rcImage = "$($component.registry)/$($component.name):$($component.version)-$($component.build)-rc"
$latestImage = "$($component.registry)/$($component.name):latest"

# Define server name
$pos = $component.registry.IndexOf("/")
$server = ""
if ($pos -gt 0) {
    $server = $component.registry.Substring(0, $pos)
}

# Automatically login to server
if ($env:DOCKER_USER -ne $null -and $env:DOCKER_PASS -ne $null) {
    docker login $server -u $env:DOCKER_USER -p $env:DOCKER_PASS
}

# Push image to docker registry
docker push $rcImage

# Check that image was pushed successfully
if ($LastExitCode -ne 0) {
    Write-Error "Can't push image '$rcImage' to docker registry. Make sure you use correct credentials in environment variables DOCKER_USER AND DOCKER_PASS on login or check package.ps1 logs." -ErrorAction Stop
}

# Push image to docker registry
docker push $latestImage

# Check that image was pushed successfully
if ($LastExitCode -ne 0) {
    Write-Error "Can't push image '$latestImage' to docker registry. Make sure you use correct credentials in environment variables DOCKER_USER AND DOCKER_PASS on login or check package.ps1 logs." -ErrorAction Stop
}