using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime.Loader;

namespace GasaiYuno;

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
            CheckPrerequisites();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Unable to download prerequisites. Application cannot run without. Manual downloading required");
            Log.CloseAndFlush();
            return;
        }
        
        var executionFolder = Path.GetDirectoryName(AppContext.BaseDirectory);
        AssemblyLoadContext.Default.Resolving += (context, assembly) => context.LoadFromAssemblyPath(Path.Combine(executionFolder ?? string.Empty, $"{assembly.Name}.dll"));

        try
        {
            Log.Information("Booting up...");
            await Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog((context, _, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration))
                .ConfigureServices(ConfigureServices)
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }


    private static void ConfigureServices(IServiceCollection services)
    {
    }

    private static void ConfigureContainer(HostBuilderContext hostBuilderContext, ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterModule(new ConfigurationModule(hostBuilderContext.Configuration));
    }

    private static void CheckPrerequisites()
    {
        var file = "libsodium.dll";
        if (!File.Exists(file))
            throw new FileNotFoundException("The required dependency could not be found.", file);

        file = "opus.dll";
        if (!File.Exists(file))
            throw new FileNotFoundException("The required dependency could not be found.", file);
    }
}