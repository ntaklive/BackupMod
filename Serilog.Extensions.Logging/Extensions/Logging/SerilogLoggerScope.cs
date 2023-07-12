// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Extensions.Logging.Extensions.Logging
{
    class SerilogLoggerScope : IDisposable
    {
        const string NoName = "None";

        readonly SerilogLoggerProvider _provider;
        readonly object _state;
        readonly IDisposable _chainedDisposable;

        // An optimization only, no problem if there are data races on this.
        bool _disposed;

        public SerilogLoggerScope(SerilogLoggerProvider provider, object state, IDisposable chainedDisposable = null)
        {
            _provider = provider;
            _state = state;

            Parent = _provider.CurrentScope;
            _provider.CurrentScope = this;
            _chainedDisposable = chainedDisposable;
        }

        public SerilogLoggerScope Parent { get; }
        
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                // In case one of the parent scopes has been disposed out-of-order, don't
                // just blindly reinstate our own parent.
                for (var scan = _provider.CurrentScope; scan != null; scan = scan.Parent)
                {
                    if (ReferenceEquals(scan, this))
                        _provider.CurrentScope = Parent;
                }

                _chainedDisposable?.Dispose();
            }
        }

        public void EnrichAndCreateScopeItem(LogEvent logEvent, ILogEventPropertyFactory propertyFactory, out LogEventPropertyValue scopeItem)
        {
            if (_state == null)
            {
                scopeItem = null;
                return;
            }

            if (_state is IEnumerable<KeyValuePair<string, object>> stateProperties)
            {
                scopeItem = null; // Unless it's `FormattedLogValues`, these are treated as property bags rather than scope items.

                foreach (var stateProperty in stateProperties)
                {
                    if (stateProperty.Key == SerilogLoggerProvider.OriginalFormatPropertyName && stateProperty.Value is string)
                    {
                        scopeItem = new ScalarValue(_state.ToString());
                        continue;
                    }

                    var key = stateProperty.Key;
                    var destructureObject = false;
                    var value = stateProperty.Value;

                    if (key.StartsWith("@"))
                    {
                        key = key.Substring(1);
                        destructureObject = true;
                    }

                    if (key.StartsWith("$"))
                    {
                        key = key.Substring(1);
                        value = value?.ToString();
                    }

                    var property = propertyFactory.CreateProperty(key, value, destructureObject);
                    logEvent.AddPropertyIfAbsent(property);
                }
            }
            else
            {
                scopeItem = propertyFactory.CreateProperty(NoName, _state).Value;
            }
        }
    }
}
