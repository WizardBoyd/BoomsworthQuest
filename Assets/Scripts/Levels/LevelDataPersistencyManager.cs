using System;
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
using Misc.Singelton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WizardSave;
using WizardSave.ObjectSerializers;

namespace Levels
{
    /// <summary>
    /// The Job of this manager is to gather any new level data gathered from the addressable system
    /// and persist it to the local storage.
    /// also if a level is complete it will update the level data to reflect that.
    /// </summary>
    public class LevelDataPersistencyManager : MonoBehaviorSingleton<LevelDataPersistencyManager>
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
        
        public List<SerializedZone> m_serializedZones { get; private set; } = new List<SerializedZone>();
        public Dictionary<ZoneSO, SerializedZone> m_zoneToSerializedZone { get; private set;} = new Dictionary<ZoneSO, SerializedZone>();

        private LevelSceneSO m_currentPlayingLevelSceneSo;
        

        protected override void Awake()
        {
            base.Awake();
            m_levelDataStore = new DictionaryKeyValueStore();
            m_levelDataStore.FilePath = Path.Combine(Application.persistentDataPath, "LevelData.json");
            GetZoneResourceLocations(out m_zoneResourceLocations);
        }

        private void OnEnable()
        {
            m_onLevelStartPlayEvent.OnLoadingRequested += OnLevelStartPlay;
            m_onLevelCompleteEvent.OnEventRaised += OnLevelComplete;
        }

        private void OnDisable()
        {
            m_onLevelStartPlayEvent.OnLoadingRequested -= OnLevelStartPlay;
            m_onLevelCompleteEvent.OnEventRaised -= OnLevelComplete;
        }

        private IEnumerator Start()
        {
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
            foreach (IResourceLocation location in m_zoneResourceLocations)
            {
                yield return LoadZoneSo(location);
            }

            SerializedZone firstZone = m_serializedZones.FirstOrDefault(x => x.ZoneIndex == 0);
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

        private IEnumerator LoadZoneSo(IResourceLocation location)
        {
            var handle = Addressables.LoadAssetAsync<ZoneSO>(location);
            if (!handle.IsDone)
                yield return handle;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Zones from Addressables");
                yield break;
            }
            SerializedZone zone = CreateSerializedZone(location, handle.Result);
            m_serializedZones.Add(zone);
            m_zoneToSerializedZone.Add(handle.Result, zone);
            Addressables.Release(handle);
        }

        private SerializedZone CreateSerializedZone(IResourceLocation location, ZoneSO so)
        {
            SerializedZone serializedZone = new SerializedZone(location.PrimaryKey, so.ZoneIndex);
            serializedZone.Levels = new SerializedLevel[so.Levels.Length];
            for(int i = 0 ; i < so.Levels.Length; i++)
            {
                SerializedLevel serializedLevel = new SerializedLevel();
                serializedLevel.LevelIndex = so.Levels[i].LevelIndex;
                serializedLevel.CurrentlyLocked = true;
                serializedLevel.CompletionStatus = LevelCompletionStatus.Unkown;
                serializedZone.Levels[i] = serializedLevel;
            }
            return serializedZone;
        }
        
        private void GetZoneResourceLocations(out IList<IResourceLocation> zoneResourceLocations)
        {
            var handle = Addressables.LoadResourceLocationsAsync("Zone", typeof(ZoneSO));
            handle.WaitForCompletion(); //Blocking Call
            zoneResourceLocations = handle.Result;
            Addressables.Release(handle); //Not sure if this will delete the resource locations
        }
        
        //Loading
        private IEnumerator LoadLevelData()
        {
            m_levelDataStore.Load();
            m_serializedCurrentLevelProgression =
                m_levelDataStore.GetObject<SerializedCurrentLevelProgression>("CurrentLevelProgression",
                    m_serializedCurrentLevelProgression);
            yield return LoadSerializedLevelIntoList();
            IList<IResourceLocation> resourceLocationsNotInSave = new List<IResourceLocation>();
            GetResourceLocationsNotInSave(out resourceLocationsNotInSave);
            foreach (IResourceLocation location in resourceLocationsNotInSave)
            {
                //Load in any new zones not in the save file
                yield return LoadZoneSo(location);
            }
            m_levelDataStore.SetObject(m_objectSerializerMap, "SerializedZones", m_serializedZones);
            m_levelDataStore.Save();
        }

        private void GetResourceLocationsNotInSave(out IList<IResourceLocation> zoneResourceLocations)
        {
            zoneResourceLocations = m_zoneResourceLocations.Where(x => m_serializedZones.All(y => y.ZoneSoAssetKey != x.PrimaryKey)).ToList();
        }

        private IEnumerator LoadSerializedLevelIntoList()
        {
            var mSerializedZones = m_serializedZones;
            if (!m_levelDataStore.TryGetObject<List<SerializedZone>>(m_objectSerializerMap, "SerializedZones",
                    out mSerializedZones))
            {
                Debug.LogError($"Failed to load SerializedZones into list");
            }

            foreach (SerializedZone serializedZone in mSerializedZones)
            {
                var handle = Addressables.LoadAssetAsync<ZoneSO>(serializedZone.ZoneSoAssetKey);
                if (!handle.IsDone)
                    yield return handle;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load ZoneSO from Addressables");
                    continue;
                }
                m_zoneToSerializedZone.Add(handle.Result, serializedZone);
                Addressables.Release(handle);
            }
            m_serializedZones = mSerializedZones;
        }
        
        private void OnLevelStartPlay(LevelSceneSO levelSceneSo, bool showLoadingScreen, bool fadeScreen)
        {
            m_currentPlayingLevelSceneSo = levelSceneSo;
        }
   

        private void OnLevelComplete(LevelCompletionStatus levelCompletionStatus)
        {
            SerializedZone zone = m_zoneToSerializedZone[m_currentPlayingLevelSceneSo.BelongingZone];
            SerializedLevel level = zone.Levels.FirstOrDefault(x => x.LevelIndex == m_currentPlayingLevelSceneSo.LevelIndex);
            m_currentPlayingLevelSceneSo = null;
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
      
        
    }
}