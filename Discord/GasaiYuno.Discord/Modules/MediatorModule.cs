using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Modules
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(GetAssemblies());
        }

        private List<Assembly> GetAssemblies()
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies().Where(x => !loadedAssemblies.Contains(x.FullName) && (x.Name?.StartsWith("GasaiYuno.") ?? false)))
                {
                    if (loadedAssemblies.Contains(reference.FullName)) continue;

                    var assembly = Assembly.Load(reference);
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    returnAssemblies.Add(assembly);
                }
            }

            return returnAssemblies;
        }
    }
}