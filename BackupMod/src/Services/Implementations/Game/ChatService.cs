using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Game;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services.Game;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedVariable")]
[SuppressMessage("ReSharper", "RedundantCast")]
[SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
public class ChatService : IChatService
{
    private readonly IWorldService _worldService;
    private readonly ILogger<ChatService> _logger;
    private readonly ConnectionManager _connectionManager;
    
    public ChatService(
        IConnectionManagerProvider connectionManagerProvider,
        IWorldService worldService,
        ILogger<ChatService> logger)
    {
        _worldService = worldService;
        _logger = logger;
        _connectionManager = connectionManagerProvider.GetConnectionManager();
    }
    
    public void SendMessage(string text)
    {
        ChatMessageServer(null, EChatType.Global, -1, Utils.CreateGameMessage("BackupMod", text), null, EMessageSender.None);
    }

    // The official Release 1.1 b14 implementation but just without junk logging
    public void ChatMessageServer(
    ClientInfo _cInfo,
    EChatType _chatType,
    int _senderEntityId,
    string _msg,
    List<int> _recipientEntityIds,
    EMessageSender _msgSender)
  {
      try
      {
          if (_connectionManager.IsServer)
          {
              ChatMessageClient(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender);
              if (_recipientEntityIds != null)
              {
                  foreach (int recipientEntityId in _recipientEntityIds)
                      SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(recipientEntityId)
                          ?.SendPackage((NetPackage)NetPackageManager.GetPackage<NetPackageChat>()
                              .Setup(_chatType, _senderEntityId, _msg, (List<int>)null, _msgSender));
              }
              else
                  SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(
                      (NetPackage)NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg,
                          (List<int>)null, _msgSender), true);
          }
          else
              SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer((NetPackage)NetPackageManager
                  .GetPackage<NetPackageChat>()
                  .Setup(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender));
      }
      catch
      {
          _logger.LogWarning("Some messages cannot be sent. But if you are leaving the world, it doesn't matter");
      }
  }

  public void ChatMessageClient(
    EChatType _chatType,
    int _senderEntityId,
    string _msg,
    List<int> _recipientEntityIds,
    EMessageSender _msgSender)
  {
    if (GameManager.IsDedicatedServer)
      return;
    
    World world = _worldService.GetCurrentWorld();
    
    foreach (EntityPlayerLocal localPlayer in world.GetLocalPlayers())
    {
      if (_recipientEntityIds == null || _recipientEntityIds.Contains(localPlayer.entityId))
        XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(localPlayer).xui, EnumGameMessages.Chat, _chatType, _msg, _senderEntityId, _msgSender, GeneratedTextManager.TextFilteringMode.Filter);
    }
  }
}