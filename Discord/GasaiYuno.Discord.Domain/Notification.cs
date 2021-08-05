namespace GasaiYuno.Discord.Domain
{
    public class Notification
    {
        public Server Server { get; init; }
        public NotificationType Type { get; init; }
        public string Message { get; set; }
        public string Image { get; set; }
        public ulong? Channel { get; set; }
    }
}