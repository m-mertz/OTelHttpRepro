using System;
using System.IO;
using System.Threading.Tasks;

namespace OTelHttpRepro
{
    public static class Program
    {
        public static void Main()
        {
            // This doesn't capture HTTP GET span for HttpClient call in JobLib.Job class.
            Repro();

            // Works when changing TargetFramework for JobLib and OTelHttpRepro projects from net6.0 to net7.0 (adjust jobAssemblyPath in Repro method below to net7.0 as well).

            // Also works with net6.0 when running with below code instead of using reflection to load and run the job from a different binaries directory.

            // var job = new JobLib.Job();
            // Task.Run(() => job.RunAsync()).GetAwaiter().GetResult();
            // job.Dispose();
        }

        private static void Repro()
        {
            var currentDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var jobAssemblyPath = Path.Combine(currentDir, "..", "..", "..", "..", "JobLib", "bin", "Debug", "net6.0", "JobLib.dll");

            var loadContext = new CustomLoadContext(jobAssemblyPath);

            var jobAssembly = loadContext.LoadFromAssemblyPath(jobAssemblyPath);
            var jobType = jobAssembly.GetType("JobLib.Job");
            var job = Activator.CreateInstance(jobType);

            var runMethodInfo = jobType.GetMethod("RunAsync");

            Task.Run(async () =>
            {
                var obj = runMethodInfo.Invoke(job, null);

                if (obj is Task task && task != null)
                {
                    await task.ConfigureAwait(false);
                }
                else
                {
                    throw new ArgumentException($"Invalid type {obj?.GetType()}");
                }
            }).GetAwaiter().GetResult();

            var disposeMethodInfo = jobType.GetMethod("Dispose");
            disposeMethodInfo.Invoke(job, null);
        }
    }
}