using System;
using Microsoft.Extensions.Hosting;
using Sample.Testes.App;
using Serilog;
using WireMock.Server;

namespace Sample.Testes.Aceitacao.Infra
{
    public class TestsFixture : IDisposable
    {
        public readonly IHost App;
        public readonly WireMockServer Server;

        public TestsFixture()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = Program.CreateConfiguration(envName, new string[0]);

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            App = Program
                .CreateHostBuilder(new string[0], logger)
                .UseEnvironment("Testing")
                .Build();

            Server = WireMockServer.Start();
            App.Start();
        }

        public void Dispose()
        {
            Server.Stop();
            App.StopAsync(TimeSpan.FromSeconds(15)).GetAwaiter().GetResult();
            App.Dispose();
        }
    }
}

