# Duplicati in a Windows container
Run the [duplicati backup tool](https://www.duplicati.com/) on a Windows container host.

[![Build Status](https://dev.azure.com/dr1rrb/docker-duplicati-win/_apis/build/status/dr1rrb.docker-duplicati-win?branchName=master)](https://dev.azure.com/dr1rrb/docker-duplicati-win/_build/latest?definitionId=1&branchName=master)
![crawler](https://healthchecks.io/badge/68b95c5c-92b3-4dc3-be66-88f08cb712e8/uloFLUs6.svg "crawler")

## Configuration
### Network
* 8200 TCP: Web interface for the server

### Volumes
* `C:\Config`: Directory for the configuration of your duplicati instance

## Run the container
### Command line
```
docker run -d -p 8200:8200 -v C:\Docker\Duplicati\Config:C:\config dr1rrb/duplicati-win
```

### Docker compose
Suggested docker compose file
```
version: '3.4'
services:
  duplicati:
    container_name: duplicati
    image: dr1rrb/duplicati-win
    restart: unless-stopped
    volumes:
      - C:\Docker\Duplicati\Config:C:\config
    ports:
      - 8200:8200
    security_opt: 
      - "credentialspec=file://duplicati.json" # Network identity, cf. TIPS below
    hostname: duplicati
```

## TIPS: Running a Windows container on a AD joined host
Usually when running a Windows container, we need it to integrates with other servers and network infrastructure.
If you have an active directory (AD), it's pretty easy to give a valid network identity to your container.
You can find more info [here](https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/manage-serviceaccounts)
and [here](https://artisticcheese.wordpress.com/2017/09/09/enabling-integrated-windows-authentication-in-windows-docker-container/).

## Crawler
A crawler is running every day to check if a new release is avaible on the [duplicati repository](https://github.com/duplicati/duplicati/releases) and automatically build a new docker image for it. 

Here are the statues of this crawler for each release channel:
![stable](https://healthchecks.io/badge/68b95c5c-92b3-4dc3-be66-88f08cb712e8/4ug0eilr/stable.svg)
![beta](https://healthchecks.io/badge/68b95c5c-92b3-4dc3-be66-88f08cb712e8/cX68_bL4/beta.svg)
![experimental](https://healthchecks.io/badge/68b95c5c-92b3-4dc3-be66-88f08cb712e8/99oR8r2a/experimental.svg)
![canary](https://healthchecks.io/badge/68b95c5c-92b3-4dc3-be66-88f08cb712e8/vtJIAS2-/canary.svg)

