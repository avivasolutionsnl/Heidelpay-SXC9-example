# escape=`
FROM avivasolutionsnl.azurecr.io/sxc-solr:9.0.2

SHELL ["powershell", "-NoProfile", "-Command", "$ErrorActionPreference = 'Stop';"]

ADD mercury-solr.json /Files/Config/
ADD managed-schema /Files/Config/

RUN /Scripts/WaitForSolr.ps1 "solr"; `
    Install-SitecoreConfiguration -Path "C:\\Files\\Config\\mercury-solr.json"
