using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Raffles.Mediator.Commands;

namespace GasaiYuno.Discord.Raffles.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageEvents)]
[Group("raffle", "Create a new raffle for users to join.")]
public class RaffleModule : BaseInteractionModule<RaffleModule>
{
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [SlashCommand("new", "Create a new raffle for users to join.")]
    public Task NewRaffleCommand([Summary(description: "The time until the result need to be posted. Example: 4d3h2m -> 4 days, 3 hours, 2 minutes")] TimeSpan duration)
        => RespondWithModalAsync<RaffleModal>($"raffle form:{duration.TotalSeconds}");

    public class RaffleModal : IModal
    {
        /// <inheritdoc />
        public string Title => "New raffle";

        [InputLabel("Title")]
        [ModalTextInput("form title", TextInputStyle.Short, maxLength: 256)]
        public string Header { get; set; }

        [InputLabel("Description")]
        [ModalTextInput("form description", TextInputStyle.Paragraph, maxLength: 256)]
        public string Description { get; set; }
    }

    public class RaffleModalModule : BaseInteractionModule<RaffleModalModule, SocketModal>
    {
        [ModalInteraction("form:*")]
        public async Task RaffleResponse(string seconds, RaffleModal modal)
        {
            var duration = TimeSpan.FromSeconds(double.Parse(seconds));

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(modal.Header);
            embedBuilder.WithDescription($"{modal.Description}\n\n{Localization.Translate("Automation.Raffle.Footer")}");
            embedBuilder.WithFooter(Localization.Translate("Automation.Raffle.EndDate"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var reference = Guid.NewGuid();
            var componentBuilder = new ComponentBuilder()
                .WithButton("Enter", $"raffle interaction:{reference}");

            await RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build()).ConfigureAwait(false);
            var interactionMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            await Mediator.Publish(new AddRaffleCommand(reference, Context.Guild.Id, Context.Channel.Id, interactionMessage.Id, modal.Header, DateTime.Now + duration)).ConfigureAwait(false);
        }
    }

    public class RaffleComponentModule : BaseInteractionModule<RaffleComponentModule, SocketMessageComponent>
    {
        [ComponentInteraction("interaction:*")]
        public async Task RaffleInteraction(string reference)
        {
            var referenceId = Guid.Parse(reference);
            await Mediator.Publish(new AddRaffleEntryCommand(referenceId, Context.User.Id)).ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }
    }
}