//using System;
//using System.Globalization;
//using System.Text;
//using Microsoft.Azure.WebJobs.Host;
//using paramore.brighter.commandprocessor.Logging;

// todo removed for now
//namespace Brighter.AzureExtensions.Functions
//{
//    public class TraceWriterLogProvider : ILogProvider
//    {
//        private readonly TraceWriter _traceWriter;

//        public TraceWriterLogProvider(TraceWriter traceWriter)
//        {
//            _traceWriter = traceWriter;
//        }

//        public Logger GetLogger(string name)
//        {
//            return (logLevel, messageFunc, exception, formatParameters) =>
//            {
//                var sb = new StringBuilder();
//                sb.AppendFormat(CultureInfo.InvariantCulture, messageFunc(), formatParameters);

//                if (exception != null)
//                {
//                    sb.Append(" | ");
//                    sb.Append(exception);
//                }

//                var message = sb.ToString();

//                switch (logLevel)
//                {
//                    case LogLevel.Trace:
//                    case LogLevel.Debug:
//                        _traceWriter.Verbose(message);
//                        break;

//                    case LogLevel.Info:
//                        _traceWriter.Info(message);
//                        break;

//                    case LogLevel.Warn:
//                        _traceWriter.Warning(message);
//                        break;

//                    case LogLevel.Error:
//                    case LogLevel.Fatal:
//                        _traceWriter.Error(message);
//                        break;
//                }

//                return true;
//            };
//        }

//        public IDisposable OpenNestedContext(string message)
//        {
//            return NullDisposable.Instance;
//        }

//        public IDisposable OpenMappedContext(string key, string value)
//        {
//            return NullDisposable.Instance;
//        }

//        private class NullDisposable : IDisposable
//        {
//            internal static readonly IDisposable Instance = new NullDisposable();

//            public void Dispose()
//            {
//            }
//        }
//    }
//}