using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using Interactivity;
using Interactivity.Pagination;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Extensions
{
    public static class InteractivityExtensions
    {
        public static async Task SendPaginatorComponentAsync(
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
                await message.ModifyAsync(x =>
                {
                    x.Components = componentBuilder.Build();
                    x.Content = page.Text;
                    x.Embed = page.Embed;
                }).ConfigureAwait(false);
            }
            else message = await channel.SendMessageAsync(page.Text, false, page.Embed, component: componentBuilder.Build()).ConfigureAwait(false);

            try
            {
                client.InteractionCreated += HandleInteractionAsync;
                var isCancelled = await Task.WhenAny(timeoutTask, cancelSource.Task).ConfigureAwait(false) != timeoutTask;
                if (paginator.Deletion.HasFlag(DeletionOptions.AfterCapturedContext)) 
                    await message.DeleteAsync().ConfigureAwait(false);
                else if (isCancelled && paginator.CancelledEmbed != null)
                    await message.ModifyAsync(x =>
                    {
                        x.Components = null;
                        x.Embed = paginator.CancelledEmbed;
                        x.Content = null;
                    }).ConfigureAwait(false);
                else if (!isCancelled && paginator.TimeoutedEmbed != null)
                    await message.ModifyAsync(x =>
                    {
                        x.Components = null;
                        x.Embed = paginator.TimeoutedEmbed;
                        x.Content = null;
                    }).ConfigureAwait(false);
            }
            finally
            {
                client.InteractionCreated -= HandleInteractionAsync;
                cancellationRegistration.Dispose();
                timeoutProvider.Dispose();
            }

            async Task HandleInteractionAsync(SocketInteraction i)
            {
                if (i is not SocketMessageComponent socketMessageComponent) return;
                if (runOnGateway ?? interactivityService.RunOnGateway) await CheckInteractionAsync(socketMessageComponent);
                else Task.Run(() => CheckInteractionAsync(socketMessageComponent));
            }

            async Task CheckInteractionAsync(SocketMessageComponent messageComponent)
            {
                if ((long)messageComponent.Message.Id != (long)message.Id || (long)messageComponent.User.Id == (long)client.CurrentUser.Id) return;
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

                    var page = await paginator.GetOrLoadCurrentPageAsync().ConfigureAwait(false);
                    await message.ModifyAsync(x =>
                    {
                        x.Embed = page.Embed;
                        x.Content = page.Text;
                    }).ConfigureAwait(false);
                }
            }
        }
        
        public static Task<bool> HandleInteractionAsync(this Paginator paginator, SocketMessageComponent messageComponent, out PaginatorAction action)
        {
            if (!Enum.TryParse(messageComponent.Data.CustomId, false, out action)) action = PaginatorAction.Exit;
            return Task.FromResult(!paginator.IsUserRestricted || paginator.Users.Any(x => (long)x.Id == (long)messageComponent.User.Id));
        }
    }
}