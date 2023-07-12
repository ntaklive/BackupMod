namespace ntaklive.BackupMod.Infrastructure.Game
{
    public interface IThreadManager
    {
        public global::ThreadManager.TaskInfo AddSingleTask(
            global::ThreadManager.TaskFunctionDelegate taskDelegate,
            object parameter = null!,
            global::ThreadManager.ExitCallbackTask exitCallback = null!,
            bool exitCallbackOnMainThread = false,
            bool endEvent = true);
    }
}