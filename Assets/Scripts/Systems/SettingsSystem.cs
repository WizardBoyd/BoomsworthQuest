using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;

namespace Systems
{
    public class SettingsSystem : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO SaveSettingsEvent = default;

        [SerializeField] private SettingsSO _currentSettings = default;
        [SerializeField] private UniversalRenderPipelineAsset _urpAsset = default;
        [SerializeField] private SaveSystem.SaveSystem _saveSystem = default;

        [SerializeField] private BoolEventChannelSO _changeMasterVolumeEventChannel = default;
        [SerializeField] private BoolEventChannelSO _changeSFXVolumeEventChannel = default;
        [SerializeField] private BoolEventChannelSO _changeMusicVolumeEventChannel = default;

        private void Awake()
        {
            _saveSystem.LoadSaveDataFromDisk();
            _currentSettings.LoadSavedSettings(_saveSystem.saveData);
            SetCurrentSettings();
        }
        private void OnEnable()
        {
            SaveSettingsEvent.OnEventRaised += SaveSettings;
        }
        private void OnDisable()
        {
            SaveSettingsEvent.OnEventRaised -= SaveSettings;
        }
        /// <summary>
        /// Set current settings 
        /// </summary>
        void SetCurrentSettings()
        {
            _changeMusicVolumeEventChannel.RaiseEvent(false);//raise event for volume change
            _changeSFXVolumeEventChannel.RaiseEvent(false); //raise event for volume change
            _changeMasterVolumeEventChannel.RaiseEvent(false); //raise event for volume change
            Resolution currentResolution = Screen.currentResolution; // get a default resolution in case saved resolution doesn't exist in the resolution List
            if (_currentSettings.ResolutionsIndex < Screen.resolutions.Length)
                currentResolution = Screen.resolutions[_currentSettings.ResolutionsIndex];
            Screen.SetResolution(currentResolution.width, currentResolution.height, _currentSettings.IsFullscreen);
            _urpAsset.shadowDistance = _currentSettings.ShadowDistance;
            _urpAsset.msaaSampleCount = _currentSettings.AntiAliasingIndex;

            LocalizationSettings.SelectedLocale = _currentSettings.CurrentLocale;
        }
        void SaveSettings()
        {
            _saveSystem.SaveDataToDisk();
        }
    }
}