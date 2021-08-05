namespace GasaiYuno.Discord.Domain
{
    public class CustomCommand
    {
        public Server Server { get; init; }
        public string Command { get; set; }
        public string Response { get; set; }
    }
}