FROM mcr.microsoft.com/dotnet/runtime:9.0-windowsservercore-ltsc2019

WORKDIR C:\\duplicati

# Download and unzip Duplicati
RUN cmd /C curl -L {duplicati.install} -o duplicati.zip & \
tar -xf duplicati.zip --strip-components=1 & \
del duplicati.zip

# Image config
EXPOSE 8200/tcp
VOLUME C:\\config
CMD Duplicati.Server.exe --webservice-interface=any --webservice-port=8200 --webservice-allowed-hostnames=* --server-datafolder=C:\\config