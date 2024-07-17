using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetManagment;
using AssetManagment.ConcreteReferences;
using DependencyInjection.attributes;
using Events.ScriptableObjects;
using Factory;
using Levels.ScriptableObjects;
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
        private AsyncOperationHandle<IList<ZoneSO>> m_operationHandle;
        
        private List<ZoneSO> m_preloadedZones;
        
        private List<ZoneRuntime> m_runtimeZones;
        
        [Header("Pool Helper")]
        [SerializeField]
        private ComponentPoolSO<LevelButton> m_buttonPool;

        [Header("Configurations")]
        [SerializeField]
        private Vector2Int m_zoneResolutionSize;
        
        [Header("Listen On")]
        [SerializeField]
        private VoidEventChannelSO m_OnLevelReadyEvent;
        
        [Inject]
        private AutoSaveKeyValueStoreWrapper m_autoSaveKeyValueStoreWrapper;

        protected override void Awake()
        {
            base.Awake();
        }
        
        private IEnumerator Start()
        {
            if(m_autoSaveKeyValueStoreWrapper == null)
                throw new ArgumentNullException(nameof(m_autoSaveKeyValueStoreWrapper));
            m_operationHandle = Addressables.LoadAssetsAsync<ZoneSO>("Zone", null, true);
            if (!m_operationHandle.IsDone)
                yield return m_operationHandle;
            if (m_operationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                m_preloadedZones = new List<ZoneSO>(m_operationHandle.Result);
                InstantiateRuntimeZones();
            }
        }

        private void OnEnable()
        {
            //m_OnLevelReadyEvent.OnEventRaised += InstantiateRuntimeZones;
        }

        private void OnDisable()
        {
            //m_OnLevelReadyEvent.OnEventRaised -= InstantiateRuntimeZones;
        }

        private void OnDestroy()
        {
            if (m_operationHandle.IsValid())
            {
                Addressables.Release(m_operationHandle);
            }
        }

        private void InstantiateRuntimeZones()
        {
            m_runtimeZones = new List<ZoneRuntime>(m_preloadedZones.Count);
            foreach (ZoneSO preloadedZone in m_preloadedZones)
            {
                ZoneRuntime zoneRuntime = Instantiate(preloadedZone.ZonePrefab);
                if (zoneRuntime.TryGetComponent<RectTransform>(out RectTransform rectTransform))
                {
                    //Set the size of the zone
                    rectTransform.sizeDelta = m_zoneResolutionSize;
                }
                zoneRuntime.transform.SetParent(transform);
                SpawnLevelButtonsForZone(zoneRuntime, preloadedZone);
            }
        }
        
        private void SpawnLevelButtonsForZone(ZoneRuntime zoneRuntime, ZoneSO zoneSo)
        {
            SceneCurve sceneCurve;
            if(!zoneRuntime.TryGetComponent<SceneCurve>(out sceneCurve))
                return;
            List<LevelButton> buttons = new List<LevelButton>(m_buttonPool.Request(zoneSo.m_levelsInZone.Count));
            List<Vector3> positions = sceneCurve.Curve.GetPositions(buttons.Count);
            for(int i = 0 ; i < buttons.Count; i++)
            {
                buttons[i].AssignedLevel = zoneSo.m_levelsInZone[i];
                buttons[i].transform.localScale = zoneRuntime.transform.lossyScale;
                buttons[i].transform.SetParent(zoneRuntime.transform);
                buttons[i].transform.position = zoneRuntime.transform.TransformPoint(positions[i]);
            }
        }
        
        
        
    }
}