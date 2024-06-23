using System;
using BaseClasses;
using Events.ScriptableObjects;
using Levels.Enums;
using Levels.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class LevelButton : MonoBehaviour
    {
        [Header("Stars")] 
        [SerializeField] private RectTransform LeftStar;
        [SerializeField] private RectTransform MiddleStar;
        [SerializeField] private RectTransform RightStar;

        [Header("Misc")] 
        [SerializeField] private RectTransform Lock;
        [SerializeField] private TMP_Text numberText;

        private Button button;

        public LevelSO AssignedLevel
        {
            get => m_assignedLevel;
            set
            {
                if (value == m_assignedLevel)
                {
                    return;
                }
                m_assignedLevel = value;
                UpdateButtonView();
            }
        }
        private LevelSO m_assignedLevel;

        [Header("Broadcasting On Event")] 
        [SerializeField] private LevelLoadEventChannel On_LevelClicked;

        protected void Awake()
        {
            button = GetComponentInChildren<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(On_ButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(On_ButtonClicked);
        }

        private void UpdateButtonView()
        {
            UpdateStarView();
            if (AssignedLevel.LevelLocked)
            {
                Lock.gameObject.SetActive(true);
                button.interactable = false;
            }
            else
            {
                Lock.gameObject.SetActive(false);
                //Activate the button intractable state
                button.interactable = true;
            }
            numberText.text = AssignedLevel.LevelIndex.ToString();
        }
        
        
        private void UpdateStarView()
        {
            LeftStar.gameObject.SetActive(false);
            MiddleStar.gameObject.SetActive(false);
            RightStar.gameObject.SetActive(false);
            
            if (AssignedLevel.LevelCompletionStatus == LevelCompletionStatus.Unkown)
            {
               
                return;
            }
            else
            {
                switch (m_assignedLevel.LevelCompletionStatus)
                {
                    case LevelCompletionStatus.OneStarCompletion:
                        LeftStar.gameObject.SetActive(true);
                        break;
                    case LevelCompletionStatus.TwoStarCompletion:
                        LeftStar.gameObject.SetActive(true);
                        MiddleStar.gameObject.SetActive(true);
                        break;
                    case LevelCompletionStatus.ThreeStarCompletion:
                        LeftStar.gameObject.SetActive(true);
                        MiddleStar.gameObject.SetActive(true);
                        RightStar.gameObject.SetActive(true);
                        break;
                }
            }
        }

        private void On_ButtonClicked()
        {
            On_LevelClicked.RaiseEvent(AssignedLevel);
        }
    }
}