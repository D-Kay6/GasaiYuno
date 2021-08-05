using Autofac;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;

namespace GasaiYuno.Logging.Serilog
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterSerilog(this ContainerBuilder builder)
        {
            var configuration = new LoggerConfiguration()
                .WriteTo.Async(x => x.Console())
                .WriteTo.Async(x => x.File("logs/log.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Async(x => x.Seq("http://192.168.1.2:5341"))
                .Destructure.ToMaximumDepth(2);

            builder.RegisterSerilog(configuration);
            return builder;
        }
    }
}