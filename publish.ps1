#!/usr/bin/env pwsh

Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

# Generate an image name using the data in the "component.json" file
$component = Get-Content -Path "component.json" | ConvertFrom-Json
$image = "$($component.registry)/$($component.name):$($component.version)-$($component.build)-rc"

# Define a server name
$pos = $component.registry.IndexOf("/")
$server = ""
if ($pos -gt 0) {
    $server = $component.registry.Substring(0, $pos)
}

# Automatically login to the server
if ($env:DOCKER_USER -ne $null -and $env:DOCKER_PASS -ne $null) {
    docker login $server -u $env:DOCKER_USER -p $env:DOCKER_PASS
}

# Push production image to docker registry
docker push $image