using System;
using System.Collections.Generic;
using Factory;
using Levels;
using Levels.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public enum MapType{Vertical, Horizontal}
    public class MapMaker : MonoBehaviour
    {
        public MapType mapType = MapType.Horizontal;

        [SerializeField] 
        public List<LevelZone> LevelPrefabs;
        
        [HideInInspector]
        public List<LevelZone> zones;

        private List<LevelButtonView> levelButtons;

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.hierarchyChanged += On_HierarchyChange;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.hierarchyChanged -= On_HierarchyChange;
#endif
        }

#if UNITY_EDITOR
        private void On_HierarchyChange()
        {
            List<LevelZone> zoneList = new List<LevelZone>(GetComponentsInChildren<LevelZone>());
            zoneList.Sort((b, c) =>
            {
                int bi = b.transform.GetSiblingIndex();
                int ci = c.transform.GetSiblingIndex();
                if (ci > bi) return -1;
                if (ci < bi) return 1;
                return 0;
            });
            zones = zoneList;
        }

        public void AddComponents(bool addLayoutGroup, bool addContentSizeFitter, MapType mapType)
        {
          
            VerticalLayoutGroup[] vLGs = GetComponents<VerticalLayoutGroup>();
            HorizontalLayoutGroup[] hLGs = GetComponents<HorizontalLayoutGroup>();
            ContentSizeFitter[] cSFs = GetComponents<ContentSizeFitter>();

            VerticalLayoutGroup vLG = null;
            HorizontalLayoutGroup hLG = null;

            if (mapType == MapType.Vertical)
            {
                //destroy HorizontalLayoutGroup
                if (hLGs != null && hLGs.Length > 0)
                {
                    // Debug.Log("destroy hlgs");
                    for (int i = 0; i < hLGs.Length; i++)
                    {
                        DestroyImmediate(hLGs[i]);
                    }
                }

                if (vLGs != null && vLGs.Length > 0)
                {
                    for (int i = (addLayoutGroup) ? 1 : 0; i < vLGs.Length; i++)
                    {
                        DestroyImmediate(vLGs[i]);
                    }
                    vLG = vLGs[0];
                }
                else if (vLGs == null || vLGs.Length == 0)
                {
                    if (addLayoutGroup) vLG = gameObject.AddComponent<VerticalLayoutGroup>();
                }

            }
            else
            {
                if (vLGs != null && vLGs.Length > 0)
                {
                    // Debug.Log("destroy vlgs");
                    for (int i = 0; i < vLGs.Length; i++)
                    {
                        DestroyImmediate(vLGs[i]);
                    }
                }

                if (hLGs != null && hLGs.Length > 0)
                {
                    for (int i = (addLayoutGroup) ? 1 : 0; i < hLGs.Length; i++)
                    {
                        DestroyImmediate(hLGs[i]);
                    }
                    hLG = hLGs[0];
                }
                else if (hLGs == null || hLGs.Length == 0)
                {
                    if (addLayoutGroup) hLG = gameObject.AddComponent<HorizontalLayoutGroup>();
                }
            }
            
            ContentSizeFitter cSF = null;
            if (cSFs != null && cSFs.Length > 0)
            {
                for (int i = (addContentSizeFitter) ? 1 : 0; i < cSFs.Length; i++)
                {
                    DestroyImmediate(cSFs[i]);
                }
                cSF = cSFs[0];

            }
            else if (cSFs == null || cSFs.Length == 0)
            {
                if (addContentSizeFitter)
                {
                    cSF = gameObject.AddComponent<ContentSizeFitter>();
                }
            }
            
            if (addContentSizeFitter && cSF != null)
            {
                cSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                cSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                UnityEditor.EditorUtility.SetDirty(cSF);
            }

            if (addLayoutGroup && vLG != null)
            {
                vLG.childForceExpandHeight = false;
                vLG.childForceExpandWidth = false;
                RectTransform rT = GetComponent<RectTransform>();
                rT.anchorMin = new Vector2(0.5f, 0);
                rT.anchorMax = new Vector2(0.5f, 0);
                rT.pivot = new Vector2(0.5f, 0);
                rT.anchoredPosition = new Vector2(0, 0);
                UnityEditor.EditorUtility.SetDirty(vLG);
            }

            if (addLayoutGroup && hLG != null)
            {
                hLG.childForceExpandHeight = false;
                hLG.childForceExpandWidth = false;
                RectTransform rT = GetComponent<RectTransform>();
                rT.anchorMin = new Vector2(0, 0.5f);
                rT.anchorMax = new Vector2(0.0f, 0.5f);
                rT.pivot = new Vector2(0, 0.5f);
                rT.anchoredPosition = new Vector2(0, 0);
                UnityEditor.EditorUtility.SetDirty(hLG);
            }
        }
#endif

        public bool HasEmptyPrefabs()
        {
            if (LevelPrefabs == null || LevelPrefabs.Count == 0) return true;
            foreach (var bp in LevelPrefabs)
            {
                if (bp == null) return true;
            }
            return false;
        }

        public void Build()
        {
            if(zones == null || zones.Count == 0
                             || LevelPrefabs == null || LevelPrefabs.Count == 0)
                return;

            LevelZone[] prefs = new LevelZone[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                for (int pi = 0; pi < zones.Count; pi++)
                {
                    if (zones[i].name.Contains(LevelPrefabs[i].name))
                    {
                        prefs[i] = LevelPrefabs[pi];
                        break;
                    }
                }
            }
            
            //Delete all the existing level zones
            Clean();
            
            foreach (LevelZone zonePrefab in zones)
            {
                AddZone(zonePrefab);
            }
            
            ReArrangeZones();
            
        }

        //Remove All zones from List and Destroy
        public void Clean()
        {
            if(zones == null || zones.Count == 0)
                return;
            for (int i = zones.Count - 1; i >= 0; i--)
            {
                if (i < zones.Count)
                {
                    LevelZone zone = zones[i];
                    if (zone)
                    {
                        zones.Remove(zone);
                        DestroyImmediate(zone.gameObject);
                    }
                }
            }
        }
        
        public void AddZone(LevelZone zonePrefab)
        {
            if (!zonePrefab)
            {
                Debug.LogError("No Prefab");
                return;
            }

            if (zones == null)
                zones = new List<LevelZone>();
            zones.Add(CreateZone(zonePrefab));
        }

        public void AddZoneAtTop(LevelZone prefab)
        {
            if (!prefab)
            {
                Debug.Log("No prefab");
                return;
            }
            if (zones == null) zones = new List<LevelZone>();
            zones.Insert(0, CreateZone(prefab));
        }

        private LevelZone CreateZone(LevelZone zonePrefab)
        {
            if (!zonePrefab)
            {
                Debug.LogError("No Prefab");
                return null;
            }

            LevelZone zone = Instantiate(zonePrefab);
            zone.transform.localScale = transform.lossyScale;
            zone.transform.SetParent(transform);
            zone.transform.localPosition = Vector3.zero;
            Image zoneImage = zone.GetComponent<Image>();
            if(zoneImage)
                zoneImage.SetNativeSize();
            return zone;
        }

        public void RemoveZone(int index)
        {
            if (zones != null && index < zones.Count && index >= 0)
            {
                LevelZone zone = zones[index];
                if (zone)
                {
                    zones.Remove(zone);
                    Destroy(zone.gameObject);
                }
            }
        }

        public void RemoveZone(LevelZone zone, bool destroy)
        {
            if (zones != null && zones.Contains(zone))
            {
                zones.Remove(zone);
            }
            if(destroy)
                DestroyImmediate(zone.gameObject);
        }

        public void ChangeBiome(int index, LevelZone zonePrefab)
        {
            LevelZone zone = zones[index];

            LevelZone NewZone = CreateZone(zonePrefab);

            if (NewZone)
            {
                zones.Remove(zone);
                Destroy(zone.gameObject);
                zones.Insert(index, NewZone);
            }
        }

        public void ReArrangeZones()
        {
            if(zones == null || zones.Count == 0)
                return;
            
            VerticalLayoutGroup vLG = GetComponent<VerticalLayoutGroup>();
            HorizontalLayoutGroup hLG = GetComponent<HorizontalLayoutGroup>();
            if ((vLG && vLG.enabled) || (hLG && hLG.enabled))
            {
                for (int i = 0; i < zones.Count; i++)
                {
                    LevelZone b = zones[i];
                    b.transform.SetSiblingIndex(i);
                }
                return;
            }
            
            // if no layouts set transform positions manually  and set new hierarchy order
            zones[0].transform.localPosition = new Vector3(0, 0, 0);
            zones[0].transform.SetSiblingIndex(0);
            if (zones.Count > 1)
            {
                for (int i = 1; i < zones.Count; i++)
                {
                    LevelZone b = zones[i];
                    LevelZone bPrev = zones[i - 1];
                    switch (mapType)
                    {
                        case MapType.Vertical:
                            float dy = i * bPrev.ZoneSize.y / 2 + i * b.ZoneSize.y / 2;
                            b.transform.localPosition = new Vector3(0, dy, 0);
                            break;
                        case MapType.Horizontal:
                            float dx = i * bPrev.ZoneSize.x / 2 + i * b.ZoneSize.x / 2;
                            b.transform.localPosition = new Vector3(dx, 0, 0);
                            break;
                    }
                    b.transform.SetSiblingIndex(i);
                }
            }
        }

        public void CleanLostZones()
        {
            if(zones == null || zones.Count == 0)
                return;
            for (int i = 0; i < zones.Count; i++)
            {
                if (i >= zones.Count) break;
                if (!zones[i])
                {
                    zones.Remove(zones[i]);
                }
            }
        }

        public void CleanExcessZones()
        {
            LevelZone[] b = GetComponentsInChildren<LevelZone>();
            bool dflag = false;
            for (int i = 0; i < b.Length; i++)
            {
                LevelZone bi = b[i];
                if (zones == null || zones.IndexOf(bi) == -1)
                {
                    Debug.Log("Remove: " + bi.name);
                    DestroyImmediate(bi.gameObject);
                    dflag = true;
                }
            }
            if (dflag)
            {
                GC.Collect();
            }
        }
    }
}