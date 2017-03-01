using Microsoft.Azure.WebJobs.Host;
using Serilog.Core;
using Serilog.Events;

namespace Brighter.AzureExtensions.Functions
{
    public class TraceWriterSink : ILogEventSink
    {
        private readonly TraceWriter _traceWriter;

        public TraceWriterSink(TraceWriter traceWriter)
        {
            _traceWriter = traceWriter;
        }

        public void Emit(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                    _traceWriter.Verbose(logEvent.RenderMessage());
                    break;

                case LogEventLevel.Information:
                    _traceWriter.Info(logEvent.RenderMessage());
                    break;

                case LogEventLevel.Warning:
                    _traceWriter.Warning(logEvent.RenderMessage());
                    break;

                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    _traceWriter.Error(logEvent.RenderMessage());
                    break;
            }
        }
    }
}