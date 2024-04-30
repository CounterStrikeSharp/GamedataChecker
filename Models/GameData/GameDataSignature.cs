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
    public class GameDataSignature
    {
        [JsonPropertyName("library")]
        public required string Library { get; set; }

        [JsonPropertyName("windows")]
        public required string Windows { get; set; }

        [JsonPropertyName("linux")]
        public required string Linux { get; set; }

        public bool IsValid(OSPlatform platform)
        {
            if (platform == OSPlatform.Windows)
                return !string.IsNullOrEmpty(this.Windows);

            if (platform == OSPlatform.Linux)
                return !string.IsNullOrEmpty(this.Linux);

            throw new NotSupportedException($"Platform '{platform}' is not supported");
        }

        public byte?[] GetPattern()
        {
            return this.ConvertPattern(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? this.Windows : this.Linux);
        }

        public byte?[] GetPattern(OSPlatform platform)
        {
            return this.ConvertPattern(platform == OSPlatform.Windows ? this.Windows : this.Linux);
        }

        public string GetPatternPrettified(string wildcard = "??")
        {
            return this.GetPattern().Prettify(wildcard);
        }

        public string GetPatternPrettified(OSPlatform platform, string wildcard = "??")
        {
            return this.GetPattern(platform).Prettify(wildcard);
        }

        private byte?[] ConvertPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return new byte?[0];
            }

            try
            {
                // TODO: maybe regex? (should be less performant for these kind of strings)
                // ok since the latest update we use IDA style patterns, but we might still support these aswell
                pattern = pattern.Replace("\\\\x", " ")
                                 .Replace("\\x", " ")
                                 .Replace("2A", "?")
                                 .Replace("??", "?");

                string[] bytes = pattern.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                byte?[] result = new byte?[bytes.Length];

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == "?")
                    {
                        result[i] = null;
                    } else
                    {
                        result[i] = Convert.ToByte(bytes[i], 16);
                    }
                }

                return result;
            } catch (FormatException)
            {
                throw new ArgumentException("Invalid pattern format.");
            } catch (OverflowException)
            {
                throw new ArgumentException("Hexadecimal value is out of range for byte.");
            }
        }
    }
}
