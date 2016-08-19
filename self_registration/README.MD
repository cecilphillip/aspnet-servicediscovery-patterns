# Self Registration

In this sample, instead of hard coding addresses or providing a configuration file at deploy time,
we're going to make use of a registration service. When our server starts up it will contact the registration
service to register itself. You can compare this to clocking into work for your job. The service essentially is
telling the registration service it's available, what it's address is and also provide some other meta data if
needed. [Consul](https://www.consul.io/) by HashiCorp is what we'll use as our registration service.

To run this demo you'll need to [download](https://www.consul.io/downloads.html) and
[install](https://www.consul.io/intro/getting-started/install.html) Consul. Now [start](https://www.consul.io/intro/getting-started/agent.html)
the service.
The following command is enough in most cases to run a single node consul environment
```
consul agent -dev
```

If you have multiple IPs on your machine (usually the case if you have VM software setup or multiple network adapters) then
the previous command might have failed. Instead, you'll have to specify the IP you wish for Consul to listen on. The following
startup options might help.

```
consul agent -dev --node yourNodeName --bind 11.11.11.11 -client 11.11.11.11 -ui
```

#### Quick Consul command reference
- -dev : Puts Consul in development mode. This will not persist any state you provide it with
- --node : This is a unique name for the agent. You can use whatever naming convention works for you
- --bind : This is the address used for communication between Consul agents in a cluster. In our case
we're running a single node cluster in dev mode.
- -client: This is the address used for client interfaces to the agent. HTTP API requests, DNS request, etc
- -ui : enables the consul ui. Once agent has started, the UI should be available at <<-Your-IP->>:8500/ui


Once you have Consul up and running, make sure that the server and client have the right IP address for your
Consul agent set in the *appsettings.json* file in each project.
Next start the API project. Open a command prompt and navigate to the location
of the API project. Now run:

```
dotnet run
```
Once the API service starts up successfuly, you should be able to see a registration entry in the Consul UI
under the *Services* tab. If you want to see the code responsible for the registration see the *RegisterWithConsul* extension method in [Extensions.cs](src/SchoolAPI/Infrastructure/Extensions.cs)

Now for the client project. Open a command prompt and navigate to the location
of the client project. Now run:

```
dotnet run
```

If everything is configured currectly, your client will retrieve the list of registered servers from our registration serivce
AKA Consul. Right now there's only one, but feel to start up another instance of the API in another command prompt window.

Next [Health Checks](../health_checks/README.MD)