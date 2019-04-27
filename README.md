# Duplicati in a Windows container
Run the duplicati (https://www.duplicati.com/) backup tool on a Windows container host.

## Configuration
### Network
* 8200 TCP: Web interface for the server

### Volumes
* `C:\Config`: Directory for the configuration of your duplicati instance

## Run the container
```
docker run -d -p 8200:8200 -v C:\Docker\Duplicati\Config:C:\config dr1rrb/duplicati-win
```

## Tips: Running a Windows container on a AD joined host
Usually when running a Windows container, we need it to integrates with other servers and network infrastructure.
If you have an active directory (AD), it's pretty easy to give a valid network identity to your container.
You can find more info here https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/manage-serviceaccounts
and here https://artisticcheese.wordpress.com/2017/09/09/enabling-integrated-windows-authentication-in-windows-docker-container/ .

