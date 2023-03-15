using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace OTelInitWrapper
{
    public static class Init
    {
        public static TracerWrapper Initialize(Func<TracerProviderBuilder, TracerProviderBuilder> configureTracerProviderBuilder)
        {
            var activitySource = new ActivitySource("OTelInitWrapper");

            var tracerProviderBuilder = Sdk.CreateTracerProviderBuilder()
                .SetSampler(new AlwaysOnSampler())
                .AddSource(activitySource.Name);

            if (configureTracerProviderBuilder != null)
            {
                tracerProviderBuilder = configureTracerProviderBuilder(tracerProviderBuilder);
            }

            tracerProviderBuilder = tracerProviderBuilder.AddConsoleExporter();

            TracerProvider tracerProvider = tracerProviderBuilder.Build();

            return new TracerWrapper(activitySource, tracerProvider);
        }
    }
}