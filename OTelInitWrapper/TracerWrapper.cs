using System;
using System.Diagnostics;
using OpenTelemetry.Trace;

namespace OTelInitWrapper
{
    public class TracerWrapper : IDisposable
    {
        public TracerWrapper(ActivitySource source, TracerProvider provider)
        {
            Source = source;
            Provider = provider;
        }

        public void Dispose()
        {
            Source.Dispose();
            Provider.Dispose();
        }

        public ActivitySource Source { get; }

        private TracerProvider Provider { get; }
    }
}