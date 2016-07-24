# Local Registry

In this sample, instead of relying on a single service entry in a configuration file,
the application is provided with a pool of service that I can leverage to make requests
to. In the event of failure (server down, requests blocked, etc) the client can now move
on to the next location in the collection.

If you inspect the appsettings.json file in the client projects, you'll now see instead
of a single service location, our application is provided with a collection of address. In
the constructor of the ApiClient.cs, you'll see how the server address get bound to a collection,
and also we're also setting up a retry policy using the [Polly](https://github.com/App-vNext/Polly) library.
The way it's setup, for every service location, we'll make 2 request attempts before moving on to the next one.

To see both client and server working together,
open a command line prompt from the root of each project.

In the server project run

```
dotnet run --server.urls="http://localhost:5002"
```

Then from a command prompt in the client project folder run the following

```
dotnet run
```

After a few seconds, you should see the client print "Trying next server", before
successful making a request to your running service.

Next see [Self Registration]()
