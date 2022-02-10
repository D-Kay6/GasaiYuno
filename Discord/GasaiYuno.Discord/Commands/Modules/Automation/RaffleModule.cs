using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("Raffle")]
    [RequireOwner]
    public class RaffleModule : BaseModule<RaffleModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RaffleModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task RaffleDefaultAsync() => ReplyAsync(Translation.Message("Automation.Raffle.Default"));
    
        [Command]
        public Task RaffleDefaultAsync(string title, TimeSpan duration, [Remainder] string context) => RaffleCreateAsync(title, duration, context);
    
        [Command("Create")]
        [Alias("New")]
        [Priority(-1)]
        public async Task RaffleCreateAsync(string title, TimeSpan duration, [Remainder] string context)
        {
            await Context.Message.DeleteAsync().ConfigureAwait(false);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(title);
            embedBuilder.WithDescription($"{context}\n\n{Translation.Message("Automation.Raffle.Footer")}");
            embedBuilder.WithFooter(Translation.Message("Automation.Raffle.EndDate"));
            embedBuilder.WithTimestamp(DateTimeOffset.Now + duration);

            var message = await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            await message.AddReactionAsync(new Emoji("✅")).ConfigureAwait(false);

            var raffle = new Raffle
            {
                Server = Server,
                Channel = message.Channel.Id,
                Message = message.Id,
                Title = title,
                EndDate = DateTime.Now + duration
            };
            _unitOfWork.Raffles.Add(raffle);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}