using Autofac;
using Autofac.Extensions.DependencyInjection;
using GasaiYuno.Chatbot.Cleverbot.Extensions;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Interface.Bot;
using GasaiYuno.Listing.Discord.Extensions;
using GasaiYuno.Localization.Extensions;
using GasaiYuno.Logging.Serilog;
using GasaiYuno.Storage.Configuration.Extensions;
using GasaiYuno.Storage.Image.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GasaiYuno
{
    internal class Program
    {
        private static IConnection _connection;

        private static async Task Main(string[] args)
        {
            Console.Title = "Gasai Yuno - Discord";
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            DownloadPrerequisites();

            var container = GenerateContainer();

            do
            {
                await using var scope = container.BeginLifetimeScope();
                _connection = scope.Resolve<IConnection>();
                await _connection.ConnectAsync().ConfigureAwait(false);
            } while (_connection.Restart);
        }

        private static async void OnExit(object sender, EventArgs e)
        {
            await _connection.DisconnectAsync().ConfigureAwait(false);
        }

        private static void DownloadPrerequisites()
        {
            using var client = new WebClient();
            var file = "libsodium.dll";
            if (!File.Exists(file))
                client.DownloadFile("https://discord.foxbot.me/binaries/win64/libsodium.dll", file);

            file = "opus.dll";
            if (!File.Exists(file)) client.DownloadFile("https://discord.foxbot.me/binaries/win64/opus.dll", file);
        }

        private static IContainer GenerateContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(Enumerable.Empty<ServiceDescriptor>());
            containerBuilder.RegisterDiscord();
            containerBuilder.RegisterLocalization();
            containerBuilder.RegisterSerilog();
            containerBuilder.RegisterCleverbot();
            containerBuilder.RegisterListing();
            containerBuilder.RegisterConfigStorage();
            containerBuilder.RegisterImageStorage();

            return containerBuilder.Build();
        }
    }
}