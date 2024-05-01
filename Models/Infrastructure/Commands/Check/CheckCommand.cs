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
    using Spectre.Console;

    public sealed class CheckCommand : AsyncCommand<CheckCommandSettings>
    {
        public override ValidationResult Validate(CommandContext context, CheckCommandSettings settings)
        {
            if (settings.GamedataFiles == null)
            {
                return ValidationResult.Error("No gamedata specified");
            }

            if (settings.BinaryFiles == null)
            {
                return ValidationResult.Error("No binaries specified");
            }

            foreach (string gameDataFile in settings.GamedataFiles)
            {
                if (!File.Exists(gameDataFile))
                {
                    return ValidationResult.Error($"Gamedata file not found: {gameDataFile}");
                }
            }

            foreach (string binaryFile in settings.BinaryFiles)
            {
                if (!File.Exists(binaryFile))
                {
                    return ValidationResult.Error($"Binary file not found: {binaryFile}");
                }
            }

            return ValidationResult.Success();
        }

        public override async Task<int> ExecuteAsync(CommandContext context, CheckCommandSettings settings)
        {
            List<GameBinary> binaries = new List<GameBinary>();

            foreach (string binaryPath in settings.BinaryFiles)
            {
                // this could be libserver.so or server.dll for e.g.
                string binaryName = Path.GetFileNameWithoutExtension(binaryPath);

                // we dont include the "lib" part in the gamedata library
                if (binaryName.StartsWith("lib"))
                {
                    binaryName = binaryName.Substring("lib".Length);
                }

                binaries.Add(new GameBinary(binaryName, binaryPath, binaryPath.EndsWith(".dll") ? OSPlatform.Windows : OSPlatform.Linux));
            }

            foreach (string gameDataFilePath in settings.GamedataFiles)
            {
                Log.Info("================================================");
                Log.Info($"Checking '[bold slateblue1]{Path.GetFileName(gameDataFilePath)}[/]'...");

                using (GameDataChecker gameDataChecker = new GameDataChecker(gameDataFilePath, binaries))
                {
                    Dictionary<string, List<GameDataSignatureReport>> groupReports = await gameDataChecker.CheckAsync();

                    foreach (KeyValuePair<string, List<GameDataSignatureReport>> reports in groupReports)
                    {
                        foreach (GameDataSignatureReport report in reports.Value)
                        {
                            if (report.Found)
                            {
                                Log.Info($"[blue]{report.Signature.Library}[/] ({(report.Platform == OSPlatform.Windows ? "[steelblue1]" : "[darkorange3_1]")}{report.Platform.ToString().UppercaseFirstLetter()}[/]):\t'[lightskyblue1]{report.Name}[/]' -> Found ({report.Matches.Count} matches, {(report.IsReliable() ? "[bold lightgreen]reliable[/]" : "[bold red]unreliable[/]")})");
                            }
                            else
                            {
                                if (report.Signature.IsValid(report.Platform))
                                {
                                    Log.Error($"[blue]{report.Signature.Library}[/] ({(report.Platform == OSPlatform.Windows ? "[steelblue1]" : "[darkorange3_1]")}{report.Platform.ToString().UppercaseFirstLetter()}[/]):\t'[lightskyblue1]{report.Name}[/]' -> [bold red]Signature has no matches[/]");
                                }
                                else
                                {
                                    Log.Warning($"[blue]{report.Signature.Library}[/] ({(report.Platform == OSPlatform.Windows ? "[steelblue1]" : "[darkorange3_1]")}{report.Platform.ToString().UppercaseFirstLetter()}[/]):\t'[lightskyblue1]{report.Name}[/]' -> [bold yellow1]No signature provided[/]");
                                }
                            }
                        }
                    }

                    await Console.Out.WriteLineAsync();
                    Log.Info($"Done in {(gameDataChecker.GetElapsedTime().TotalSeconds):F2}s");
                    Log.Info($"[lightgreen]{gameDataChecker.Passed} Passed[/]");
                    Log.Info($"[gold1]{gameDataChecker.Warnings} Warnings[/]");
                    Log.Info($"[red]{gameDataChecker.Errors} Errors[/]");
                    Log.Info("================================================");
                    await Console.Out.WriteLineAsync();
                }
            }

            return GameDataChecker.TotalErrors;
        }
    }
}
