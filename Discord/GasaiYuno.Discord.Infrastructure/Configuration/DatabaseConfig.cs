namespace GasaiYuno.Discord.Infrastructure.Configuration
{
    public struct DatabaseConfig
    {
        public string Ip { get; init; }
        public string Port { get; init; }
        public string Database { get; init; }
        public string UserId { get; init; }
        public string Password { get; init; }
    }
}