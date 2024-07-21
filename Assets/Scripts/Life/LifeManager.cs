using System;
using System.IO;
using DependencyInjection;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Levels.Enums;
using Life.SerializableData;
using Misc.FileManagment;
using SaveSystem;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using WizardOptimizations.Runtime.Singelton;
using WizardOptimizations.Runtime.Timer;
using WizardSave;
using WizardSave.ObjectSerializers;
using WizardSave.Utils;
using Timer = WizardOptimizations.Runtime.Timer.Timer;

namespace Life
{
    public class LifeManager : MonoBehaviorSingleton<LifeManager>, IDependencyProvider, ISavableData
    {
        public Timer m_lifeTimer { get; private set; }

        public ISaveableKeyValueStore SaveContainer { get; set; }
        public string FilePath { get => "LifeData"; }
        
        private ObjectSerializerMap m_objectSerializerMap;
        
        private CurrentLifeData m_currentLifeData;
        
        [Header("Listening To")]
        [SerializeField]
        private IntEventChannelSO m_OnLifeGainedExternally;
        [SerializeField]
        private IntEventChannelSO m_OnLifeLostExternally;
        [SerializeField]
        private LevelCompletionEventChannelSO m_OnLevelComplete;
        [SerializeField]
        private VoidEventChannelSO m_OnLevelFailed;
        
        [Header("Broadcasting on")]
        [SerializeField]
        private IntEventChannelSO m_OnLifeCountChanged;
        [SerializeField]
        private VoidEventChannelSO m_AttemptedToPlayWithoutLife;
        
        [Header("Configurations")]
        [SerializeField][Range(60, 600)]
        private float m_lifeRegenTime = 60f;
        
        [field: SerializeField]
        public int LifeRegenMinutes{get => (int)m_lifeRegenTime / 60;}
        [field: SerializeField]
        public int LifeRegenSeconds{get => (int)m_lifeRegenTime % 60;}
        
        private void OnEnable()
        {
            m_OnLifeGainedExternally.OnEventRaised += RegenLife;
            m_OnLifeLostExternally.OnEventRaised += LossLife;
            m_OnLevelComplete.OnEventRaised += LossLife;
            m_OnLevelFailed.OnEventRaised += LossLife;
        }

        private void OnDisable()
        {
            m_OnLifeGainedExternally.OnEventRaised -= RegenLife;
            m_OnLifeLostExternally.OnEventRaised -= LossLife;
            m_OnLevelComplete.OnEventRaised -= LossLife;
            m_OnLevelFailed.OnEventRaised -= LossLife;
        }

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
                CurrentLifeCount = 0,
                MaxLifeCount = 5
            };
            SaveContainer.SetObject(objectSerializerMap, "CurrentLifeData", m_currentLifeData);
            SaveContainer.Save();
        }

        private void RegenLife()
        {
            if(m_currentLifeData.CurrentLifeCount < m_currentLifeData.MaxLifeCount)
            {
                m_currentLifeData.CurrentLifeCount++;
                SaveContainer.SetObject(m_objectSerializerMap, "CurrentLifeData", m_currentLifeData);
                SaveContainer.Save();
                m_OnLifeCountChanged.RaiseEvent(m_currentLifeData.CurrentLifeCount);
            }
            else
            {
                //Cancel the timer if the life is full 
                m_lifeTimer.Cancel();
            }
        }
        
        
        private void RegenLife(int count)
        {
            count = Mathf.Abs(count);
            if(m_currentLifeData.CurrentLifeCount < m_currentLifeData.MaxLifeCount)
            {
                if(m_currentLifeData.CurrentLifeCount + count < m_currentLifeData.MaxLifeCount)
                {
                    m_currentLifeData.CurrentLifeCount += count;
                    //Cancel the timer and restart it
                    m_lifeTimer.Cancel();
                    m_lifeTimer = Timer.Register(m_lifeRegenTime, RegenLife, OnUpdateTimer, true, true);
                }
                else
                {
                    //Cancel the timer if the life is full 
                    m_lifeTimer.Cancel();
                    m_currentLifeData.CurrentLifeCount = m_currentLifeData.MaxLifeCount;
                }
                SaveContainer.SetObject(m_objectSerializerMap, "CurrentLifeData", m_currentLifeData);
                SaveContainer.Save();
                m_OnLifeCountChanged.RaiseEvent(m_currentLifeData.CurrentLifeCount);
            }
        }
        
        private void LossLife(int count)
        {
            count = Mathf.Abs(count);
            if(m_currentLifeData.CurrentLifeCount > 0)
            {
                if(m_currentLifeData.CurrentLifeCount - count >= 0)
                {
                    m_currentLifeData.CurrentLifeCount -= count;
                }
                else
                {
                    m_currentLifeData.CurrentLifeCount = 0;
                }
                SaveContainer.SetObject(m_objectSerializerMap, "CurrentLifeData", m_currentLifeData);
                SaveContainer.Save();
                m_OnLifeCountChanged.RaiseEvent(m_currentLifeData.CurrentLifeCount);
            }
        }
        
        private void LossLife(LevelCompletionStatus status)=> LossLife(1);
        private void LossLife()=> LossLife(1);
       

        private void OnUpdateTimer(float timeElapsed)
        {
            if(m_currentLifeData.CurrentLifeCount >= m_currentLifeData.MaxLifeCount)
            {
                m_lifeTimer.Pause();
            }
        }
        
        private void Start()
        {
           m_lifeTimer = Timer.Register(m_lifeRegenTime, RegenLife, OnUpdateTimer, true, true);
           if(m_currentLifeData.CurrentLifeCount >= m_currentLifeData.MaxLifeCount)
           {
              m_lifeTimer.Cancel();
           }
        }

        public void Save(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.SetObject(objectSerializerMap, "CurrentLifeData", m_currentLifeData);
            SaveContainer.Save();
        }

        public void Load(ObjectSerializerMap objectSerializerMap)
        {
            SaveContainer.Load();
            m_objectSerializerMap = objectSerializerMap;
            if(SaveContainer.TryGetObject<CurrentLifeData>(objectSerializerMap, "CurrentLifeData", out CurrentLifeData currentLifeData))
            {
                m_currentLifeData = currentLifeData;
            }
            else
            {
                m_currentLifeData = new CurrentLifeData
                {
                    CurrentLifeCount = 0,
                    MaxLifeCount = 5
                };
            }
        }

        public void DeleteData()
        {
            SaveContainer.DeleteKey("CurrentLifeData");
            SaveContainer.Save();
        }
        
        public bool HasRemainingLife()
        {
            if (m_currentLifeData.CurrentLifeCount > 0)
            {
                return true;
            }
            else
            {
                m_AttemptedToPlayWithoutLife.RaiseEvent();
                return false;
            }
        }
    }
}