using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("poll", "Create new polls for users to answer.")]
    [RequireOwner(Group = "Permission")]
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    public class PollModule : BaseInteractionModule<PollModule>
    {
        public class PollModal : IModal
        {
            /// <inheritdoc />
            public string Title => "New poll";

            [InputLabel("Description")]
            [ModalTextInput("poll_description", TextInputStyle.Short, maxLength: 500)]
            public string Description { get; set; }

            [InputLabel("Options, each on a new line.")]
            [ModalTextInput("poll_options", TextInputStyle.Paragraph)]
            public string Options { get; set; }
        }

        [SlashCommand("new", "Create a new poll.")]
        public Task NewPollCommand(
            [Summary(description: "The time until the result needs to be posted. Example: 4d3h2m -> 4 days, 3 hours, 2 minutes")] TimeSpan duration,
            [Summary(description: "Whether or not the poll allows for multiple options to be selected.")] bool multiSelect = false)
            => RespondWithModalAsync<PollModal>($"poll_form:{duration.TotalSeconds}|{multiSelect}");

        [ModalInteraction("poll_form:*|*", true)]
        public async Task PollResponse(string seconds, string multiSelect, PollModal modal)
        {
            var duration = TimeSpan.FromSeconds(double.Parse(seconds));
            var allowMultiple = bool.Parse(multiSelect);
            var options = modal.Options.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (options.Length <= 1)
                throw new InvalidOperationException("Cannot create a poll with only one or fewer options.");

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Automation.Poll.Title"));
            embedBuilder.WithDescription(modal.Description);
            embedBuilder.WithFooter(Translation.Message("Automation.Poll.EndDate"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var selectMenuBuilder = new SelectMenuBuilder()
                .WithCustomId($"poll_selection:{Context.Interaction.Id}")
                .WithPlaceholder(Translation.Message(allowMultiple ? "Automation.Poll.Selector.Multiple" : "Automation.Poll.Selector.Single"))
                .WithMinValues(1)
                .WithMaxValues(allowMultiple ? options.Length : 1);
            for (var i = 0; i < options.Length; i++)
            {
                selectMenuBuilder.AddOption(options[i], i.ToString());
            }

            await RespondAsync(embed: embedBuilder.Build(), components: new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build()).ConfigureAwait(false);
            var selectionMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            var poll = new Poll
            {
                Id = Context.Interaction.Id,
                Server = Server,
                Channel = Context.Channel.Id,
                Message = selectionMessage.Id,
                EndDate = DateTime.Now + duration,
                Text = modal.Description,
                Options = options.Select(x => new PollOption{ Value = x }).ToList()
            };
            UnitOfWork.Polls.Add(poll);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        [ComponentInteraction("poll_selection:*", true)]
        public async Task PollSelection(string reference, string[] selectedOptions)
        {
            var referenceId = ulong.Parse(reference);
            var selections = selectedOptions.Select(int.Parse).ToList();
            var poll = await UnitOfWork.Polls.GetAsync(referenceId).ConfigureAwait(false);
            if (poll == null) return;

            var selection = poll.Selections.FirstOrDefault(x => x.User == Context.User.Id);
            if (selection != null) poll.Selections.RemoveAll(x => x.User == Context.User.Id);

            foreach (var selectedOption in selections)
            {
                poll.Selections.Add(new PollSelection
                {
                    User = Context.User.Id,
                    SelectedOption = selectedOption
                });
            }
            UnitOfWork.Polls.Update(poll);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }
    }
}