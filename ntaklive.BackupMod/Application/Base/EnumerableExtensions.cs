using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ntaklive.BackupMod.Application.Base
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> action)
        {
            foreach (T item in collection)
            {
                await action(item);
            }
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection) =>
            collection.Where(t => t is not null);
    }
}