namespace GasaiYuno.Discord.Domain
{
    public class Server
    {
        public ulong Id { get; init; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public Language Language { get; set; }
    }
}