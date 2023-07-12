#pragma warning disable CS8600
#pragma warning disable CS8625

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace ntaklive.BackupMod.Infrastructure.Game
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly ConnectionManager _connectionManager;
    
        public ChatService(
            IConnectionManagerProvider connectionManagerProvider,
            ILogger<ChatService> logger)
        {
            _logger = logger;
            _connectionManager = connectionManagerProvider.GetConnectionManager();
        }
    
        public void SendMessage(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("The message for sending cannot be null");
            }
        
            ChatMessageServerWithoutLog(null, EChatType.Global, -1, text!, "BackupMod", false, null);
        }

        // The official Alpha 20 implementation but just without junk logging
        private void ChatMessageServerWithoutLog(
            ClientInfo _cInfo,
            EChatType _chatType,
            int _senderEntityId,
            string _msg,
            string _mainName,
            bool _localizeMain,
            List<int> _recipientEntityIds)
        {
            try
            {
                if (_connectionManager.IsServer)
                {
                    ModEvents.ChatMessage.Invoke(_cInfo, _chatType, _senderEntityId, _msg, _mainName, _localizeMain, _recipientEntityIds);
                    GameManager.Instance.ChatMessageClient(_chatType, _senderEntityId, _msg, _mainName, _localizeMain, _recipientEntityIds);
                    string _txt = string.Format("Chat (from '{0}', entity id '{1}', to '{2}'): '{3}': {4}",
                        (object) (_cInfo?.PlatformId != null ? _cInfo.PlatformId.CombinedString : "-non-player-"),
                        (object) _senderEntityId, (object) _chatType.ToStringCached<EChatType>(),
                        _localizeMain ? (object) Localization.Get(_mainName) : (object) _mainName,
                        (object) global::Utils.FilterBbCode(_msg));
                    if (_recipientEntityIds != null)
                    {
                        foreach (int recipientEntityId in _recipientEntityIds)
                            _connectionManager.Clients.ForEntityId(recipientEntityId)
                                ?.SendPackage((NetPackage) NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType,
                                    _senderEntityId, _msg, _mainName, _localizeMain, (List<int>) null));
                    }
                    else
                    {
                        _connectionManager.SendPackage(
                            (NetPackage) NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg,
                                _mainName, _localizeMain, (List<int>) null), true);
                    }
                }
                else
                {
                    _connectionManager.SendToServer((NetPackage) NetPackageManager
                        .GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, _mainName, _localizeMain,
                            _recipientEntityIds));
                }
            }
            catch
            {
                _logger.LogWarning("Some messages cannot be sent. But if you are leaving the world, it doesn't matter");
            }
        }
    }
}