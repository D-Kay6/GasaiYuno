using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation;

[EnabledInDm(false)]
[DefaultPermission(false)]
[DefaultMemberPermissions(GuildPermission.ManageRoles)]
[Group("role-distributor", "Manage the automated role distribution of message interactions.")]
public class DistributionRoleModule : BaseInteractionModule<DistributionRoleModule>
{
    [RequireUserPermission(GuildPermission.ManageRoles)]
    [SlashCommand("button", "Create a message that gives roles when the button is pressed.")]
    public Task ButtonRoleDistributorCommand() => RespondWithModalAsync<DistributionRoleButtonModal>("role-distributor button_form");

    [RequireUserPermission(GuildPermission.ManageRoles)]
    [SlashCommand("list", "Create a message where users can select roles from a list.")]
    public Task ListRoleDistributorCommand() => RespondWithModalAsync<DistributionRoleListModal>("role-distributor list_form");

    public class DistributionRoleButtonModal : IModal
    {
        /// <inheritdoc />
        public string Title => "New role distribution";

        [InputLabel("Description")]
        [ModalTextInput("button_form description", TextInputStyle.Paragraph, "A descriptive message for the users.")]
        public string Description { get; set; }

        [InputLabel("Button text")]
        [ModalTextInput("button_form label", TextInputStyle.Short, "The text to display on the button.", maxLength: 100)]
        public string ButtonLabel { get; set; }
    }

    public class DistributionRoleListModal : IModal
    {
        /// <inheritdoc />
        public string Title => "New role distribution";

        [InputLabel("Description")]
        [ModalTextInput("list_form description", TextInputStyle.Paragraph, "A descriptive message for the users.")]
        public string Description { get; set; }

        [InputLabel("Minimum selected (optional)")]
        [ModalTextInput("list_form min_selected", TextInputStyle.Short, "Minimum amount of roles users need to select (0 to 25).", 0, 2)]
        [RequiredInput(false)]
        public string MinSelected { get; set; }

        [InputLabel("Maximum selected (optional)")]
        [ModalTextInput("list_form max_selected", TextInputStyle.Short, "Maximum amount of roles users can select (1 to 25).", 0, 2)]
        [RequiredInput(false)]
        public string MaxSelected { get; set; }
    }

    public class DistributionRoleModalModule : BaseInteractionModule<DistributionRoleModalModule, SocketModal>
    {
        [ModalInteraction("button_form")]
        public async Task DistributorButtonResponse(DistributionRoleButtonModal modal)
        {
            if (string.IsNullOrWhiteSpace(modal.Description)) return;
            if (string.IsNullOrWhiteSpace(modal.ButtonLabel)) return;

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Select the roles to give to users.")
                .WithDescription("Be sure to click outside the dropdown to apply the changes.")
                .AddField("Description", modal.Description);
            await RespondAsync(embed: embedBuilder.Build(), components: CreateRoleSelector("button"), ephemeral: true).ConfigureAwait(false);
            var responseMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            var distributionRole = new DistributionRole
            {
                Server = Context.Guild.Id,
                Channel = Context.Channel.Id,
                Message = responseMessage.Id,
                Description = modal.Description,
                ButtonText = modal.ButtonLabel
            };
            await UnitOfWork.DistributionRoles.AddAsync(distributionRole).ConfigureAwait(false);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        [ModalInteraction("list_form")]
        public async Task DistributorListResponse(DistributionRoleListModal modal)
        {
            if (string.IsNullOrWhiteSpace(modal.Description)) return;

            var minSelected = 0;
            if (!string.IsNullOrWhiteSpace(modal.MinSelected))
            {
                if (!int.TryParse(modal.MinSelected, out minSelected) || minSelected is < 0 or > 25) return;
            }

            var maxSelected = 0;
            if (!string.IsNullOrWhiteSpace(modal.MaxSelected))
            {
                if (!int.TryParse(modal.MaxSelected, out maxSelected) || maxSelected is <= 0 or > 25) return;
                if (minSelected > maxSelected) return;
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Select the roles users are allowed to select from.")
                .WithDescription("Be sure to click outside the dropdown to apply the changes.")
                .AddField("Description", modal.Description);
            await RespondAsync(embed: embedBuilder.Build(), components: CreateRoleSelector("list"), ephemeral: true).ConfigureAwait(false);
            var responseMessage = await GetOriginalResponseAsync().ConfigureAwait(false);
            var distributionRole = new DistributionRole
            {
                Server = Context.Guild.Id,
                Channel = Context.Channel.Id,
                Message = responseMessage.Id,
                Description = modal.Description,
                MinSelected = minSelected,
                MaxSelected = maxSelected
            };
            await UnitOfWork.DistributionRoles.AddAsync(distributionRole).ConfigureAwait(false);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        private MessageComponent CreateRoleSelector(string selectorName)
        {
            var componentBuilder = new ComponentBuilder();
            var optionChunks = Context.Guild.Roles.Where(x => !x.IsEveryone)
                .Select(x => new SelectMenuOptionBuilder(x.Name, x.Id.ToString())).Chunk(25).ToList();
            for (var i = 0; i < optionChunks.Count; i++)
            {
                var optionChunk = optionChunks[i].ToList();
                componentBuilder.WithSelectMenu($"role-distributor {selectorName}_selection:{i}", optionChunk, "Select the roles", 0, optionChunk.Count);
            }

            componentBuilder
                .WithButton("Cancel", $"role-distributor {selectorName}_cancel", ButtonStyle.Danger, row: optionChunks.Count)
                .WithButton("Finish", $"role-distributor {selectorName}_finish", ButtonStyle.Primary, row: optionChunks.Count);

            return componentBuilder.Build();
        }
    }

    public class DistributionRoleComponentModule : BaseInteractionModule<DistributionRoleComponentModule, SocketMessageComponent>
    {
        [ComponentInteraction("button_selection:*")]
        public async Task SelectionButtonInteraction(string selectorId, string[] selectedRoles)
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;

            var rolesToRemove = distributionRole.Roles.Where(x => x.Value == selectorId).Select(x => x.Key);
            foreach (var role in rolesToRemove)
                distributionRole.Roles.Remove(role);

            foreach (var selectedRole in selectedRoles)
                distributionRole.Roles.Add(ulong.Parse(selectedRole), selectorId);

            UnitOfWork.DistributionRoles.Update(distributionRole);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }

        [ComponentInteraction("button_cancel")]
        public async Task CancelButtonInteraction()
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole != null)
            {
                UnitOfWork.DistributionRoles.Remove(distributionRole);
                await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = "Canceled";
                x.Embed = null;
                x.Components = null;
            }).ConfigureAwait(false);
        }

        [ComponentInteraction("button_finish")]
        public async Task FinishButtonInteraction()
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;
            if (distributionRole.Roles.Count == 0)
            {
                await RespondAsync("You need to add at least one role.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            var componentBuilder = new ComponentBuilder().WithButton(distributionRole.ButtonText, "role-distributor button_interaction");
            var message = await ReplyAsync(distributionRole.Description, components: componentBuilder.Build()).ConfigureAwait(false);
            distributionRole.Message = message.Id;
            UnitOfWork.DistributionRoles.Update(distributionRole);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = "Done";
                x.Embed = null;
                x.Components = null;
            }).ConfigureAwait(false);
        }

        [ComponentInteraction("button_interaction")]
        public async Task RoleButtonInteraction()
        {
            if (Context.User is not SocketGuildUser user) return;

            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;

            await user.AddRolesAsync(distributionRole.Roles.Keys).ConfigureAwait(false);
            await RespondAsync(":thumbsup:", ephemeral: true).ConfigureAwait(false);
        }


        [ComponentInteraction("list_selection:*")]
        public async Task SelectionListInteraction(string selectorId, string[] selectedRoles)
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;

            var rolesToRemove = distributionRole.Roles.Where(x => x.Value == selectorId).Select(x => x.Key);
            foreach (var role in rolesToRemove)
                distributionRole.Roles.Remove(role);

            foreach (var selectedRole in selectedRoles)
                distributionRole.Roles.Add(ulong.Parse(selectedRole), selectorId);

            UnitOfWork.DistributionRoles.Update(distributionRole);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await DeferAsync(true).ConfigureAwait(false);
        }

        [ComponentInteraction("list_cancel")]
        public async Task CancelListInteraction()
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole != null)
            {
                UnitOfWork.DistributionRoles.Remove(distributionRole);
                await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = "Canceled";
                x.Embed = null;
                x.Components = null;
            }).ConfigureAwait(false);
        }

        [ComponentInteraction("list_finish")]
        public async Task FinishListInteraction()
        {
            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;
            if (distributionRole.Roles.Count == 0)
            {
                await RespondAsync("You need to add at least one role.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (distributionRole.Roles.Count > 25)
            {
                await RespondAsync("No more than 25 roles are allowed", ephemeral: true).ConfigureAwait(false);
                return;
            }

            var roles = Context.Guild.Roles.Where(x => distributionRole.Roles.ContainsKey(x.Id));
            var options = roles.Select(x => new SelectMenuOptionBuilder(x.Name, x.Id.ToString())).ToList();
            var selectMenuBuilder = new SelectMenuBuilder()
                .WithOptions(options)
                .WithMinValues(distributionRole.MinSelected)
                .WithMaxValues(distributionRole.MaxSelected > 0 ? distributionRole.MaxSelected : options.Count)
                .WithCustomId("role-distributor list_interaction");

            var componentBuilder = new ComponentBuilder().WithSelectMenu(selectMenuBuilder);
            var message = await ReplyAsync(distributionRole.Description, components: componentBuilder.Build()).ConfigureAwait(false);
            distributionRole.Message = message.Id;
            UnitOfWork.DistributionRoles.Update(distributionRole);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Content = "Done";
                x.Embed = null;
                x.Components = null;
            }).ConfigureAwait(false);
        }

        [ComponentInteraction("list_interaction")]
        public async Task RoleListInteraction(string[] selectedRoles)
        {
            if (Context.User is not SocketGuildUser user) return;

            var distributionRole = await UnitOfWork.DistributionRoles.GetAsync(Context.Guild.Id, Context.Channel.Id, Context.Interaction.Message.Id);
            if (distributionRole == null) return;

            var rolesToGive = selectedRoles.Select(ulong.Parse).ToList();
            var rolesToRemove = distributionRole.Roles.Keys.Except(rolesToGive);

            await user.RemoveRolesAsync(rolesToRemove).ConfigureAwait(false);
            await user.AddRolesAsync(rolesToGive).ConfigureAwait(false);
            await RespondAsync(":thumbsup:", ephemeral: true).ConfigureAwait(false);
        }
    }
}