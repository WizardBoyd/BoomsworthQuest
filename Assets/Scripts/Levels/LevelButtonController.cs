using BaseClasses;
using Events.ScriptableObjects;

namespace Levels
{
    public class LevelButtonController : BaseController<LevelButtonModel>
    {
        public void LoadLevel(LevelLoadEventChannel broadcastingChannel)
        {
            broadcastingChannel.RaiseEvent(Model.level, true, false);
        }

        public bool IsLevelLocked() => Model.LevelState == LevelState.Locked;
    }
}