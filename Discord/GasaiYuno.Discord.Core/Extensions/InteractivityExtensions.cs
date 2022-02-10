using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Models;
using Interactivity;
using Interactivity.Pagination;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Extensions
{
    public static class InteractivityExtensions
    {
        public static async Task SendPaginatorAsync(
            this InteractivityService interactivityService,
            BaseSocketClient client,
            Paginator paginator,
            IMessageChannel channel,
            TimeSpan? timeout = null,
            IUserMessage message = null,
            bool? runOnGateway = null,
            CancellationToken cancellationToken = default)
        {
            var cancelSource = new TaskCompletionSource<bool>();
            var cancellationRegistration = cancellationToken.Register(() => cancelSource.SetResult(true));
            var timeoutProvider = new TimeoutProvider(timeout ?? interactivityService.DefaultTimeout);
            var timeoutTask = timeoutProvider.WaitAsync();

            var page = await paginator.GetOrLoadCurrentPageAsync().ConfigureAwait(false);
            var componentBuilder = new ComponentBuilder()
                .WithButton("<<", PaginatorAction.SkipToStart.ToString(), ButtonStyle.Secondary)
                .WithButton("<", PaginatorAction.Backward.ToString(), ButtonStyle.Secondary)
                .WithButton(">", PaginatorAction.Forward.ToString(), ButtonStyle.Secondary)
                .WithButton(">>", PaginatorAction.SkipToEnd.ToString(), ButtonStyle.Secondary)
                .WithButton("X", PaginatorAction.Exit.ToString(), ButtonStyle.Danger);
            if (message != null)
            {
                if (message.Author.Id != client.CurrentUser.Id)
                    throw new ArgumentException("Message author not current user!");

                await message.ModifyAsync(x =>
                {
                    x.Content = page.Text;
                    x.Embed = page.Embed;
                }).ConfigureAwait(false);
            }
            else
                message = await channel.SendMessageAsync(page.Text, false, page.Embed, components: componentBuilder.Build()).ConfigureAwait(false);

            try
            {
                client.InteractionCreated += InteractionCreatedAsync;
                var isCancelled = await Task.WhenAny(timeoutTask, cancelSource.Task).ConfigureAwait(false) != timeoutTask;
                if (paginator.Deletion.HasFlag(DeletionOptions.AfterCapturedContext))
                    await message.DeleteAsync().ConfigureAwait(false);
                else if (isCancelled && paginator.CancelledEmbed != null)
                {
                    await message.ModifyAsync(x =>
                    {
                        x.Embed = paginator.CancelledEmbed;
                        x.Content = null;
                    }).ConfigureAwait(false);
                }
                else if (!isCancelled && paginator.TimeoutedEmbed != null)
                {
                    await message.ModifyAsync(x =>
                    {
                        x.Embed = paginator.TimeoutedEmbed;
                        x.Content = null;
                    }).ConfigureAwait(false);
                }
            }
            finally
            {
                client.InteractionCreated -= InteractionCreatedAsync;
                cancellationRegistration.Dispose();
                timeoutProvider.Dispose();
            }

            async Task InteractionCreatedAsync(SocketInteraction interaction)
            {
                if (interaction is not SocketMessageComponent messageComponent) return;
                if (runOnGateway ?? interactivityService.RunOnGateway) await CheckInteractionAsync(messageComponent);
                else Task.Run(() => CheckInteractionAsync(messageComponent));
            }

            async Task CheckInteractionAsync(SocketMessageComponent messageComponent)
            {
                if (messageComponent.Message.Id != message.Id || messageComponent.User.Id == client.CurrentUser.Id) return;
                if (!await paginator.HandleInteractionAsync(messageComponent, out var action).ConfigureAwait(false)) return;

                await messageComponent.DeferAsync(true);
                if (action == PaginatorAction.Exit)
                {
                    cancelSource.SetResult(true);
                }
                else
                {
                    timeoutProvider.Reset();
                    if (!await paginator.ApplyActionAsync(action).ConfigureAwait(false)) return;

                    var newPage = await paginator.GetOrLoadCurrentPageAsync().ConfigureAwait(false);
                    await message.ModifyAsync(x =>
                    {
                        x.Embed = newPage.Embed;
                        x.Content = newPage.Text;
                    }).ConfigureAwait(false);
                }
            }
        }

        private static Task<bool> HandleInteractionAsync(this Paginator paginator, SocketMessageComponent messageComponent, out PaginatorAction action)
        {
            if (!Enum.TryParse(messageComponent.Data.CustomId, false, out action)) action = PaginatorAction.Exit;
            return Task.FromResult(!paginator.IsUserRestricted || paginator.Users.Any(x => (long)x.Id == (long)messageComponent.User.Id));
        }

        public static async Task<SocketMessageComponent> NextMessageComponentAsync(
            this InteractivityService interactivityService,
            BaseSocketClient client,
            Predicate<SocketMessageComponent> filter = null,
            Func<SocketMessageComponent, bool, Task> actions = null,
            TimeSpan? timeout = null,
            bool? runOnGateway = null,
            CancellationToken cancellationToken = default)
        {
            if (actions == null)
                actions = (s, v) => Task.CompletedTask;
            if (filter == null)
                filter = s => true;

            var componentSource = new TaskCompletionSource<SocketMessageComponent>();
            var cancelSource = new TaskCompletionSource<bool>();
            var cancellationRegistration = cancellationToken.Register(() => cancelSource.SetResult(true));
            var componentTask = componentSource.Task;
            var timeoutTask = Task.Delay(timeout ?? interactivityService.DefaultTimeout);

            SocketMessageComponent result = null;
            try
            {
                client.InteractionCreated += InteractionCreatedAsync;
                var task2 = await Task.WhenAny(componentTask, timeoutTask, cancelSource.Task).ConfigureAwait(false);
                if (task2 == componentTask)
                {
                    result = await componentTask.ConfigureAwait(false);
                }
            }
            finally
            {
                client.InteractionCreated -= InteractionCreatedAsync;
                cancellationRegistration.Dispose();
            }
            return result;

            async Task InteractionCreatedAsync(SocketInteraction interaction)
            {
                if (interaction is not SocketMessageComponent component) return;
                if (runOnGateway ?? interactivityService.RunOnGateway) await CheckComponentAsync(component);
                else Task.Run(() => CheckComponentAsync(component));
            }

            async Task CheckComponentAsync(SocketMessageComponent s)
            {
                if (!filter(s))
                {
                    await actions(s, true).ConfigureAwait(false);
                }
                else
                {
                    await actions(s, false).ConfigureAwait(false);
                    componentSource.SetResult(s);
                }
            }
        }
    }
}