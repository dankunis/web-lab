using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database;
using Homework1.Models.Requests;
using Homework1.Test.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Homework1.Test
{
    public class ApiFixture : IDisposable
    {
        public TestServer Server { get; set; }
        public HttpClient Client { get; set; }

        public ApiFixture()
        {
            var builder = SetupWebHost();
            Server = new TestServer(builder);
            Client = Server.CreateClient();

            if (Debugger.IsAttached)
                Client.Timeout = TimeSpan.FromHours(1);

            Client.CreateDefaultCookies().Wait();
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }

        private static IWebHostBuilder SetupWebHost()
        {
            return new WebHostBuilder()
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddJsonFile(ConfigDirectoryPath());
                })
                .UseEnvironment("Development")
                .UseStartup<Startup>();
        }

        private static string ConfigDirectoryPath()
        {
            var exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(exeLocation), "../../../Configuration/config.json");
        }
    }
}