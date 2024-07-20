using System;
using Levels.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using WizardOptimizations.Runtime.Singelton;

namespace Gameplay
{
    public class CollectableStarManager : MonoBehaviorSingleton<CollectableStarManager>
    {
        public const int NumberOfStars = 3;
        
        [Header("Configuration")]
        [SerializeField] private CollectableStar m_starPrefab = default;
        
        private CollectableStar[] m_stars;
        
        private int m_collectedStars = 0;

        private void Start()
        {
            GameObject[] gameObjectsWithStarPlacementTag = GameObject.FindGameObjectsWithTag("StarPlacement");
            if(gameObjectsWithStarPlacementTag == null || gameObjectsWithStarPlacementTag.Length < NumberOfStars)
            {
                Debug.LogError("No Star Placement Found");
                return;
            }
            m_stars = new CollectableStar[NumberOfStars];
            for (int i = 0; i < NumberOfStars; i++)
            {
                GameObject starPlacement = gameObjectsWithStarPlacementTag[i];
                m_stars[i] = Instantiate(m_starPrefab, starPlacement.transform.position, Quaternion.identity);
                m_stars[i].OnStarCollected += On_StarCollected;
                SceneManager.MoveGameObjectToScene(m_stars[i].gameObject, starPlacement.scene);
                Destroy(starPlacement);
            }
        }

        private void On_StarCollected(CollectableStar star)
        {
            if(m_collectedStars < NumberOfStars && m_collectedStars >= 0)
            {
                m_collectedStars++;
                star.OnStarCollected -= On_StarCollected;
                Destroy(star.gameObject);
                //TODO could spawn particles here if I wanted to
            }
        }
        
        public LevelCompletionStatus GetLevelCompletionStatus()
        {
            switch (m_collectedStars)
            {
                case 0:
                    return LevelCompletionStatus.NoStarCompletion;
                case 1:
                    return LevelCompletionStatus.OneStarCompletion;
                case 2:
                    return LevelCompletionStatus.TwoStarCompletion;
                case 3:
                    return LevelCompletionStatus.ThreeStarCompletion;
                default:
                    return LevelCompletionStatus.Unkown;
                
            }
        }
    }
}