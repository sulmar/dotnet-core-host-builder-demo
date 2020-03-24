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
        .UseTcpService(request => $"ECHO {request}")
        .Build();

    await host.RunAsync();
}
~~~

Sposób przetwarzania żądań można wskazać poprzez funkcję zgodną z *Func<string, string>*

## Konfiguracja

Domyślny port to 5000 ale można go zmienić poprzez konfigurację:

~~~ json

"TcpHostedServer": {
    "port": 8899
  }

~~~



