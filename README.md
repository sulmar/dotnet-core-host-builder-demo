# Serwer TCP dla .NET Core 3.0

Przykład wykorzystania generycznego hosta z użyciem klasy HostBuilder do tworzenia aplikacji w stylu TCP Server.

Klasa MyTcpServer implementuje interfejs IHostedService.


## Opis

Metoda *UseTcpService<T>()* konfiguruje serwer TCP.


W metodzie można przekazać delegat do obsługi zapytań:

 Przykład użycia

~~~ csharp
static async Task Main(string[] args)
{
    IHost host = new HostBuilder()
        .UseTcpService(request => Task.FromResult("OK"))
        .Build();

    await host.RunAsync();
}
~~~

lub przekazać instancję klasy, która musi implementować interfejs IRequestService

~~~ csharp
 tatic async Task Main(string[] args)
{
    
    IHost host = new HostBuilder()
        .UseTcpService(new MyRequestService())

        .Build();

    await host.RunAsync();
}
~~~


## Konfiguracja

Domyślny port to 5000 ale można go zmienić poprzez konfigurację:

~~~ json

"MyOptions": {
    "port": 8899
  }

~~~






