using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DependencyInjection;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Misc.FileManagment;
using Misc.Singelton;
using UnityEngine;
using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;

namespace SaveSystem
{
    public class SaveLoadSystem : MonoBehaviorSingleton<SaveLoadSystem>, IDependencyProvider
    {
        private DictionaryKeyValueStore m_applicationStatus;
        
        #region Dependency Injection

        [Provide]
        private AutoSaveKeyValueStoreWrapper ProvideSaveContainer()
        {
            return new AutoSaveKeyValueStoreWrapper(m_applicationStatus);
        }
        
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
            m_applicationStatus = new DictionaryKeyValueStore()
            {
                FilePath = Path.Combine(Application.persistentDataPath, "ApplicationStatus.json")
            };
            bool isApplicationStatusNew = LoadOrCreatePersistentData(m_applicationStatus) == 1;
            if(isApplicationStatusNew){
                m_applicationStatus.SetBool("FirstTime", true);
                
            }
            else
            {
                m_applicationStatus.SetBool("FirstTime", false);
            }
        }

        private int LoadOrCreatePersistentData(AStreamSavableFile savableKeyValueStore)
        {
            if(FileManager.FileExists(savableKeyValueStore.FilePath))
            {
                savableKeyValueStore.Load();
                return -1;
            }
            else
            {
                savableKeyValueStore.Save();
                return 1;
            }
        }
        
        private void AddTypes(ref IDictionary<Type, IObjectSerializer> serializers)
        {
            UnityMathTextSerializer mathSerializer = new UnityMathTextSerializer();
            serializers.Add(typeof(Vector2),mathSerializer);
            serializers.Add(typeof(Vector2Int),mathSerializer);
        }

        #endregion

        private void OnDestroy()
        {
            m_applicationStatus.Save();
        }
        
    }
}