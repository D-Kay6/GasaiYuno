using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("DynamicChannels")]
    [Alias("dc", "DynamicChannel")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class DynamicChannelModule : BaseModule<DynamicChannelModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DynamicChannelModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task DynamicChannelDefaultAsync() => ReplyAsync(Translation.Message("Automation.Channel.Default"));

        [Command]
        [Priority(-1)]
        public async Task DynamicChannelDefaultAsync(string name)
        {
            var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                return;
            }

            var assignedChannels = string.Empty;
            if (dynamicChannel.Channels.Any())
                assignedChannels = string.Join(Environment.NewLine, dynamicChannel.Channels.Select(x => Context.Guild.GetChannel(x)?.Name).Where(x => !string.IsNullOrEmpty(x)));

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(dynamicChannel.Name);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Info.Channels"), assignedChannels, true);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Info.Type"), dynamicChannel.Type.ToString(), true);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Info.GenerationName"), dynamicChannel.GenerationName, true);

            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command("List")]
        [Alias("Overview")]
        public async Task DynamicChannelListAsync()
        {
            var dynamicChannels = await _unitOfWork.DynamicChannels.ListAsync(Context.Guild.Id).ConfigureAwait(false);
            if (!dynamicChannels.Any())
            {
                await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configurations")).ConfigureAwait(false);
                return;
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Automation.Channel.Info.Title"));
            foreach (var dynamicChannel in dynamicChannels)
                embedBuilder.AddField(dynamicChannel.Name, Translation.Message("Automation.Channel.Info.List", dynamicChannel.Type.ToString(), dynamicChannel.Channels.Count(x => Context.Guild.GetVoiceChannel(x) != null)), true);

            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command("Add")]
        [Alias("Configure")]
        public async Task DynamicChannelAddAsync(string name, AutomationType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Name")).ConfigureAwait(false);
                return;
            }
            if (await _unitOfWork.DynamicChannels.AnyAsync(x => x.Server.Id == Context.Guild.Id && x.Name == name).ConfigureAwait(false))
            {
                await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Exists", name)).ConfigureAwait(false);
                return;
            }

            var dynamicChannel = new DynamicChannel
            {
                Server = Server,
                Type = type,
                Name = name
            };
            
            _unitOfWork.DynamicChannels.Add(dynamicChannel);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Automation.Channel.Added", name)).ConfigureAwait(false);
        }

        [Command("Delete")]
        [Alias("Remove")]
        public async Task DynamicChannelDeleteAsync(string name)
        {
            var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                return;
            }
            
            _unitOfWork.DynamicChannels.Remove(dynamicChannel);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                            
            await ReplyAsync(Translation.Message("Automation.Channel.Deleted", name)).ConfigureAwait(false);
        }

        [Group("Edit")]
        [Alias("Modify", "Change")]
        public class DynamicChannelModifyModule : BaseModule<DynamicChannelModifyModule>
        {
            private readonly ChannelTypeReader<IVoiceChannel> _typeReader;
            private readonly IUnitOfWork _unitOfWork;

            public DynamicChannelModifyModule(IUnitOfWork unitOfWork)
            {
                _typeReader = new ChannelTypeReader<IVoiceChannel>();
                _unitOfWork = unitOfWork;
            }

            [Command]
            [Priority(-1)]
            public async Task DynamicChannelModifyDefaultAsync(string name, string action, [Remainder] string parameters)
            {
                var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
                if (dynamicChannel == null)
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                    return;
                }

                TypeReaderResult parseResult;
                switch (action.ToLower())
                {
                    case "d_name":
                    case "name":
                        await DynamicChannelModifyNameAsync(dynamicChannel, parameters);
                        return;
                    case "assign":
                    case "add":
                    case "apply":
                        parseResult = await _typeReader.ReadAsync(Context, parameters, null).ConfigureAwait(false);
                        if (!parseResult.IsSuccess)
                            throw new ArgumentException("InvalidArgument", nameof(parameters));

                        await DynamicChannelModifyAssignAsync(dynamicChannel, parseResult.BestMatch as IVoiceChannel);
                        return;
                    case "remove":
                    case "delete":
                        parseResult = await _typeReader.ReadAsync(Context, parameters, null).ConfigureAwait(false);
                        if (!parseResult.IsSuccess)
                            throw new ArgumentException("InvalidArgument", nameof(parameters));

                        await DynamicChannelModifyRemoveAsync(dynamicChannel, parseResult.BestMatch as IVoiceChannel);
                        return;
                }

                throw new ArgumentOutOfRangeException(nameof(action), action, "InvalidArgument");
            }

            [Command("d_name")]
            [Alias("Name")]
            public async Task DynamicChannelModifyNameAsync(string name, string generationName)
            {
                var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
                if (dynamicChannel == null)
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                    return;
                }

                await DynamicChannelModifyNameAsync(dynamicChannel, generationName).ConfigureAwait(false);
            }

            private async Task DynamicChannelModifyNameAsync(DynamicChannel dynamicChannel, string generationName)
            {
                if (string.IsNullOrWhiteSpace(generationName))
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.GenerationName")).ConfigureAwait(false);
                    return;
                }

                dynamicChannel.GenerationName = generationName;
                
                _unitOfWork.DynamicChannels.Update(dynamicChannel);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Automation.Channel.Renamed", dynamicChannel.Name, dynamicChannel.GenerationName));
            }

            [Command("Assign")]
            [Alias("Add", "Apply")]
            public async Task DynamicChannelModifyAssignAsync(string name, IVoiceChannel voiceChannel)
            {
                var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
                if (dynamicChannel == null)
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                    return;
                }

                await DynamicChannelModifyAssignAsync(dynamicChannel, voiceChannel).ConfigureAwait(false);
            }

            private async Task DynamicChannelModifyAssignAsync(DynamicChannel dynamicChannel, IVoiceChannel voiceChannel)
            {
                var dynamicChannels = await _unitOfWork.DynamicChannels.ListAsync(Context.Guild.Id).ConfigureAwait(false);
                var conflictingConfiguration = dynamicChannels.FirstOrDefault(x => x.Channels.Contains(voiceChannel.Id));
                if (conflictingConfiguration != null)
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Assigned", conflictingConfiguration.Name, voiceChannel.Name)).ConfigureAwait(false);
                    return;
                }

                dynamicChannel.Channels.Add(voiceChannel.Id);
                
                _unitOfWork.DynamicChannels.Update(dynamicChannel);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Automation.Channel.Assigned", dynamicChannel.Name, voiceChannel.Name));
            }

            [Command("Remove")]
            [Alias("Delete")]
            public async Task DynamicChannelModifyRemoveAsync(string name, IVoiceChannel voiceChannel)
            {
                var dynamicChannel = await _unitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
                if (dynamicChannel == null)
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name)).ConfigureAwait(false);
                    return;
                }

                await DynamicChannelModifyRemoveAsync(dynamicChannel, voiceChannel).ConfigureAwait(false);
            }

            private async Task DynamicChannelModifyRemoveAsync(DynamicChannel dynamicChannel, IVoiceChannel voiceChannel)
            {
                if (!dynamicChannel.Channels.Contains(voiceChannel.Id))
                {
                    await ReplyAsync(Translation.Message("Automation.Channel.Invalid.Unassigned", dynamicChannel.Name, voiceChannel.Name)).ConfigureAwait(false);
                    return;
                }

                dynamicChannel.Channels.Remove(voiceChannel.Id);
                
                _unitOfWork.DynamicChannels.Update(dynamicChannel);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Automation.Channel.Removed", dynamicChannel.Name, voiceChannel.Name));
            }
        }
    }
}