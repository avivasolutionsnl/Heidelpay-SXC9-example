# escape=`
FROM avivasolutionsnl.azurecr.io/sxc-commerce:9.0.2

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Delete does not always work
# RUN [IO.Directory]::Delete('C:\\inetpub\\wwwroot\\CommerceAuthoring_Sc9', $true); `
#     [IO.Directory]::Delete('C:\\inetpub\\wwwroot\\CommerceMinions_Sc9', $true); `
#     [IO.Directory]::Delete('C:\\inetpub\\wwwroot\\CommerceOps_Sc9', $true); `
#     [IO.Directory]::Delete('C:\\inetpub\\wwwroot\\CommerceShops_Sc9', $true)

ADD ./WatchDirectoryMultiple.ps1 /Scripts

# Install VS 2017 remote debugging
ADD ./RemoteDebug.ps1 /Scripts

RUN choco install -y --params="Quiet" visualstudio2017-remotetools

ENTRYPOINT /Scripts/RemoteDebug.ps1; `
    /Scripts/WatchDirectoryMultiple.ps1 -Path C:\Workspace `
            -Destinations @('C:\\inetpub\\wwwroot\\CommerceAuthoring_Sc9', `
                            'C:\\inetpub\\wwwroot\\CommerceMinions_Sc9', `
                            'C:\\inetpub\\wwwroot\\CommerceOps_Sc9', `
                            'C:\\inetpub\\wwwroot\\CommerceShops_Sc9')
