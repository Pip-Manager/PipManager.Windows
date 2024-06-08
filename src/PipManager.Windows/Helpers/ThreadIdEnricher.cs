using Serilog.Core;
using Serilog.Events;

namespace PipManager.Windows.Helpers;

internal class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ThreadId", Environment.CurrentManagedThreadId));
    }
}