using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedParameter.Local")]
public class ChatService : IChatService
{
    public void SendMessage(string text)
    {
        ChatMessageServerWithoutLog(null, EChatType.Global, -1, text, "BackupMod", false, null);
    }

    private static void ChatMessageServerWithoutLog(
        ClientInfo _cInfo,
        EChatType _chatType,
        int _senderEntityId,
        string _msg,
        string _mainName,
        bool _localizeMain,
        List<int> _recipientEntityIds)
    {
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            ModEvents.ChatMessage.Invoke(_cInfo, _chatType, _senderEntityId, _msg, _mainName, _localizeMain, _recipientEntityIds);
            GameManager.Instance.ChatMessageClient(_chatType, _senderEntityId, _msg, _mainName, _localizeMain, _recipientEntityIds);
            string _txt = string.Format("Chat (from '{0}', entity id '{1}', to '{2}'): '{3}': {4}",
                (object) (_cInfo?.PlatformId != null ? _cInfo.PlatformId.CombinedString : "-non-player-"),
                (object) _senderEntityId, (object) _chatType.ToStringCached<EChatType>(),
                _localizeMain ? (object) Localization.Get(_mainName) : (object) _mainName,
                (object) Utils.FilterBbCode(_msg));
            if (_recipientEntityIds != null)
            {
                foreach (int recipientEntityId in _recipientEntityIds)
                    SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(recipientEntityId)
                        ?.SendPackage((NetPackage) NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType,
                            _senderEntityId, _msg, _mainName, _localizeMain, (List<int>) null));
            }
            else
                SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(
                    (NetPackage) NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg,
                        _mainName, _localizeMain, (List<int>) null), true);
        }
        else
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer((NetPackage) NetPackageManager
                .GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, _mainName, _localizeMain,
                    _recipientEntityIds));
    }
}