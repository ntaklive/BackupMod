using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ntaklive.BackupMod.Abstractions
{
    public static class DomainEvents
    {
        private static readonly List<Type> Handlers = 
            Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IHandler<>)))
            .ToList();

        public static void Dispatch(IDomainEvent domainEvent)
        {
            foreach (Type handlerType in Handlers)
            {
                bool canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IHandler<>)
                        && x.GenericTypeArguments[0] == domainEvent.GetType());
        
                if (canHandleEvent)
                {
                    object handler = Activator.CreateInstance(handlerType)!;
                    handlerType.GetMethod("Handle")!.Invoke(handler, new object[] {domainEvent});
                }
            }
        }
    }
}
