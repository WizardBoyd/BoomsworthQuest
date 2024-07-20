using System;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using Levels.ScriptableObjects;
using Levels.SerializableData;
using UnityEngine;
using UnityEngine.UI;
using WizardOptimizations.Runtime.Pool;
using WizardOptimizations.Runtime.Singelton;

namespace Levels
{
    [RequireComponent(typeof(RectTransform))]
    public class MapController : MonoBehaviorSingleton<MapController>
    {
        
        [Header("Pool Helper")]
        [SerializeField]
        private ComponentPoolSO<LevelButton> m_buttonPool;

        [Header("Configurations")]
        [SerializeField]
        private Vector2Int m_zoneResolutionSize;
        
        [Header("Listen On")]
        [SerializeField]
        private VoidEventChannelSO m_OnLevelReadyEvent;

        private void OnEnable()
        {
            m_OnLevelReadyEvent.OnEventRaised += InstantiateRuntimeZones;
        }


        public void InstantiateRuntimeZones()
        {
            m_OnLevelReadyEvent.OnEventRaised -= InstantiateRuntimeZones;
            foreach (var zoneSo in LevelManager.Instance.m_zoneSos)
            {
                var zone = CreateZoneTemplate(zoneSo);
                SpawnLevelButtonsForZone(zone, zoneSo);
            }
        }

        private GameObject CreateZoneTemplate(ZoneSO zoneSo)
        {
            GameObject zone = new GameObject(
                zoneSo.name + "_Runtime", typeof(RectTransform));
            RectTransform zoneTransform = zone.GetComponent<RectTransform>();
            Image image = zone.AddComponent<Image>();
            image.sprite = zoneSo.ZoneCurveLayout.ZoneImage;
            //Set the size of the rect transform
            zoneTransform.sizeDelta = new Vector2(m_zoneResolutionSize.x, m_zoneResolutionSize.y);
            //Add it as a child
            zoneTransform.SetParent(transform);
            return zone;
        }

        private void SpawnLevelButtonsForZone(GameObject zoneTemplate, ZoneSO zoneSo)
        {
            var post = zoneSo.ZoneCurveLayout.Curve.GetPositions(zoneSo.Levels.Length);
            for (int i = 0; i < zoneSo.Levels.Length; i++)
            {
                var levelButton = m_buttonPool.Request();
                levelButton.transform.SetParent(zoneTemplate.transform);
                levelButton.transform.position = zoneTemplate.transform.TransformPoint(post[i]);
                levelButton.AssignedLevel = LevelManager.Instance.m_zoneToSerializedZone[zoneSo].Levels[i];
                levelButton.LevelSceneSo = zoneSo.Levels[i];
            }
        }
        
        
        
    }
}