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

    internal static class Log
    {
        private static void InternalWrite(string message)
        {
            string final = $"[grey][[{DateTime.Now:HH:mm:ss}]][/] {message}";
            AnsiConsole.MarkupLine(final);
        }

        public static void Info(string message)
        {
            InternalWrite($"[[[bold purple]GDC[/]]] {message}");
        }

        public static void Error(string message)
        {
            InternalWrite($"[[[bold red]ERROR[/]]] {message}");
        }

        public static void Warning(string message)
        {
            InternalWrite($"[[[bold yellow1]WARNING[/]]] {message}");
        }

        public static void Debug(string message)
        {
#if DEBUG
            InternalWrite($"[[[bold cyan]DEBUG[/]]] [[[bold grey42]THREAD:/{Thread.CurrentThread.ManagedThreadId}[/]]]\t{message}");
#endif
        }

        public static void Exception(Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
        }
    }
}
