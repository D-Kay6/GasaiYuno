﻿using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.AutoChannels;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterMediatR(Assembly.GetExecutingAssembly());

        builder.RegisterType<AutoChannelListener>().As<IListener>().InstancePerLifetimeScope();
    }
}