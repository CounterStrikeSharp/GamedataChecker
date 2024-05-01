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

namespace GDC;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var app = new CLIApp();

        app.Configure(config =>
        {
            config.SetApplicationName("GDC");

            config.SetExceptionHandler((Exception exception, ITypeResolver? resolver) =>
            {
                Log.Error("Exception occurred:");
                Log.Exception(exception);
            });

            config.AddCommand<CheckCommand>("check")
                .WithAlias("c")
                .WithDescription("Check gamedata file")
                .WithExample("check", "--gamedata mygamedata.json", "--gamedata mygamedata2.json", "--binary server.dll", "--binary libserver.so");
        });

        return await app.RunAsync(args);
    }
}