using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using positive_pay_app.app;
using positive_pay_app.app.jobs;
using positive_pay_app.app.jobs.impl;
using positive_pay_app.app.Sftp;

namespace positive_pay_app
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, args);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var appConfig = serviceProvider.GetService<AppConfig>();
            var chaseGPService = serviceProvider.GetService<ChaseGPPickUpService>();
            
            chaseGPService?.RunFilesMigration();

        }

        private static void ConfigureServices(IServiceCollection services, string[] args)
        {

            var environmentName = Environment.GetEnvironmentVariable("APP_ENV") ?? args[0];
            Console.WriteLine("Loading Env {0} ", environmentName);

            IConfiguration builder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddJsonFile($"appsettings.{environmentName}.json")
              .AddEnvironmentVariables().Build();

            var logger = LoggerFactory.Create(loggerBuilder => { loggerBuilder.ClearProviders().AddConfiguration(builder.GetSection("Logging")).AddConsole(); }).CreateLogger("PositivePay");

            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<EmailService>();
            services.AddSingleton<ChaseGPPickUpService>();

            //services.AddSingleton<IProcessorJob, ChaseSftpServiceJob>();
            services.AddSingleton<IProcessorJob, WindsorServiceJob>();

            var appConfig = builder.GetSection(nameof(AppConfig)).Get<AppConfig>()!;
            services.AddSingleton<AppConfig>(appConfig);

            if (appConfig == null)
            {
                throw new Exception("No configurations provided");
            }

            if (appConfig.ChaseGPSharedFolder == null)
            {
                throw new Exception("No configurations provided for ChaseGp Windows Shared Folder");
            }

            if (appConfig.WindsorSFTPDetails == null)
            {
                throw new Exception("No configurations provided for Windsor Sftp Server");
            }

            if (appConfig.WindsorSFTPDetails == null)
            {
                throw new Exception("No configurations provided for Windsor Sftp Server");
            }

            var chaseSftpConfig = new SftpConfig
            {
                Host = appConfig.ChaseSFTPDetails.Host,
                Port = appConfig.ChaseSFTPDetails.Port,
                UserName = appConfig.ChaseSFTPDetails.User,
                Password = appConfig.ChaseSFTPDetails.Password,
                Directory = appConfig.ChaseSFTPDetails.Directory,
            };

            var chaseSftpService = new SftpService(logger, chaseSftpConfig);
            services.AddKeyedSingleton<SftpService>("ChaseSFTP", chaseSftpService);

            var windorSftpConfig = new SftpConfig
            {
                Host = appConfig.WindsorSFTPDetails.Host,
                Port = appConfig.WindsorSFTPDetails.Port,
                UserName = appConfig.WindsorSFTPDetails.User,
                Password = appConfig.WindsorSFTPDetails.Password,
                Directory = appConfig.WindsorSFTPDetails.Directory,
            };

            var windsorSftpService = new SftpService(logger, windorSftpConfig);
            services.AddKeyedSingleton<SftpService>("WindsorSFTP", windsorSftpService);

        }
    }

}
