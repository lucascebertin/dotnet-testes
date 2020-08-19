using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Sample.Testes.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = CreateConfiguration(envName, args);

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                logger.Information("App starting");
                using var host = CreateHostBuilder(args, logger).Build();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "App crashed");
                Serilog.Log.CloseAndFlush();
            }
        }

        public static IConfiguration CreateConfiguration(string environment, string[] args = null) =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

        public static IHostBuilder CreateHostBuilder(string[] args, Serilog.ILogger logger,
            EventHandler afterMessageConsumed = null) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .UseSerilog(logger)
                .ConfigureHostConfiguration(config => config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("hostsettings.json", true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, config) =>
                    CreateConfiguration(context.HostingEnvironment.EnvironmentName, args))
                .ConfigureServices((context, services) => services
                    .AddHostedService<BackgroundHostedService>()
                );
    }

    internal class BackgroundHostedService : BackgroundService
    {
        private readonly ILogger<BackgroundHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Stopwatch _stopwatch;

        public BackgroundHostedService(
            ILogger<BackgroundHostedService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _stopwatch = Stopwatch.StartNew();
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting.");

            this.StopAsync(default);
            return base.StartAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping.");

            return base.StopAsync(cancellationToken);
        }

        private long Stop()
        {
            var elaped = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Stop();

            return elaped;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //Add here the business rules.       
                _logger.LogInformation("Iniciando processamento...");
                _logger.LogInformation("Fim de processamento!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar!");
            }

            await Task.CompletedTask;
        }
    }
}
