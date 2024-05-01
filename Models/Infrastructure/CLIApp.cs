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
    using System.Reflection;

    /// <summary>
    /// Wrapper around <see cref="CommandApp"/> to encapsulate DI
    /// </summary>
    public sealed class CLIApp
    {
        private CommandApp App { get; set; }

        public CLIApp()
        {
            // I have ideas for later on and we will benefit from a DI setup, until that this is pretty much unused
            ServiceCollection registrations = new ServiceCollection();

            // https://github.com/roflmuffin/CounterStrikeSharp/blob/main/managed/CounterStrikeSharp.API/Core/Plugin/PluginContext.cs#L166-L180
            Type interfaceType = typeof(IAppServiceCollection);
            Type[] serviceCollectionConfiguratorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToArray();

            if (serviceCollectionConfiguratorTypes.Any())
            {
                foreach (Type t in serviceCollectionConfiguratorTypes)
                {
                    object? pluginServiceCollection = Activator.CreateInstance(t);
                    MethodInfo? method = t.GetMethod("ConfigureServices");
                    method?.Invoke(pluginServiceCollection, new object[] { registrations });
                }
            }

            TypeRegistrar registrar = new TypeRegistrar(registrations);
            this.App = new CommandApp(registrar);
        }

        public void Configure(Action<IConfigurator> configuration) => this.App.Configure(configuration);

        public async Task<int> RunAsync(string[] args) => await this.App.RunAsync(args);
    }
}
