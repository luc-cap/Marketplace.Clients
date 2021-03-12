using System;
using System.Threading.Tasks;
using Ecomdash.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace Ecomdash.Client
{
	class Program
	{
		static async Task Main(string[] args)
		{
			try
			{
				var host = GetHost();

				using (var serviceScope = host.Services.CreateScope())
				{  
					IServiceProvider serviceProvider = serviceScope.ServiceProvider;
					 
					// Gli Integration Services
					await serviceProvider.GetService<IIntegrationService>().Run(); 
				}
			}
			catch (Exception ex)
			{ 
				// Per loggare anche errori sull'IOC --> prendo il Logger con il LogManger di NLog.
				Logger logger = LogManager.GetCurrentClassLogger();
				logger.Error(ex, "An exception happened while running the integration service.");
			}
			finally
			{
				NLog.LogManager.Shutdown();
			}

			Console.ReadKey();
		} 


		private static IHost GetHost()
		{
			var hostBuilder = new HostBuilder()
			  .ConfigureServices((hostContext, services) =>
			  { 
				  // Configurazione dei servizi
				  ConfigureServices(services); 
			  })
			  .ConfigureLogging(logBuilder =>
			  {
				  logBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
				  // aggiungo NLog
				  logBuilder.AddNLog();
			  })
			  .UseConsoleLifetime();

			return hostBuilder.Build();
		}

		private static void ConfigureServices(IServiceCollection serviceCollection)
		{
			// specifico qual'è il file con la configurazione di NLog
			IConfigurationRoot configuration = new ConfigurationBuilder()
			   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
			   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			   .Build();

			// aggiungo la configurazione di NLog
			NLog.LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));
			serviceCollection.AddSingleton<IConfiguration>(configuration);

			// register the integration service on our container with a 
			// scoped lifetime

			// For the CRUD Call
			serviceCollection.AddScoped<IIntegrationService, CRUDService>();

			// For the partial update demos
			// serviceCollection.AddScoped<IIntegrationService, PartialUpdateService>();

			// For the stream demos
			// serviceCollection.AddScoped<IIntegrationService, StreamService>();

			// For the cancellation demos
			// serviceCollection.AddScoped<IIntegrationService, CancellationService>();

			// For the HttpClientFactory demos
			// serviceCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();

			// For the dealing with errors and faults demos
			// serviceCollection.AddScoped<IIntegrationService, DealingWithErrorsAndFaultsService>();

			// For the custom http handlers demos
			// serviceCollection.AddScoped<IIntegrationService, HttpHandlersService>();     
		}
	}
}
