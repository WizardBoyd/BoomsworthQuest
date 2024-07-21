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
using SaveSystem;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WizardAddressables.Runtime.AssetManagement;
using WizardOptimizations.Runtime.Singelton;
using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;

namespace Levels
{
    /// <summary>
    /// The Job of this manager is to gather any new level data gathered from the addressable system
    /// and persist it to the local storage.
    /// also if a level is complete it will update the level data to reflect that.
    /// </summary>
    public class LevelManager : MonoBehaviorSingleton<LevelManager>, ISavableData
    {
        
        public ISaveableKeyValueStore SaveContainer { get; set; }
        
        public string FilePath
        {
            get => "LevelData";
        }
        
        private ObjectSerializerMap m_objectSerializerMap;
        
        private SerializedCurrentLevelProgression m_serializedCurrentLevelProgression;

        private AsyncOperationHandle ZoneLoadHandle;
        
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
            if (ZoneLoadHandle.IsValid() && !ZoneLoadHandle.IsDone)
            {
                yield return ZoneLoadHandle;
                Debug.Log("Zone Load Handle is done");
            }
        }

        public void NewSave(ObjectSerializerMap objectSerializerMap)
        {
            m_objectSerializerMap = objectSerializerMap;
            m_serializedCurrentLevelProgression = new SerializedCurrentLevelProgression();
            m_serializedCurrentLevelProgression.CurrentLevelIndex = 0;
            m_serializedCurrentLevelProgression.CurrentZoneIndex = 0;
            SaveContainer.SetObject(objectSerializerMap, "CurrentLevelProgression", m_serializedCurrentLevelProgression);
            ZoneLoadHandle = AssetManager.Instance.LoadAssetsByLabelAsync<ZoneSO>("Zone", CreateSerializedZone);
            ZoneLoadHandle.Completed += (handle) =>
            {
                m_serializedZones = m_zoneToSerializedZone.Values.OrderBy(x => x.ZoneIndex).ToList();
                m_serializedZones[0].Levels[0].CurrentlyLocked = false;
                m_zoneSos = m_zoneSos.OrderBy(x => x.ZoneIndex).ToList();
                SaveContainer.SetObject(objectSerializerMap, "SerializedZones", m_serializedZones);
                SaveContainer.Save();
            };
        }
        
        public void Load(ObjectSerializerMap objectSerializerMap)
        {
            m_objectSerializerMap = objectSerializerMap;
            SaveContainer.Load();
            if (!SaveContainer.TryGetObject<List<SerializedZone>>(objectSerializerMap, "SerializedZones",
                    out var tempSerializedZones))
            {
                NewSave(objectSerializerMap);
                return;
            }
            m_serializedZones = tempSerializedZones;
            m_serializedCurrentLevelProgression =
                SaveContainer.GetObject<SerializedCurrentLevelProgression>("CurrentLevelProgression",
                    m_serializedCurrentLevelProgression);
            ZoneLoadHandle = AssetManager.Instance.LoadAssetsByLabelAsync<ZoneSO>("Zone", LoadSerializedList);
            ZoneLoadHandle.Completed += (handle) =>
            {
                m_serializedZones = m_zoneToSerializedZone.Values.OrderBy(x => x.ZoneIndex).ToList();
                m_zoneSos = m_zoneSos.OrderBy(x => x.ZoneIndex).ToList();
            };
        }
        
        public void Save(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.SetObject(objectSerializerMap, "SerializedZones", m_serializedZones);
            SaveContainer.SetObject(objectSerializerMap, "CurrentLevelProgression", m_serializedCurrentLevelProgression);
            SaveContainer.Save();
        }

        public void DeleteData()
        {
            SaveContainer.DeleteKey("SerializedZones");
            SaveContainer.DeleteKey("CurrentLevelProgression");
            SaveContainer.Save();
        }

        private void LoadSerializedList(object key, AsyncOperationHandle<ZoneSO> zoneSoHandle)
        {
            SerializedZone serializedZone = m_serializedZones.FirstOrDefault(x => x.ZoneSoAssetKey == key.ToString());
            if(m_zoneToSerializedZone.ContainsKey(zoneSoHandle.Result))
                return;
            m_zoneToSerializedZone.Add(zoneSoHandle.Result, serializedZone);
            m_zoneSos.Add(zoneSoHandle.Result);
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
            if(levelCompletionStatus > level.CompletionStatus)
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

                SaveContainer.SetObject(ObjectSerializerMap.DefaultSerializerMap, "CurrentLevelProgression",
                    m_serializedCurrentLevelProgression);
            }
            SaveContainer.SetObject(m_objectSerializerMap, "SerializedZones", m_serializedZones);
            //TODO uncomment the save
            SaveContainer.Save();
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