using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("raffle", "Create a new raffle for users to join.")]
    [RequireOwner(Group = "Permission")]
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    public class RaffleModule : BaseInteractionModule<RaffleModule>
    {
        public class RaffleModal : IModal
        {
            /// <inheritdoc />
            public string Title => "New raffle";

            [InputLabel("Title")]
            [ModalTextInput("raffle_title", TextInputStyle.Short, maxLength: 500)]
            public string Header { get; set; }

            [InputLabel("Description")]
            [ModalTextInput("raffle_description", TextInputStyle.Paragraph)]
            public string Description { get; set; }
        }

        [SlashCommand("new", "Create a new raffle for users to join.")]
        public Task NewRaffleCommand([Summary(description: "The time until the result need to be posted. Example: 4d3h2m -> 4 days, 3 hours, 2 minutes")] TimeSpan duration)
            => RespondWithModalAsync<RaffleModal>($"raffle_form:{duration.TotalSeconds}");

        [ModalInteraction("raffle_form:*", true)]
        public async Task RaffleResponse(string seconds, RaffleModal modal)
        {
            var duration = TimeSpan.FromSeconds(double.Parse(seconds));

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(modal.Header);
            embedBuilder.WithDescription($"{modal.Description}\n\n{Translation.Message("Automation.Raffle.Footer")}");
            embedBuilder.WithFooter(Translation.Message("Automation.Raffle.EndDate"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var componentBuilder = new ComponentBuilder()
                .WithButton("Enter", $"raffle_interaction:{Context.Interaction.Id}");

            await RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build()).ConfigureAwait(false);
            var interactionMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            var raffle = new Raffle
            {
                Id = Context.Interaction.Id,
                Server = Server,
                Channel = Context.Channel.Id,
                Message = interactionMessage.Id,
                Title = modal.Header,
                EndDate = DateTime.Now + duration
            };
            UnitOfWork.Raffles.Add(raffle);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        [ComponentInteraction("raffle_interaction:*", true)]
        public async Task RaffleInteraction(string reference)
        {
            var referenceId = ulong.Parse(reference);
            var raffle = await UnitOfWork.Raffles.GetAsync(referenceId).ConfigureAwait(false);
            if (raffle == null) return;
            if (raffle.Entries.Any(x => x.User == Context.User.Id)) return;

            raffle.Entries.Add(new RaffleEntry{User = Context.User.Id });
            UnitOfWork.Raffles.Update(raffle);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }
    }
}