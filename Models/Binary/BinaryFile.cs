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
    public class BinaryFile : IDisposable
    {
        public string FilePath { get; private set; }

        public string FileName { get; private set; }

        private byte[]? Data { get; set; }

        private nint NativeHandle { get; set; } = nint.Zero;

        public BinaryFile(string path)
        {
            this.FilePath = path;
            this.FileName = Path.GetFileName(this.FilePath);
        }

        public bool FindExport(string symbol, Action<nint>? callback = null)
        {
            if (this.NativeHandle == nint.Zero)
            {
                if (NativeLibrary.TryLoad(this.FilePath, out nint handle))
                {
                    this.NativeHandle = handle;
                } else
                {
                    throw new Exception($"Unable to load native library '{this.FileName}'");
                }
            }

            nint export = NativeLibrary.GetExport(this.NativeHandle, symbol);
            callback?.Invoke(export);
            return export != nint.Zero;
        }

        public async Task ReadBytesAsync()
        {
            this.Data = await File.ReadAllBytesAsync(this.FilePath);
            Log.Debug($"Parsed binary '[bold slateblue1]{this.FileName}[/]' ({this.Data.Length} bytes)");
        }

        public bool IsLoaded()
        {
            return this.Data != null;
        }

        public bool FindPattern(byte?[] pattern, Action<long>? callback = null)
        {
            if (this.Data == null)
            {
                return false;
            }

            string patternPrettified = pattern.Prettify();
            int count = 0;

            Log.Debug($"Searching for pattern '[bold lightskyblue1]{patternPrettified}[/]' in binary '[bold slateblue1]{this.FileName}[/]'...");

            for (long i = 0; i <= this.Data.LongLength - pattern.LongLength; i++)
            {
                long j;

                for (j = 0; j < pattern.LongLength; j++)
                {
                    if (pattern[j].HasValue && this.Data[i + j] != pattern[j])
                        break;
                }

                if (j == pattern.LongLength)
                {
                    callback?.Invoke(i);
                    count++;

                    Log.Debug($"Found pattern '[bold lightskyblue1]{patternPrettified}[/]' in binary '[bold slateblue1]{this.FileName}[/]' at offset [bold lightskyblue1]0x{i:X}[/] ([]{count}[/]. match)");
                }
            }

            return count > 0;
        }

        public void Dispose()
        {
            this.Data = null;
        }
    }
}
