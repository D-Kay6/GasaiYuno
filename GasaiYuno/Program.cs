using Autofac;
using Autofac.Extensions.DependencyInjection;
using GasaiYuno.Chatbot.Cleverbot.Extensions;
using GasaiYuno.Discord.Modules;
using GasaiYuno.Listing.Discord.Extensions;
using GasaiYuno.Localization.Extensions;
using GasaiYuno.Storage.Configuration.Extensions;
using GasaiYuno.Storage.Image.Extensions;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GasaiYuno
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.Title = "Gasai Yuno - Discord";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                DownloadPrerequisites();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Unable to download prerequisites. Application cannot run without. Manual downloading required.");
                Log.CloseAndFlush();
                return;
            }

            try
            {
                Log.Information("Booting up...");
                var host = CreateHostBuilder(args).Build();
                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.RegisterModule(new DiscordModule(context.Configuration));
                    builder.RegisterLocalization();
                    builder.RegisterCleverbot();
                    builder.RegisterListing();
                    builder.RegisterConfigStorage();
                    builder.RegisterImageStorage();
                })
                .UseConsoleLifetime();

        private static void DownloadPrerequisites()
        {
            using var client = new WebClient();
            var file = "libsodium.dll";
            if (!File.Exists(file))
                client.DownloadFile("https://discord.foxbot.me/binaries/win64/libsodium.dll", file);

            file = "opus.dll";
            if (!File.Exists(file)) client.DownloadFile("https://discord.foxbot.me/binaries/win64/opus.dll", file);
        }
    }
}