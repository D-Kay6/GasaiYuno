using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Polls.Mediator.Commands;

namespace GasaiYuno.Discord.Polls.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageEvents)]
[Group("poll", "Create new polls for users to answer.")]
public class PollModule : BaseInteractionModule<PollModule>
{
    [RequireUserPermission(GuildPermission.ManageEvents)]
    [SlashCommand("new", "Create a new poll.")]
    public Task NewPollCommand(
        [Summary(description: "The time until the result needs to be posted. Example: 4d3h2m -> 4 days, 3 hours, 2 minutes")] TimeSpan duration,
        [Summary(description: "Whether or not the poll allows for multiple options to be selected.")] bool multiSelect = false)
        => RespondWithModalAsync<PollModal>($"poll form:{duration.TotalSeconds}|{multiSelect}");

    public class PollModal : IModal
    {
        /// <inheritdoc />
        public string Title => "New poll";

        [InputLabel("Description")]
        [ModalTextInput("form description", TextInputStyle.Paragraph, maxLength: 256)]
        public string Description { get; set; }

        [InputLabel("Options, each on a new line.")]
        [ModalTextInput("form options", TextInputStyle.Paragraph)]
        public string Options { get; set; }
    }

    public class PollModalModule : BaseInteractionModule<PollModalModule, SocketModal>
    {
        [ModalInteraction("form:*|*")]
        public async Task PollResponse(string seconds, string multiSelect, PollModal modal)
        {
            var duration = TimeSpan.FromSeconds(double.Parse(seconds));
            var allowMultiple = bool.Parse(multiSelect);
            var options = modal.Options.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (options.Length <= 1)
                throw new InvalidOperationException("Cannot create a poll with only one or fewer options.");

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Localization.Translate("Automation.Poll.Title"));
            embedBuilder.WithDescription(modal.Description);
            embedBuilder.WithFooter(Localization.Translate("Automation.Poll.EndDate"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var reference = Guid.NewGuid();
            var selectMenuBuilder = new SelectMenuBuilder()
                .WithCustomId($"poll selection:{reference}")
                .WithPlaceholder(Localization.Translate(allowMultiple ? "Automation.Poll.Selector.Multiple" : "Automation.Poll.Selector.Single"))
                .WithMinValues(1)
                .WithMaxValues(allowMultiple ? options.Length : 1);
            for (var i = 0; i < options.Length; i++)
            {
                selectMenuBuilder.AddOption(options[i], i.ToString());
            }

            await RespondAsync(embed: embedBuilder.Build(), components: new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build()).ConfigureAwait(false);
            var selectionMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            await Mediator.Publish(new AddPollCommand(reference, Context.Guild.Id, Context.Channel.Id, selectionMessage.Id, DateTime.Now + duration, modal.Description, options)).ConfigureAwait(false);
        }
    }

    public class PollComponentModule : BaseInteractionModule<PollComponentModule, SocketMessageComponent>
    {
        [ComponentInteraction("selection:*")]
        public async Task PollSelection(string reference, string[] selectedOptions)
        {
            var referenceId = Guid.Parse(reference);
            var selections = selectedOptions.Select(int.Parse).ToArray();
            await Mediator.Publish(new AddPollSelectionsCommand(referenceId, Context.User.Id, selections)).ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }
    }
}