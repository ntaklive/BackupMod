namespace NtakliveBackupMod.Services.Abstractions;

public interface IThreadManager
{
    public ThreadManager.TaskInfo AddSingleTask(
        ThreadManager.TaskFunctionDelegate taskDelegate,
        object parameter = null,
        ThreadManager.ExitCallbackTask exitCallback = null,
        bool exitCallbackOnMainThread = false,
        bool endEvent = true);
}