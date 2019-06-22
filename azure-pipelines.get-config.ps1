# $Env:BUILD_ARTIFACTSTAGINGDIRECTORY = "C:\test"
# $Env:DUPLICATI_DEFAULT_VERSION = "default-version"
# $Env:DUPLICATI_DEFAULT_INSTALL = "default-install"

$artifacts = $Env:BUILD_ARTIFACTSTAGINGDIRECTORY
cd $artifacts
mkdir Docker
cd Docker

$channels = "default", "stable", "beta", "experimental", "canary", "custom"
$channel  = $Env:DUPLICATI_CHANNEL
if ($channel -like '') { $channel = "default" }

$channelsConfigs = $channels | Select @{l="Channel";e={$_}}, @{l="Config";e={"Env:DUPLICATI_" + $_.ToUpper()  + "_*" | Get-Item | Select @{l="Key";e={$_.Name.ToString().Split('_')[2].ToLower()}}, Value } }
$channelConfig   = ($channelsConfigs | Where Channel -eq $channel).Config

# Set build variables (And dump to file for release pipeline)
$channelConfig | ForEach { "##vso[task.setvariable variable=duplicati."+$_.Key+"]"+ $_.Value | Write-Host }
$channelConfig | ForEach { "##vso[task.setvariable variable=duplicati."+$_.Key+"]"+ $_.Value } > "config.txt"

# Dump config to output
Write-Host ""
Write-Host "Build windows docker image of duplicati:"
Write-Host "  Requested channel: $channel"
$channelConfig | ForEach { "  " + $_.Key + ": " + $_.Value | Write-Host }

# Validate the config
if (($channelConfig | Where Key -eq "version").Value -like '') { Write-Error "Version must be defined" }
if (($channelConfig | Where Key -eq "install").Value -like '') { Write-Error "Installer URL must be defined" }
