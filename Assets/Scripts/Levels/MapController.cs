using System;
using System.Collections;
using System.Collections.Generic;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Factory;
using Levels.ScriptableObjects;
using Levels.SerializableData;
using Misc.Singelton;
using Mkey;
using SaveSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using WizardSave;

namespace Levels
{
    [RequireComponent(typeof(RectTransform))]
    public class MapController : MonoBehaviorSingleton<MapController>
    {
        private List<SerializedZone> SerializedZones
        {
            get => LevelManager.Instance.m_serializedZones;
        }
        private Dictionary<ZoneSO, SerializedZone> m_zoneToSerializedZone
        {
            get => LevelManager.Instance.m_zoneToSerializedZone;
        }
        
        [Header("Pool Helper")]
        [SerializeField]
        private ComponentPoolSO<LevelButton> m_buttonPool;

        [Header("Configurations")]
        [SerializeField]
        private Vector2Int m_zoneResolutionSize;
        
        [Header("Listen On")]
        [SerializeField]
        private VoidEventChannelSO m_OnLevelReadyEvent;
        
        private List<RectTransform> m_zones = new List<RectTransform>();

        protected override void Awake()
        {
            base.Awake();
            InstantiateRuntimeZones();
        }
        
        private void InstantiateRuntimeZones()
        {
            foreach (var (zoneSo, serializedZone) in m_zoneToSerializedZone)
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
                levelButton.AssignedLevel = m_zoneToSerializedZone[zoneSo].Levels[i];
                levelButton.LevelSceneSo = zoneSo.Levels[i];
            }
        }
        
        
        
    }
}