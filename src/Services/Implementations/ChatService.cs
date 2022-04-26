using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class ChatService : IChatService
{
    public void SendMessage(string text)
    {
        GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, text, "BackupMod", false, null);
    }
}