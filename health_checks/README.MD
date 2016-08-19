# Health Checks

Health checks provide a mechanism for the client to know whether or not it's dependant services are still up and running.
This would prevent unnecessary requests to dead servers. There are various ways that these checks can be implemented. For this
sample, the API service provides a health endpoint accessible at `/api/health/status`. During self registration, the
API serive will provide Consul with this status endpoint, a check internval, and a timeout period. The Consul Registration Service
will now send a `GET` requests to the provided endpoint based on the configured internval. If the endpoint is unavailable,
takes too long to respond, or sends back an unsuccessful status code, then the service is marked as unhealthy. However, the entry in the
registry is not removed automatically removed. see the *RegisterWithConsul* extension method in [Extensions.cs](src/SchoolAPI/Infrastructure/Extensions.cs)

Make sure you have Consul running
```
consul agent -dev --node yourNodeName --bind 11.11.11.11 -client 11.11.11.11 -ui
```

Next start the API service. Open 2 command terminals and navigate to the location
of the API project. Now run:

```
dotnet run
```
```
dotnet run --server.urls=http://localhost:50001
```

As before, the service will register itself with Consul but now it'll also register the status endpoint for Consul to use
for health checks. In your command terminal, you should now see requests being made every 10 seconds or so to `/api/health/status`.
Take a look at the service tab in the Consul UI to see the health status of your service. I should be available at the IP your
set Consul to run at i.e. `11.11.11.11:8500/ui`.

Also, do a `dotnet run` in a command terminal in the directory of the client project. The client will check the health of the service
before make a request for data.

Now try stopping the server and refreshing the Consul ui in your browser and see what happens.

Next [Load Balancing](../load_balancing/README.MD)