# Health Checks

Health checks provide a mechanism for the client to know whether or not it's dependant services are still up and running.
This would prevent unnecessary requests to head servers. There are various ways that these checks can be implemented. For this
sample, the API service now provides a health endpoint accessible at `/api/health/status`. During self registration, the
API serive will provide Consul with this status endpoint, a check internval, and a timeout period. The Consul Registration Service
will now send a `GET` requests to the provided endpoint based on the configured internval. If the endpoint is unavailable,
takes too long to respond, or sends back an unsuccessful status code, then the service is marked as unhealthy. However, the entry in the
registry is not removed automatically removed. see the *RegisterWithConsul* extension method in [Extensions.cs](src/SchoolAPI/Infrastructure/Extensions.cs)


Next [Load Balancing]()