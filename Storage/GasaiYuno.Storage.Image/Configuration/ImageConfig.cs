namespace GasaiYuno.Storage.Image.Configuration
{
    public struct ImageConfig
    {
        public string Directory { get; init; }
        public string CoreSubDirectory { get; init; }
        public string[] SupportedFormats { get; init; }
    }
}