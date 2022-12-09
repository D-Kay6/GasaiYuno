namespace GasaiYuno.Discord.Polls.Models;

public class PollOption
{
    public string Value { get; init; }
    public List<ulong> Selectors { get; init; }

    public PollOption() { }
    public PollOption(string value)
    {
        Value = value;
        Selectors = new List<ulong>();
    }
}