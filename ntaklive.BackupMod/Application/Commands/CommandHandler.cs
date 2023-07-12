using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ntaklive.BackupMod.Abstractions;
using ntaklive.BackupMod.Application.Commands.Args;
using ntaklive.BackupMod.Application.Commands.EventArgs;
using UnityEngine;

namespace ntaklive.BackupMod.Application.Commands
{
    public sealed class CommandHandler : CommandHandlerBase
    {
        private readonly ICommandParser _commandParser;
        // private readonly ILogger<CommandHandler> _logger;

        private readonly string _helpText;

        private readonly Dictionary<string, string> _commandDescriptionDictionary = new()
        {
            // todo: заменить на Localization
            {"", "perform a forceful backup"},
            {"info", "show the current configuration of the mod"},
            {"list", "show all available backups"},
            {"restore", "restore a save from a backup"},
            {"delete", "delete a backup"},
            {"start", "start an auto backup process (even if disabled in settings.json)"},
            {"stop", "stop the current AutoBackup process"},
        };

        public CommandHandler()
        {
            if (Provider == null)
            {
                throw new InvalidOperationException("ServiceProvider for commands is not set yet");
            }

            _commandParser = Provider.GetRequiredService<ICommandParser>();
            // _logger = Provider.GetRequiredService<ILogger<CommandHandler>>();

            _helpText =
                $"Usage:\n  {string.Join("\n  ", _commandDescriptionDictionary.Keys.Select((command, i) => $"{i + 1}. {GetCommands()[0]} {command}").ToList())}\nDescription Overview\n{string.Join("\n", _commandDescriptionDictionary.Values.Select((description, j) => $"{j + 1}. {description}").ToList())}";
        }

        public event EventHandler<CommandExecutedEventArgs>? CommandExecuted;

        public override bool AllowedInMainMenu => true;

        public override string[] GetCommands() => new[] {"backup", "bp"};

        public override string GetDescription() =>
            // todo: заменить на Localization
            "Some commands to simplify the creation of backups (commands are provided by BackupMod)";

        public override string GetHelp()
        {
            return _helpText;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            string args = _params.Aggregate((s1, s2) => $"{s1} {s2}");

            Debug.Log(Localization.Get(LocalizationKeys.OnlyWhenTheGameIsStarted));
            
            ValueResult<(Type commandType, CommandArgs args)> parseCommandResult = _commandParser.Parse(args);
            if (!parseCommandResult.IsSuccess)
            {
                // _logger.LogError(parseCommandResult.ErrorMessage);
            }
            Type commandType = parseCommandResult.Data.commandType!;
            CommandArgs commandArgs = parseCommandResult.Data.args!;

            object commandObj = Provider!.GetRequiredService(commandType);
        
            MethodInfo executeMethod = commandType.GetMethod("Execute")!;
            var executeResult = (Result) executeMethod.Invoke(commandObj, new object[] {commandArgs});
            if (!executeResult.IsSuccess)
            {
                // _logger.LogError(executeResult.ErrorMessage);
            }
            // OnCommandExecuted(new CommandExecutedEventArgs(commandType));
        }

        private void OnCommandExecuted(CommandExecutedEventArgs e)
        {
            CommandExecuted?.Invoke(this, e);
        }
    }
}