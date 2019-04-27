FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

WORKDIR C:\\duplicati

# Download and unzip Duplicati
RUN cmd /C curl -L https://github.com/duplicati/duplicati/releases/download/v2.0.4.17-2.0.4.17_canary_2019-04-11/duplicati-2.0.4.17_canary_2019-04-11.zip -o duplicati.zip & \
tar -xf duplicati.zip & \
del duplicati.zip

# Image config
EXPOSE 8200/tcp
VOLUME C:\\config
CMD Duplicati.Server.exe --webservice-interface=any --webservice-port=8200 --webservice-allowed-hostnames=* --server-datafolder=C:\\config