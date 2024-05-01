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

    public sealed class TypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection Builder;

        public TypeRegistrar(IServiceCollection builder)
        {
            this.Builder = builder;
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(this.Builder.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            this.Builder.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            this.Builder.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> func)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            this.Builder.AddSingleton(service, (provider) => func());
        }
    }
}
