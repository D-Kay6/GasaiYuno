namespace GasaiYuno.Listing.Discord.Configuration
{
    internal struct ListingConfig
    {
        public EndpointConfig TopGg { get; init; }
        public EndpointConfig DiscordBotsGg { get; init; }
        public EndpointConfig DiscordBotList { get; init; }
    }
}