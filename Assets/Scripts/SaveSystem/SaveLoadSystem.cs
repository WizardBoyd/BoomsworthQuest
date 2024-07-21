using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DependencyInjection;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Levels.SerializableData;
using Life.SerializableData;
using Misc.FileManagment;
using UnityEngine;
using WizardOptimizations.Runtime.Singelton;
using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;

namespace SaveSystem
{
    public class SaveLoadSystem : MonoBehaviorSingleton<SaveLoadSystem>, IDependencyProvider, ISavableData
    {
        
        public ISaveableKeyValueStore SaveContainer { get; set; }

        public string FilePath
        {
            get => "ApplicationStatus";
        }
        
        private Dictionary<string, ISavableData> m_keyValueStores = new Dictionary<string, ISavableData>();
        
        #region Dependency Injection
        
        [Provide]
        private ObjectSerializerMap ProvideObjectSerializerMap()
        {
            ObjectSerializerMap objectSerializerMap = new ObjectSerializerMap();
            objectSerializerMap.DefaultSerializer = new NewtonsoftJsonTextSerializer();
            IDictionary<Type, IObjectSerializer> objectSerializers = new Dictionary<Type, IObjectSerializer>();
            AddTypes(ref objectSerializers);
            objectSerializerMap.TypeToSerializeMap = objectSerializers;
            return objectSerializerMap;
        }
        #endregion
        
        #region Initialization
        protected override void Awake()
        { 
            base.Awake();
            List<ISavableData> savableInstances = GatherSavableInstances();
            foreach (ISavableData instance in savableInstances)
            {
                instance.SaveContainer = new DictionaryKeyValueStore()
                {
                    FilePath = Path.Combine(Application.persistentDataPath, instance.FilePath + ".json")
                };
                if(m_keyValueStores.ContainsKey(instance.FilePath))
                    continue;
                m_keyValueStores.Add(instance.FilePath, instance);
                
                if(!FileManager.FileExists(Path.Combine(Application.persistentDataPath, instance.FilePath + ".json")))
                    instance.NewSave(ProvideObjectSerializerMap());
                else
                    instance.Load(ProvideObjectSerializerMap());
            }
        }
        private List<ISavableData> GatherSavableInstances()
        {
            //Uses the service locate pattern to find all mono behaviors that implement the ISavableData interface
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<ISavableData>().ToList();
        }
        
        private void AddTypes(ref IDictionary<Type, IObjectSerializer> serializers)
        {
            UnityMathTextSerializer mathSerializer = new UnityMathTextSerializer();
            NewtonsoftJsonTextSerializer jsonSerializer = new NewtonsoftJsonTextSerializer();
            serializers.Add(typeof(Vector2),mathSerializer);
            serializers.Add(typeof(Vector2Int),mathSerializer);
            serializers.Add(typeof(DateTime), jsonSerializer);
            serializers.Add(typeof(List<SerializedZone>), jsonSerializer);
            serializers.Add(typeof(CurrentLifeData) , jsonSerializer);
        }

        #endregion

        private void OnDestroy()
        {
            SaveContainer.SetObject(ProvideObjectSerializerMap(),"AppCloseTimeStamp", DateTime.Now);
            foreach (ISavableData savableData in m_keyValueStores.Values)
            {
                savableData.Save(ProvideObjectSerializerMap());
            }
        }

        
        public void NewSave(ObjectSerializerMap objectSerializerMap)
        {
           
            SaveContainer.SetBool("SoundOn", true);
            SaveContainer.SetBool("SFXOn", true);
            SaveContainer.SetBool("MusicOn", true);
            SaveContainer.SetObject(objectSerializerMap,"AppOpenTimeStamp", DateTime.Now);
            SaveContainer.Save();
        }

        public void Save(ObjectSerializerMap objectSerializerMap) => SaveContainer.Save();

        public void Load(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.Load();
        }

        public void DeleteData()
        {
            SaveContainer.DeleteKey("SoundOn");
            SaveContainer.DeleteKey("SFXOn");
            SaveContainer.DeleteKey("MusicOn");
            SaveContainer.DeleteKey("AppOpenTimeStamp");
            SaveContainer.DeleteKey("AppCloseTimeStamp");
            SaveContainer.Save();
        }
        
        public ISaveableKeyValueStore GetSaveContainer(string filePath)
        {
            if (m_keyValueStores.ContainsKey(filePath))
            {
                return m_keyValueStores[filePath].SaveContainer;
            }
            return null;
        }
        
        public void DeleteSaveContainer(string filePath)
        {
            if (m_keyValueStores.ContainsKey(filePath))
            {
                m_keyValueStores[filePath].DeleteData();
            }
        }
        public void SaveDataContainer(string filePath)
        {
            if (m_keyValueStores.ContainsKey(filePath))
            {
                m_keyValueStores[filePath].Save(ProvideObjectSerializerMap());
            }
        }
    }
}