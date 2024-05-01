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
    using Spectre.Console.Cli;

    public sealed class TypeResolver : ITypeResolver, IDisposable
    {
        private readonly IServiceProvider Provider;

        public TypeResolver(IServiceProvider provider)
        {
            this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object? Resolve(Type? type)
        {
            if (type == null)
            {
                return null;
            }

            return this.Provider.GetService(type);
        }

        public void Dispose()
        {
            if (this.Provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
