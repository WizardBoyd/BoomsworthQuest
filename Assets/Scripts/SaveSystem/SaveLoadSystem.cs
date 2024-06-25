using System;
using System.Text;
using Events.ScriptableObjects;
using Misc.Singelton;
using SaveSystem.SaveData;
using UnityEngine;
using WizardSave;

namespace SaveSystem
{
    public class SaveLoadSystem : MonoBehaviorSingleton<SaveLoadSystem>
    {
        public const string ApplicationStatusName = "AppStatus";
        public const string PlayerSettingDataName = "DefaultPlayerSettings";
        public const string GameProgressDataName = "DefaultGameProgress";
        public const string PlayerResourceDataName = "DefaultResourceData";
        
        [field: SerializeField]
        public PlayerSettingsData PlayerSettingData { get; private set; }
        [field: SerializeField]
        public GameProgressData GameProgressData { get; private set; }
        [field: SerializeField]
        public PlayerResourceData PlayerResourceData { get; private set; }
        
        [field: SerializeField]
        public ApplicationStatus ApplicationStatus { get; private set; }

        [Header("Listening On")] 
        [SerializeField]
        private VoidEventChannelSO m_savePlayerSettingsChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_saveGameProgressChannel = default;
        [SerializeField]
        private VoidEventChannelSO m_savePlayerResourcesChannel = default;
        
        
        protected IDataService m_dataService;

        protected override void Awake()
        {
            base.Awake();
            m_dataService = new FileDataService<string, JsonDataReadWriter>.
                    FileDataServiceBuilder<string, JsonDataReadWriter>()
                .WithSerializer(new JsonSerializer(Encoding.UTF8, true))
                .WithFileExtension("data")
                .WithDataPath(Application.persistentDataPath)
                .Build();
          
                //Load all the default files
        }

        private void OnEnable()
        {
            m_saveGameProgressChannel.OnEventRaised += SaveGameProgress;
            m_savePlayerSettingsChannel.OnEventRaised += SavePlayerSettings;
            m_savePlayerResourcesChannel.OnEventRaised += SaveResourceData;
        }
        
        private void OnDisable()
        {
            m_saveGameProgressChannel.OnEventRaised -= SaveGameProgress;
            m_savePlayerSettingsChannel.OnEventRaised -= SavePlayerSettings;
            m_savePlayerResourcesChannel.OnEventRaised -= SaveResourceData;
        }

        private void SavePlayerSettings() => m_dataService.Save(PlayerSettingData);
        public void LoadPlayerSettings(string settingName) => PlayerSettingData = m_dataService.Load<PlayerSettingsData>(settingName);
        
        private void SaveResourceData() => m_dataService.Save(PlayerResourceData);
        public void LoadResourceData(string settingName) => PlayerResourceData = m_dataService.Load<PlayerResourceData>(settingName);
        
        private void SaveGameProgress() => m_dataService.Save(GameProgressData);
        public void LoadGameProgress(string settingName) => GameProgressData = m_dataService.Load<GameProgressData>(settingName);
        
        private void SaveAppData() => m_dataService.Save(ApplicationStatus);
        public void LoadAppData(string settingName) => ApplicationStatus = m_dataService.Load<ApplicationStatus>(settingName);
        
        public void DeleteFile(string fileName) => m_dataService.Delete(fileName);

        private void OnDestroy()
        {
            //The first time should always be off after a destory
            ApplicationStatus.FirstLaunch = false;
            m_dataService.Save(ApplicationStatus);
            
            m_dataService.Save(PlayerSettingData);
            m_dataService.Save(PlayerResourceData);
            m_dataService.Save(GameProgressData);
        }
    }
}