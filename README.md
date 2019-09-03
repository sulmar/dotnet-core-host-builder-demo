# dotnet-core-host-builder-demo
Przykład wykorzystania generycznego hosta z użyciem klasy HostBuilder do tworzenia aplikacji w stylu TCP Server.


## Opis

Metoda *UseTcpService<T>()* tworzy instancję serwera TCP, a przychodzące zapytania obsługiwane są przez delegat  


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




