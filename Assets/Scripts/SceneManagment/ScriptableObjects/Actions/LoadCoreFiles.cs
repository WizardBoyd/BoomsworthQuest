using SaveSystem;
using UnityEngine;

namespace SceneManagment.ScriptableObjects.Actions
{
    [CreateAssetMenu(fileName = "Load Core Files Action", menuName = "Scene/Scene Actions/Load Core Files Action", order = 0)]
    public class LoadCoreFiles : BaseSceneChangeAction
    {
        public override void PerformAction()
        {
            if (SaveLoadSystem.Instance != null)
            {
#if UNITY_EDITOR
                Debug.Log("Loading Core Files");
#endif
                SaveLoadSystem.Instance.LoadAppData(SaveLoadSystem.ApplicationStatusName);
                SaveLoadSystem.Instance.LoadPlayerSettings(SaveLoadSystem.PlayerSettingDataName);
                SaveLoadSystem.Instance.LoadGameProgress(SaveLoadSystem.GameProgressDataName);
                SaveLoadSystem.Instance.LoadResourceData(SaveLoadSystem.PlayerResourceDataName);
            }
        }
    }
}