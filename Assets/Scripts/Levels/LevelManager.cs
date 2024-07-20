using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Levels.Enums;
using Levels.ScriptableObjects;
using Levels.SerializableData;
using Misc.FileManagment;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WizardAddressables.Runtime.AssetManagement;
using WizardOptimizations.Runtime.Singelton;
using WizardSave;
using WizardSave.ObjectSerializers;

namespace Levels
{
    /// <summary>
    /// The Job of this manager is to gather any new level data gathered from the addressable system
    /// and persist it to the local storage.
    /// also if a level is complete it will update the level data to reflect that.
    /// </summary>
    public class LevelManager : MonoBehaviorSingleton<LevelManager>
    {

        private IList<IResourceLocation> m_zoneResourceLocations = new List<IResourceLocation>();
        
        private DictionaryKeyValueStore m_levelDataStore;
        [Inject]
        private ObjectSerializerMap m_objectSerializerMap;

        private SerializedCurrentLevelProgression m_serializedCurrentLevelProgression;
        
        [Header("Listening On")]
        [SerializeField]
        private LoadLevelEventChannelSO m_onLevelStartPlayEvent;
        [SerializeField]
        private LevelCompletionEventChannelSO m_onLevelCompleteEvent;
#if UNITY_EDITOR
        [SerializeField]
        private LoadSceneEventChannelSO m_onColdStartUpEvent;
#endif
        [SerializeField]
        private VoidEventChannelSO m_LoadNextAvailableLevelEvent;
        [SerializeField]
        private VoidEventChannelSO m_RetryLevel = default;
        
        [Header("Broadcasting On")]
        [SerializeField]
        private LoadLevelEventChannelSO m_onLoadLevelEvent;
        
        public List<SerializedZone> m_serializedZones { get; private set; } = new List<SerializedZone>();
        public List<ZoneSO> m_zoneSos { get; private set; } = new List<ZoneSO>();
        public Dictionary<ZoneSO, SerializedZone> m_zoneToSerializedZone { get; private set;} = new Dictionary<ZoneSO, SerializedZone>();

        private LevelSceneSO m_currentPlayingLevelSceneSo;
        
        
        

        protected override void Awake()
        {
            base.Awake();
            m_levelDataStore = new DictionaryKeyValueStore();
            m_levelDataStore.FilePath = Path.Combine(Application.persistentDataPath, "LevelData.json");
        }

        private void OnEnable()
        {
            m_onLevelStartPlayEvent.OnLoadingRequested += OnLevelStartPlay;
            m_onLevelCompleteEvent.OnEventRaised += OnLevelComplete;
            m_LoadNextAvailableLevelEvent.OnEventRaised += OnLoadNextAvailableLevel;
            m_RetryLevel.OnEventRaised += OnRetryLevel;
#if UNITY_EDITOR
            m_onColdStartUpEvent.OnLoadingRequested += ColdStartUp;
#endif
        }

        private void OnDisable()
        {
            m_onLevelStartPlayEvent.OnLoadingRequested -= OnLevelStartPlay;
            m_onLevelCompleteEvent.OnEventRaised -= OnLevelComplete;
            m_LoadNextAvailableLevelEvent.OnEventRaised -= OnLoadNextAvailableLevel;
            m_RetryLevel.OnEventRaised -= OnRetryLevel;
#if UNITY_EDITOR
            m_onColdStartUpEvent.OnLoadingRequested -= ColdStartUp;
#endif
        }
        

        private IEnumerator Start()
        {
            yield return GetZoneSos();
            m_serializedZones = m_serializedZones.OrderBy(x => x.ZoneIndex).ToList();
            m_zoneSos = m_zoneSos.OrderBy(x => x.ZoneIndex).ToList();
            yield return CreateOrLoadLevelData();
        }

        private IEnumerator CreateOrLoadLevelData()
        {
            if (!FileManager.FileExists(m_levelDataStore.FilePath))
            {
                //Create a new level data file
                yield return CreateNewLevelData();
            }
            else
            {
                //Load the level data file
                yield return LoadLevelData();
            }
        }

        private IEnumerator CreateNewLevelData()
        {
            SerializedZone firstZone = m_serializedZones[0];
            if (firstZone == null)
            {
                Debug.LogError($"No Zone has a ZoneIndex of 0");
                yield break;
            }
            firstZone.Levels[0].CurrentlyLocked = false;
            m_serializedCurrentLevelProgression = new SerializedCurrentLevelProgression();
            m_serializedCurrentLevelProgression.CurrentLevelIndex = 0;
            m_serializedCurrentLevelProgression.CurrentZoneIndex = 0;
            m_levelDataStore.SetObject(m_objectSerializerMap, "CurrentLevelProgression", m_serializedCurrentLevelProgression);
            m_levelDataStore.SetObject(m_objectSerializerMap, "SerializedZones", m_serializedZones);
            m_levelDataStore.Save();
        }
        
        private void CreateSerializedZone(object key, AsyncOperationHandle<ZoneSO> handle)
        {
            SerializedZone serializedZone = new SerializedZone(key.ToString(), handle.Result.ZoneIndex);
            serializedZone.Levels = new SerializedLevel[handle.Result.Levels.Length];
            for(int i = 0 ; i < handle.Result.Levels.Length; i++)
            {
                SerializedLevel serializedLevel = new SerializedLevel();
                serializedLevel.LevelIndex = handle.Result.Levels[i].LevelIndex;
                serializedLevel.CurrentlyLocked = true;
                serializedLevel.CompletionStatus = LevelCompletionStatus.Unkown;
                serializedZone.Levels[i] = serializedLevel;
            }
            m_zoneToSerializedZone.Add(handle.Result, serializedZone);
            m_zoneSos.Add(handle.Result);
            m_serializedZones.Add(serializedZone);
        }
        private IEnumerator GetZoneSos()
        {
            var handle = AssetManager.Instance.LoadAssetsByLabelAsync<ZoneSO>("Zone", CreateSerializedZone);
            yield return handle;
        }
        
        //Loading
        private IEnumerator LoadLevelData()
        {
            m_levelDataStore.Load();
            m_serializedCurrentLevelProgression =
                m_levelDataStore.GetObject<SerializedCurrentLevelProgression>("CurrentLevelProgression",
                    m_serializedCurrentLevelProgression);
            yield return LoadSerializedLevelIntoList();
            m_levelDataStore.SetObject(m_objectSerializerMap, "SerializedZones", m_serializedZones);
            m_levelDataStore.Save();
        }
        
        private IEnumerator LoadSerializedLevelIntoList()
        {
            var tempZoneToSerializedZone = new Dictionary<ZoneSO, SerializedZone>();
            if (!m_levelDataStore.TryGetObject<List<SerializedZone>>(m_objectSerializerMap, "SerializedZones",
                    out var mSerializedZones))
            {
                Debug.LogError($"Failed to load SerializedZones into list");
            }

            foreach (SerializedZone serializedZone in mSerializedZones)
            {
                if (!AssetManager.Instance.TryGetOrLoadObjectAsync<ZoneSO>(serializedZone.ZoneSoAssetKey,
                        out AsyncOperationHandle<ZoneSO> zoneHandle))
                {
                    //if the zone is not loaded then we need to wait for it to load
                    if (!zoneHandle.IsDone)
                        yield return zoneHandle;
                    if (zoneHandle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Debug.LogError($"Failed to load ZoneSO from Addressables");
                        continue;
                    }
                }
                tempZoneToSerializedZone.Add(zoneHandle.Result, serializedZone);
            }
            m_zoneToSerializedZone = tempZoneToSerializedZone;
            m_serializedZones = mSerializedZones;
        }
        
        private void OnLevelStartPlay(LevelSceneSO levelSceneSo, bool showLoadingScreen, bool fadeScreen)
        {
            m_currentPlayingLevelSceneSo = levelSceneSo;
        }

#if UNITY_EDITOR
        private void ColdStartUp(GameSceneSO currentlyPlayingScene, bool showLoadingScreen, bool fadeScreen)
        {
            if(currentlyPlayingScene.SceneType != GameSceneSO.GameSceneType.Level)
                return;
            m_currentPlayingLevelSceneSo = (LevelSceneSO) currentlyPlayingScene;
        }
#endif

        private void OnLevelComplete(LevelCompletionStatus levelCompletionStatus)
        {
            SerializedZone zone = m_zoneToSerializedZone[m_currentPlayingLevelSceneSo.BelongingZone];
            SerializedLevel level = zone.Levels.FirstOrDefault(x => x.LevelIndex == m_currentPlayingLevelSceneSo.LevelIndex);
            if(level == null)
                return;
            level.CompletionStatus = levelCompletionStatus;
            //what if we re-complete an already completed level?
            if (!IsLevelAlreadyComplete(zone, level))
            {
                if (IsLevelLastInZone(zone, level))
                {
                    //Move the state to the next zone and the first level in that next zone
                    zone = m_serializedZones.FirstOrDefault(x =>
                        x.ZoneIndex == m_serializedCurrentLevelProgression.CurrentZoneIndex + 1);
                    if (zone == null)
                    {
                        //At this point there should be no more zones in the game to move to
                        return;
                    }
                    //Set the level to unlocked
                    if(zone.Levels.Length <= 0)
                        return;
                    level = zone.Levels[0];
                    level.CurrentlyLocked = false;
                    m_serializedCurrentLevelProgression.CurrentZoneIndex = zone.ZoneIndex;
                    m_serializedCurrentLevelProgression.CurrentZoneIndex = 0;
                }
                else
                {
                    //move the state just to the next level
                    m_serializedCurrentLevelProgression.CurrentLevelIndex = level.LevelIndex + 1;
                    //Set the level to unlocked
                    zone.Levels[level.LevelIndex + 1].CurrentlyLocked = false;
                }

                m_levelDataStore.SetObject(m_objectSerializerMap, "CurrentLevelProgression",
                    m_serializedCurrentLevelProgression);
            }
            m_levelDataStore.SetObject(m_objectSerializerMap, "SerializedZones", m_serializedZones);
            //TODO uncomment the save
            m_levelDataStore.Save();
        }
        
        private bool IsLevelLastInZone(SerializedZone zone, SerializedLevel level)
        {
            return zone.Levels.Last() == level;
        }
        
        private bool IsLevelAlreadyComplete(SerializedZone zone, SerializedLevel level)
        {
            if(m_serializedCurrentLevelProgression.CurrentZoneIndex > zone.ZoneIndex)
                return true;
            if(m_serializedCurrentLevelProgression.CurrentLevelIndex > level.LevelIndex)
                return true;
            return false;
        }
        
        private void OnLoadNextAvailableLevel()
        {
            var currentZone = m_currentPlayingLevelSceneSo.BelongingZone;
            SerializedZone zone = m_zoneToSerializedZone[currentZone];
            SerializedLevel level = zone.Levels.FirstOrDefault(x => x.LevelIndex == m_currentPlayingLevelSceneSo.LevelIndex);
            LevelSceneSO nextLevel = null;
            if (IsLevelLastInZone(zone, level))
            {
                //Get the next zone
               var nextZone = m_zoneToSerializedZone.FirstOrDefault(x => x.Key.ZoneIndex == currentZone.ZoneIndex + 1);
               nextLevel = nextZone.Key.Levels[0];
               if (nextLevel == null)
               {
                   Debug.LogError("No next level found");
                   return;
               }
            }
            else
            {
                nextLevel = currentZone.Levels[m_currentPlayingLevelSceneSo.LevelIndex + 1];
            }
            m_onLoadLevelEvent.RaiseEvent(nextLevel, true, true);
        }
        
        private void OnRetryLevel()
        {
            m_onLoadLevelEvent.RaiseEvent(m_currentPlayingLevelSceneSo, true, true);
        }
      
        
    }
}