using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebJob.SDK
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
                b.AddTimers();
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
