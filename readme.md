# SQLBrowser
Library compatible with .NET 5.0 as standard libraries appear to have compatibility issues

Allows for local and network SQL server discovery


![Build status](https://dev.azure.com/DevFromDownUnder/SQLBrowser/_apis/build/status/SQLBrowser-master-pushToGit)
[![Nuget release](https://img.shields.io/nuget/v/DevFromDownUnder.SQLBrowser.svg?style=flat&label=DevFromDownUnder.SQLBrowser&logo=nuget&color=blue)](https://www.nuget.org/packages/DevFromDownUnder.SQLBrowser/)

### SQLBrowser.Example Console Serialized Result
```
12:06:28 info: SQLBrowser.Example.Program[0] Discovery started
12:06:28 info: SQLBrowser.Example.Program[0] Network discovery started
12:06:28 info: SQLBrowser.Example.Program[0] Local discovery started
12:06:28 info: SQLBrowser.Example.Program[0] Local discovery ended
12:06:30 info: SQLBrowser.Example.Program[0] Network discovery ended
12:06:30 info: SQLBrowser.Example.Program[0] Discovery ended

Json results:
[
  {
    "ServerName": ".",
    "Type": "Local",
    "HostName": ".",
    "InstanceName": "MSSQLSERVER",
    "IsClustered": false,
    "VersionString": "15.0.2000.5",
    "Version": "SQLServer2019",
    "NamedPipe": "\\\\.\\pipe\\sql\\query",
    "NamedPipeEnabled": false,
    "TCPPort": null,
    "via": null,
    "rpc": null,
    "spx": null,
    "adsp": null,
    "bv": null
  },
  {
    "ServerName": "LOCALPC",
    "Type": "Network",
    "HostName": "LOCALPC",
    "InstanceName": "MSSQLSERVER",
    "IsClustered": false,
    "VersionString": "15.0.2000.5",
    "Version": "SQLServer2019",
    "NamedPipe": null,
    "NamedPipeEnabled": false,
    "TCPPort": null,
    "via": null,
    "rpc": null,
    "spx": null,
    "adsp": null,
    "bv": null
  },
  {
    "ServerName": "REMOTEPC1360",
    "Type": "Network",
    "HostName": "REMOTEPC1360",
    "InstanceName": "MSSQLSERVER",
    "IsClustered": false,
    "VersionString": "11.0.6020.0",
    "Version": "SQLServer2012",
    "NamedPipe": "\\\\REMOTEPC1360\\pipe\\sql\\query",
    "NamedPipeEnabled": true,
    "TCPPort": 1525,
    "via": null,
    "rpc": null,
    "spx": null,
    "adsp": null,
    "bv": null
  },
  {
    "ServerName": "REMOTEPC1380\\DYNATRACE",
    "Type": "Network",
    "HostName": "REMOTEPC1380",
    "InstanceName": "DYNATRACE",
    "IsClustered": false,
    "VersionString": "10.50.6000.34",
    "Version": "SQLServer2008R2",
    "NamedPipe": "\\\\REMOTEPC1380\\pipe\\MSSQL$DYNATRACE\\sql\\query",
    "NamedPipeEnabled": true,
    "TCPPort": 5001,
    "via": null,
    "rpc": null,
    "spx": null,
    "adsp": null,
    "bv": null
  }
]

Searching complete
```