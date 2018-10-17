# escape=`
FROM avivasolutionsnl.azurecr.io/sxc-sitecore:9.0.2

SHELL ["powershell", "-NoProfile", "-Command", "$ErrorActionPreference = 'Stop';"]

# First have to initialize the commerce engine and unicorn the store front settings
# RUN $engineConnectIncludeDir = 'c:\\inetpub\\wwwroot\\sitecore\\App_Config\\Include\\Y.Commerce.Engine'; `
#     $oldName = $(Join-Path -Path $engineConnectIncludeDir -ChildPath "\Sitecore.Commerce.Engine.DataProvider.config.disabled"); `
#     $newName = $(Join-Path -Path $engineConnectIncludeDir -ChildPath "\Sitecore.Commerce.Engine.DataProvider.config"); `
#     ren $oldName $newName

# Disable Solr initialize on add
RUN mv  C:\inetpub\wwwroot\sitecore\App_Config\Include\zSitecore.Commerce.Engine.Connectors.Index.Solr.InitializeOnAdd.config  C:\inetpub\wwwroot\sitecore\App_Config\Include\zSitecore.Comerce.Engine.Connectors.Index.Solr.InitializeOnAdd.config.disabled

RUN choco install -y --params="Quiet" vcredist2013; `
    choco install -y --params="Quiet" visualstudio2017-remotetools

# Run remote debugger on start and monitor changes 
ENTRYPOINT $procs = $(Start-Process -FilePath 'C:\Program Files\Microsoft Visual Studio 15.0\Common7\IDE\Remote Debugger\x64\msvsmon.exe' `
                         -ArgumentList '/nostatus /silent /noauth /anyuser /nosecuritywarn /timeout 3600000' -PassThru; ` 
                      Start-Process powershell -ArgumentList '/Scripts/Watch-Directory.ps1 -Path C:\Workspace -Destination c:\inetpub\wwwroot\sitecore' -PassThru); `
           $procs | Wait-Process