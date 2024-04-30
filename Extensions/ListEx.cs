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
    public static class ListEx
    {
        public static Dictionary<TKey, List<TValue>> GroupByKey<TKey, TValue>(this IEnumerable<TValue> list, Func<TValue, TKey> keySelector)
            where TKey : notnull
        {
            Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

            foreach (TValue value in list)
            {
                var key = keySelector(value);

                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(value);
                } else
                {
                    dictionary[key] = new List<TValue> { value };
                }
            }

            return dictionary;
        }
    }
}
