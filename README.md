SignalR-TypeSafeClient
======================

A type safe client interface for SignalR in C#

The `HubClient` uses two interface types which should be shared with your hub: *Requests* and *Events*.
Events are calls made from the Hub to the Client. Requests are calls made from the Client to the Hub.


Contracts and Hub
-----------------
For this readme, we assume a Hub like:
```csharp

    [HubName(HubNames.ScheduledTaskQueueHub)]
    public class ScheduledTaskQueueHub : Hub, IRequests, IEvents
    {
        . . .
    }

```

With the two interfaces:

```csharp

    public interface IRequests {
        void IWantStuff(string[] kindsOfStuff);
        int HowManyClients();
    }

    public interface IEvents {
        void StuffAvailable(string stuff);
        void SessionExpired();
    }

```

Connecting
----------
```csharp

    var conn = new HubConnection("http://localhost:8080/"); // replace with your Hub's location

    conn.TransportConnectTimeout = TimeSpan.FromSeconds(15);
    conn.TraceLevel = TraceLevels.All;
    conn.TraceWriter = Console.Out

    var proxy = conn.CreateHubProxy("MyHub"); // replace with your hub name or hub class name

    var connection = conn.Start();

    if (!connection.Wait(TimeSpan.FromSeconds(30)))
    {
        conn.Dispose();
        throw new TimeoutException("Could not connect to " + endpoint);
    }

    _client = new HubClient<IRequests, IEvents>(proxy, conn);
```

Hooking up events
-----------------
These bindings

```csharp

    _client.BindEventHandler<string>(hub => hub.StuffAvailable, HandleIncomingStuff);
    _client.BindEventHandler(hub => hub.SessionExpired, AlertSessionExpired);

```

will call these methods

```csharp

    void HandleIncomingStuff(string stuff) { . . . }
    void AlertSessionExpired() { . . . }

``

Sending Requests
----------------

```csharp

    _client.SendToHub(hub => hub.IWantStuff(new []{"stuff1", "stuff2"}));

    var clients = _client.RequestFromHub(hub => hub.HowManyClients());

```