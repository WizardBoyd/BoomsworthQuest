using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using TMPro;
using UI.Properties;
using UnityEngine;
using WizardUI.Panel;

namespace UI.PanelControllers
{
    public class HeartPanelController : APanelController<HeartPanelProperties>
    {
        [SerializeField]
        private IntEventChannelSO m_OnLifeCountChanged;
        
        [SerializeField]
        private TMP_Text m_LifeCountText;
        
        [SerializeField]
        private List<RectTransform> m_HeartIcons;
        
        private bool SetIsFullTimer = false;
        
        protected override void AddListeners()
        {
            m_OnLifeCountChanged.OnEventRaised += OnLifeCountChanged;
        }
        
        protected override void RemoveListeners()
        {
            m_OnLifeCountChanged.OnEventRaised -= OnLifeCountChanged;
        }

        protected override void OnPropertiesSet()
        {
            SetHearts();
        }

        private void Update()
        {
            
            if (Properties.Timer != null && !Properties.Timer.isDone)
            {
                SetIsFullTimer = false;
                int minutes = (int)Properties.Timer.GetTimeRemaining() / 60;
                int seconds = (int)Properties.Timer.GetTimeRemaining() % 60;
                m_LifeCountText.text = $"{minutes:D2}:{seconds:D2}";
            }
            else
            {
                if (!SetIsFullTimer)
                {
                    SetIsFullTimer = true;
                    m_LifeCountText.text = "Full";
                }
            }

        }

        private void SetHearts()
        {
            foreach (var mHeartIcon in m_HeartIcons)
            {
                mHeartIcon.gameObject.SetActive(false);
            }

            for(int i = 0; i < Properties.CurrentLifeData.CurrentLifeCount; i++)
            {
                m_HeartIcons[i].gameObject.SetActive(true);
            }
        }

        protected override void WhileHiding()
        {
           
        }

        protected override void HierarchyFixOnShow()
        {
         
        }
        
        private void OnLifeCountChanged(int arg0)
        {
            SetHearts();
        }

    }
}