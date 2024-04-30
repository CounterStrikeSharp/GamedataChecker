namespace GDC
{
    public sealed class GameDataChecker : IDisposable
    {
        public IEnumerable<GameBinary> Binaries { get; private set; }

        public string FilePath { get; private set; }

        public int Passed { get; private set; } = 0;

        public int Errors { get; private set; } = 0;

        public int Warnings { get; private set; } = 0;

        private DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public static int TotalErrors { get; private set; } = 0;

        public GameDataChecker(string filePath, IEnumerable<GameBinary> binaries)
        {
            this.Binaries = binaries;
            this.FilePath = filePath;
        }

        public async Task<Dictionary<string, List<GameDataSignatureReport>>> CheckAsync()
        {
            using (GameDataFile gameDataFile = new GameDataFile(this.FilePath, this.Binaries))
            {
                await gameDataFile.LoadAsync();

                this.StartTime = DateTime.Now;
                IEnumerable<GameDataSignatureReport> reports = await gameDataFile.ScanAsync();
                this.EndTime = DateTime.Now;

                foreach (GameDataSignatureReport report in reports)
                {
                    if (report.Found)
                    {
                        if (report.IsReliable())
                        {
                            this.Passed++;
                        } else
                        {
                            this.Errors++;
                        }
                    } else
                    {
                        if (report.Signature.IsValid(report.Platform))
                        {
                            this.Errors++;
                        } else
                        {
                            this.Warnings++;
                        }
                    }
                }

                TotalErrors += this.Errors;
                return reports.GroupByKey(report => report.Name);
            }
        }

        public TimeSpan GetElapsedTime()
        {
            return this.EndTime - this.StartTime;
        }

        public void Dispose()
            { }
    }
}
