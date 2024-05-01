namespace GDC
{
    public sealed class CheckCommandSettings : CommandSettings
    {
        [CommandOption("--gamedata")]
        public required string[] GamedataFiles { get; set; }

        [CommandOption("--binary")]
        public required string[] BinaryFiles { get; set; }
    }
}
