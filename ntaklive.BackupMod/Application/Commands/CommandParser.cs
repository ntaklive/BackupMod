using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;
using UniLinq;

namespace ntaklive.BackupMod.Application.Commands
{
    public class CommandParser : ICommandParser
    {
        private readonly Dictionary<string, Func<Match, ValueTuple<Type, CommandArgs>>> _factories = new();
        private readonly Dictionary<string, Regex> _regexes = new();

        public CommandParser()
        {
            // backup (-t title)
            RegisterCommandArgsParser<BackupCommand, BackupCommandArgs>(@"^(backup|bp) ?(-t ?(?<title>[\w\s]*))?$");
            
            // backup info
            RegisterCommandArgsParser<BackupInfoCommand, CommandArgs>(@"^(backup|bp) ?info$");
        }

        public ValueResult<(Type commandType, CommandArgs args)> Parse(string args)
        {
            foreach (KeyValuePair<string, Regex> valuePair in _regexes)
            {
                Match match = valuePair.Value.Match(args);
                if (match.Success)
                {
                    (Type, CommandArgs) result = _factories[valuePair.Key].Invoke(match);
                    return ValueResult<(Type commandType, CommandArgs args)>.Success(result);
                }
            }

            return ValueResult<(Type commandType, CommandArgs args)>.Error(
                "There are no commands accepting these arguments");
        }

        private void RegisterCommandArgsParser<TCommand, TCommandArgs>(string regexPattern)
            where TCommand : IConsoleCommand<TCommandArgs>
            where TCommandArgs : CommandArgs
        {
            if (_regexes.ContainsKey(regexPattern) || _factories.ContainsKey(regexPattern))
            {
                throw new InvalidOperationException(
                    "Cannot register command factory, because the regex-key already exists in the dictionary");
            }

            var regex = new Regex(regexPattern, RegexOptions.Compiled);
            _regexes.Add(regexPattern, regex);

            Type commandArgsType = typeof(TCommandArgs);

            _factories.Add(regexPattern, match =>
            {
                ConstructorInfo[] commandArgsPublicConstructors = commandArgsType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                if (commandArgsPublicConstructors.Length == 0)
                {
                    throw new InvalidOperationException(
                        $"The {commandArgsType.FullName} class must provide one public constructor");
                }

                if (commandArgsPublicConstructors.Length > 1)
                {
                    throw new InvalidOperationException(
                        $"The {commandArgsType.FullName} class must provide only one public constructor");
                }

                ConstructorInfo commandArgsConstructor = commandArgsPublicConstructors.First();
                ParameterInfo[] commandArgsConstructorParameters = commandArgsConstructor.GetParameters();

                var namedMatchGroups = new Group[match.Groups.Count];
                match.Groups.CopyTo(namedMatchGroups, 0);
                namedMatchGroups = namedMatchGroups.Where(group => !int.TryParse(group.Name, out _)).ToArray();

                if (namedMatchGroups.Length != commandArgsConstructorParameters.Length)
                {
                    throw new InvalidOperationException(
                        "The regex must contain the number of groups equal to the number of constructor parameters");
                }

                string[] parameterNames = commandArgsConstructorParameters.Select(info => info.Name).ToArray();
            
                namedMatchGroups = namedMatchGroups.OrderBy(x => Array.IndexOf(parameterNames, x.Name)).ToArray();

                var argTypeDictionary = new Dictionary<string, (string argValue, Type paramType)>();
                for (var i = 0; i < namedMatchGroups.Length; i++)
                {
                    Group argGroup = namedMatchGroups[i];
                    string argName = argGroup.Name;
                    string argValue = argGroup.Value;

                    ParameterInfo param = commandArgsConstructorParameters[i];
                    string paramName = param.Name;
                    Type paramType = param.ParameterType;

                    if (argName != paramName)
                    {
                        throw new InvalidOperationException(
                            "The regex group names must be equal to the constructor parameter names");
                    }

                    argTypeDictionary.Add(argName, new ValueTuple<string, Type>(argValue, paramType));
                }

                var parsedArgs = new List<object>();
                foreach (KeyValuePair<string, (string argValue, Type paramType)> tuple in argTypeDictionary)
                {
                    string value = tuple.Value.argValue;
                    Type type = tuple.Value.paramType;

                    if (!TryParse(type, value, out object? arg))
                    {
                        throw new InvalidOperationException(
                            $"Cannot find suitable converter to convert {value} to {type.FullName}");
                    }

                    parsedArgs.Add(arg!);
                }

                object commandArgsObj = commandArgsConstructor.Invoke(parsedArgs.ToArray());

                return Command<TCommand, TCommandArgs>(commandArgsObj);
            });
        }
    
        private static ValueTuple<Type, CommandArgs> Command<TCommand, TCommandArgs>(object args)
            where TCommand : IConsoleCommand<TCommandArgs>
            where TCommandArgs : CommandArgs
        {
            Type commandType = typeof(TCommand);
            if (!commandType.IsAssignableFrom(typeof(TCommand)))
            {
                throw new InvalidOperationException(
                    $"The IConsoleCommand<> interface is not assignable from the {commandType.FullName} class");
            }

            return new ValueTuple<Type, CommandArgs>(commandType, (TCommandArgs) args);
        }

        private static bool TryParse(Type type, string value, out object? obj)
        {
            obj = null;

            try
            {
                obj = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)!;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}