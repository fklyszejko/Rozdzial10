using Microsoft.Extensions.Logging;
using static System.Console;

namespace BibliotekaWspolna;

public class DostawcaProkoluKonsoli : ILoggerProvider
{
    public ILogger CreateLogger(string nazwaKategorii)
    {
        // można tworzyć osobne implementacje dla różnych
        // nazw kategorii, ale tutaj mamy tylko jedną
        return new ProtokolKonsoli();
    }

    // jeżeli klasa protokołu używa zasoób niezarządzalnych
    // to tutaj powinniśmy je zwolnić
    public void Dispose() { }
}

public class ProtokolKonsoli : ILogger
{
    // jeżeli klasa protokołu używa zasobów niezarządalnych,
    // to tutaj możesz zwrócić klasę implementującą
    // interfejs IDisponsable
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        // aby ograniczyć ilość protokołowanych informacji
        // możesz tutaj filtrować według poziomu protokołu
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Information:
            case LogLevel.None:
                return false;
            case LogLevel.Debug:
            case LogLevel.Warning:
            case LogLevel.Error:
            case LogLevel.Critical:
            default:
                return true;
        };
    }
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (eventId.Id == 20100)
        {
            //wypisz pozom protokołu i identyfikator zdarzenia
            Write($"Poziom: {logLevel}, ID zdarzenia: {eventId.Id}");
            if (state != null)
            {
                Write($", State: {state}");
            }
            if (exception != null)
            {
                Write($", Wyjątek: {exception.Message}");
            }
            WriteLine();
        }
    }
}