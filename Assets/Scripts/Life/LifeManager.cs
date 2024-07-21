using System;
using System.IO;
using DependencyInjection;
using DependencyInjection.attributes;
using Life.SerializableData;
using Misc.FileManagment;
using SaveSystem;
using UnityEngine;
using WizardOptimizations.Runtime.Singelton;
using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;
using Timer = WizardOptimizations.Runtime.Timer.Timer;

namespace Life
{
    public class LifeManager : MonoBehaviorSingleton<LifeManager>, IDependencyProvider, ISavableData
    {
        Timer m_lifeTimer;

        public ISaveableKeyValueStore SaveContainer { get; set; }
        public string FilePath { get => "LifeData"; }
        
        private ObjectSerializerMap m_objectSerializerMap;
        
        private CurrentLifeData m_currentLifeData;
        
        [Provide]
        private CurrentLifeData ProvideCurrentLifeData()
        {
            return m_currentLifeData;
        }

        public void NewSave(ObjectSerializerMap objectSerializerMap)
        {
            m_objectSerializerMap = objectSerializerMap;
            m_currentLifeData = new CurrentLifeData
            {
                CurrentLifeCount = 5,
                MaxLifeCount = 5
            };
            SaveContainer.SetObject(objectSerializerMap, "CurrentLifeData", m_currentLifeData);
            SaveContainer.Save();
        }

        public void Save(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.Save();
        }

        public void Load(ObjectSerializerMap objectSerializerMap)
        {
            m_objectSerializerMap = objectSerializerMap;
            m_currentLifeData = SaveContainer.GetObject<CurrentLifeData>("CurrentLifeData", new CurrentLifeData());
        }

        public void DeleteData()
        {
            SaveContainer.DeleteKey("CurrentLifeData");
            SaveContainer.Save();
        }
    }
}