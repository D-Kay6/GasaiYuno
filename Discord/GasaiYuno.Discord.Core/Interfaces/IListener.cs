﻿using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Interfaces;

public interface IListener : IAsyncDisposable
{
    int Priority { get; }
    Task Start();
}