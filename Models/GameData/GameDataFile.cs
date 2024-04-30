/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

namespace GDC
{
    public class GameDataFile : IDisposable
    {
        private readonly OSPlatform[] Platforms = [ OSPlatform.Windows, OSPlatform.Linux ];

        private readonly Dictionary<OSPlatform, List<GameBinary>> Binaries;

        public string FilePath { get; private set; }

        public GameData? Data;

        public GameDataFile(string filePath, IEnumerable<GameBinary> binaries)
        {
            this.FilePath = filePath;
            this.Binaries = binaries.GroupByKey(binary => binary.Platform);
        }

        public async Task LoadAsync()
        {
            using (FileStream fs = File.OpenRead(this.FilePath))
            {
                this.Data = await JsonSerializer.DeserializeAsync<GameData>(fs) ?? throw new ArgumentNullException(nameof(this.FilePath));
            }
        }

        public GameBinary? GetBinaryForPlatform(string binaryName, OSPlatform platform)
        {
            if (!this.Platforms.Contains(platform))
            {
                throw new NotSupportedException($"Platform '{platform}' is not supported");
            }

            if (this.Binaries.TryGetValue(platform, out List<GameBinary>? binaries))
            {
                return binaries.FirstOrDefault(b => b.Name == binaryName);
            }

            throw new FileNotFoundException($"Unable to find binary '{binaryName}' for platform '{platform}'");
        }

        public async Task<IEnumerable<GameDataSignatureReport>> ScanAsync()
        {
            if (this.Data == null)
            {
                return Enumerable.Empty<GameDataSignatureReport>();
            }

            List<Task<GameDataSignatureReport>> tasks = new List<Task<GameDataSignatureReport>>();

            foreach (KeyValuePair<string, GameDataValue> gameData in this.Data)
            {
                foreach (OSPlatform platform in this.Platforms)
                {
                    if (gameData.Value.Offsets != null)
                        continue;

                    if (gameData.Value.Signatures == null)
                        continue;

                    GameBinary? binary = this.GetBinaryForPlatform(gameData.Value.Signatures.Library, platform);

                    if (binary == null)
                    {
                        throw new FileNotFoundException($"Binary file '{gameData.Value.Signatures.Library}' could not be found for platform '{platform}'");
                    }    

                    if (!binary.IsLoaded())
                    {
                        await binary.ReadBytesAsync();
                    }

                    tasks.Add(Task.Run(() => this.ScanBinary(binary, platform, gameData)));
                }
            }

            return await Task.WhenAll(tasks);
        }

        private GameDataSignatureReport ScanBinary(GameBinary binary, OSPlatform platform, KeyValuePair<string, GameDataValue> gameData)
        {
            GameDataSignatureReport report = new GameDataSignatureReport
            {
                Name = gameData.Key,
                Signature = gameData.Value.Signatures!,
                Platform = platform,
                Found = false
            };

            if (!gameData.Value.Signatures!.IsValid(platform))
                return report;

            byte?[]? patternBytes = gameData.Value.Signatures!.GetPattern(platform);

            if (patternBytes == null)
                return report;

            if (binary.FindPattern(patternBytes, offset => report.Matches.Add(new GameDataSignatureMatch { Offset = offset })))
            {
                report.Found = true;
            }

            return report;
        }

        public void Dispose()
        {
            this.Data?.Clear();
        }
    }
}
