# Point to Point

Most of us have traditionally built our applications with the addresses of dependent services either
hardcoded or listed in a configuration file that's made available at deployment time.

This particular sample is the baseline for all of the others. This sample contains 2 projects; a simple API built with
ASP .NET Core and a HTTP API client. The easier way to see them working together is to
open a command line prompt from the root of each project and run the following command. Start with the API project first :)

```
dotnet run
```

The client inspects it's configuration (appsettings.json) file for the address of the API, then it makes 2 requests from the API.

Next See [Local Registry]()
