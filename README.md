# Serwer TCP dla .NET Core 3.0

Przykład wykorzystania generycznego hosta z użyciem klasy HostBuilder do tworzenia aplikacji w stylu TCP Server.

Logika przetwarzania zapytań została celowo oddzielona od serwera TCP.
Dzięki temu można podpinać dowolne implementacje przetwarzania zapytań. Z drugiej strony można wykorzystać istniejącą logikę, ale wykorzystać inny protokół niż TCP.


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
public interface IRequestService
{
    Task<string> Send(string content);
}

public class MyRequestService : IRequestService
{
    public Task<string> Send(string content)
    {
        return Task.FromResult("OK");
    }
}
~~~

~~~ csharp
static async Task Main(string[] args)
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






