using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Commands.TypeReaders;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("Poll")]
    //[Alias("p")]
    [RequireUserPermission(GuildPermission.Administrator)]
    [RequireOwner]
    public class PollModule : BaseModule<PollModule>
    {
        private readonly PollOptionTypeReader _pollOptionTypeReader;
        private readonly IUnitOfWork _unitOfWork;

        public PollModule(IUnitOfWork unitOfWork)
        {
            _pollOptionTypeReader = new PollOptionTypeReader();
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task PollDefaultAsync() => ReplyAsync(Translation.Message("Automation.Poll.Default"));

        [Command]
        public Task PollDefaultAsync(string text, [Remainder] string options) => PollCreateAsync(text, options);

        [Command]
        public Task PollDefaultAsync(string text, TimeSpan duration, [Remainder] string options) => PollCreateAsync(text, duration, options);
        
        [Command("Create")]
        [Priority(-1)]
        public async Task PollCreateAsync(string text, [Remainder] string options)
        {
            var pollOptions = await _pollOptionTypeReader.ReadAsync(Context, options, null).ConfigureAwait(false);
            if (!pollOptions.IsSuccess)
                throw new ArgumentException(nameof(options));

            await PollCreateAsync(text, pollOptions.BestMatch as PollOption[]).ConfigureAwait(false);
        }

        [Command("Create")]
        [Priority(-1)]
        public async Task PollCreateAsync(string text, TimeSpan duration, [Remainder] string options)
        {
            var pollOptions = await _pollOptionTypeReader.ReadAsync(Context, options, null).ConfigureAwait(false);
            if (!pollOptions.IsSuccess)
                throw new ArgumentException(nameof(options));

            await PollCreateAsync(text, duration, pollOptions.BestMatch as PollOption[]).ConfigureAwait(false);
        }

        [Command("Create")]
        public async Task PollCreateAsync(string text, PollOption[] options)
        {
            await Context.Message.DeleteAsync().ConfigureAwait(false);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(text);
            embedBuilder.WithDescription(string.Join(Environment.NewLine, options.Select(x => $"{x.Emote} {x.Message}")));

            var message = await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            foreach (var option in options)
                await message.AddReactionAsync(new Emoji(option.Emote)).ConfigureAwait(false);
        }

        [Command("Create")]
        public async Task PollCreateAsync(string text, TimeSpan duration, PollOption[] options)
        {
            await Context.Message.DeleteAsync().ConfigureAwait(false);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(text);
            embedBuilder.WithDescription(string.Join(Environment.NewLine, options.Select(x => $"{x.Emote} {x.Message}")));
            embedBuilder.WithFooter(Translation.Message("Automation.Poll.Ending"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var message = await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            foreach (var option in options)
                await message.AddReactionAsync(new Emoji(option.Emote)).ConfigureAwait(false);

            var poll = new Poll
            {
                Server = Server,
                Channel = message.Channel.Id,
                Message = message.Id,
                MultiSelect = false,
                EndDate = DateTime.Now + duration,
                Text = text,
                Options = options.ToList()
            };
            
            _unitOfWork.Polls.Add(poll);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}