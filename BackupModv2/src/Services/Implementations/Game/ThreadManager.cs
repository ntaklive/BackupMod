using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class ThreadManager : IThreadManager
{
    public global::ThreadManager.TaskInfo AddSingleTask(
        global::ThreadManager.TaskFunctionDelegate taskDelegate,
        object parameter = null, global::ThreadManager.ExitCallbackTask exitCallback = null,
        bool exitCallbackOnMainThread = false, bool endEvent = true)
    {
        return global::ThreadManager.AddSingleTask(taskDelegate, parameter, exitCallback, endEvent);
    }
}