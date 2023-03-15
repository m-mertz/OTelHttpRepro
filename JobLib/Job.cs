using System;
using System.Net.Http;
using System.Threading.Tasks;
using OpenTelemetry.Trace;
using OTelInitWrapper;

namespace JobLib
{
    public class Job : IDisposable
    {
        public Job()
        {
            Wrapper = Init.Initialize(tracerProviderBuilder => tracerProviderBuilder.AddHttpClientInstrumentation());
        }

        public async Task RunAsync()
        {
            using (var activity = Wrapper.Source.StartActivity("RunAsync"))
            {
                const string targetUrl = "https://www.bing.com/";

                using (HttpResponseMessage response = await HttpClient.GetAsync(targetUrl).ConfigureAwait(false))
                {
                    Console.WriteLine($"Got response from {targetUrl} with status code {response.StatusCode}\n");
                }
            }
        }

        public void Dispose()
        {
            Wrapper.Dispose();
        }

        private HttpClient HttpClient { get; } = new HttpClient();

        private TracerWrapper Wrapper { get; }
    }
}