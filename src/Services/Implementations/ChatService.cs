using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class ChatService : IChatService
{
    public void SendMessage(string text)
    {
        GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, text, "BackupMod", false, null);
    }
}