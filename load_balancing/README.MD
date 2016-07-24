# Load Balancing

Instead of leveraging a registration service and providing the client with a list of service locations,
another appliance, i.e. a load balancer, is placed in front of all the service instances. The client now
sends its requests to load balancer which is responsible for distributed the requests intelligently across
the various instances that are capable of processing the given requests.

In this sample, we're leveraging RabbitMQ as the load balancing appliance. On startup, the API service creates
a queue to listen to. By default that queue is will receive a binding to the default exchange. If you're unfamiliar
with RabbitMQ exchanges or AMQP in general take a look at this [short guide](https://www.rabbitmq.com/tutorials/amqp-concepts.html).
If you start multiple instances of the API service, they will all listen to the same queue. As requests come in,
the RabbitMQ broker will distribute the requests between all of the available service workers.

To get started, [download and install RabbitMQ](https://www.rabbitmq.com/download.html) on your OS. Start the RabbitMQ service
(if is hasn't been started already). You can use the `rabbitmqctl` command line utility to check the status of your RabbitMQ installation.
As an alternative, you could always setup a free RabbitMQ instance online with (CloudAMQP)[https://www.cloudamqp.com/]

You might have to update the sample projects to point to your RabbitMQ instance. Update the *appsetting.json* file in both
the client and server projects.

Start the School Service project. Open a command prompt and navigate to the location of the School Service project. Now run:

```
dotnet run
```
Actually, let's start two. Open another command prompt and start another instance of the School Service project.
Now start the client project. Open a command prompt and navigate to the location of the client project. Now run:

```
dotnet run
```

The client sends 2 requests to RabbitMQ. What you should notice is each request being send to a different instance to been
processed.