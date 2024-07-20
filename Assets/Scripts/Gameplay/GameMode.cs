using System;
using System.Collections;
using Events.ScriptableObjects;
using Gameplay;
using Levels.Enums;
using SceneManagment.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using WizardOptimizations.Runtime.Singelton;

namespace Gameplay
{
    public class GameMode : MonoBehaviorSingleton<GameMode>
    {
        private CollectableStarManager m_collectableStarManager;
        private Target m_target;
        private Canon m_canon;
        
        [Header("Configuration")]
        [SerializeField]
        private Target m_targetPrefab = default;
        [SerializeField][Range(0f,2f)]
        private float m_timeToWaitBeforeWinScreen = 1f;
        
        [Header("Broadcasting On")]
        [SerializeField]
        private LevelCompletionEventChannelSO m_onLevelComplete = default;

        protected override void Awake()
        {
            base.Awake();
            m_collectableStarManager = FindObjectOfType<CollectableStarManager>();
        }

        private void Start()
        {
            GameObject gameObjectWithTargetTag = GameObject.FindGameObjectWithTag("TargetPlacement");
            if(gameObjectWithTargetTag == null)
            {
                Debug.LogError("No Target Placement Found");
                return;
            }
            m_target = Instantiate(m_targetPrefab, gameObjectWithTargetTag.transform.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(m_target.gameObject, gameObjectWithTargetTag.scene);
            Destroy(gameObjectWithTargetTag);
            m_target.OnTargetHit += On_TargetHit;
        }

        private void On_TargetHit()
        {
            m_target.OnTargetHit -= On_TargetHit;
            //Need to start a little bit of timer for dramatic effect before showing the win screen
            StartCoroutine(On_TargetHitRoutine());
        }
        
        private IEnumerator On_TargetHitRoutine()
        {
            yield return new WaitForSeconds(m_timeToWaitBeforeWinScreen);
            m_onLevelComplete.RaiseEvent(m_collectableStarManager.GetLevelCompletionStatus());
        }
    }
}