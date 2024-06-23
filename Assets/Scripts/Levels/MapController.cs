using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetManagment;
using AssetManagment.ConcreteReferences;
using Misc.Singelton;
using SaveSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Levels
{
    [RequireComponent(typeof(RectTransform))]
    public class MapController : MonoBehaviorSingleton<MapController>
    {
        private List<LevelZone> m_Zones;
        
        public int CurrentLevelCount { get; set; }

        protected override void Awake()
        {
            base.Awake();
            m_Zones = GetComponentsInChildren<LevelZone>().ToList();
            CurrentLevelCount = 0;
        }

        private void Start()
        {
            foreach (var zone in m_Zones)
            {
                zone.SpawnLevelButtons();
            }
        }


    }
}