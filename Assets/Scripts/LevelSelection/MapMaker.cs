using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Mkey
{
    public enum MapType { Vertical, Horizontal }
    [DisallowMultipleComponent]
    public class MapMaker : MonoBehaviour
    {
        public List<GameObject> BackgroundPrefabs;
        public MapType mapType;
        public List<Biome> biomes;

        [HideInInspector]
        [SerializeField]
        public MapType oldMapType;

        private List<LevelButton> levelButtons;

        public bool HasEmptyPrefabs()
        {
            if (BackgroundPrefabs == null || BackgroundPrefabs.Count == 0) return true;
            foreach (var bp in BackgroundPrefabs)
            {
                if (bp == null) return true;
            }
            return false;
        }

        public void AddBiome(GameObject prefab)
        {
            if (!prefab)
            {
                Debug.Log("No prefab");
                return;
            }
            if (biomes == null) biomes = new List<Biome>();
            biomes.Add(CreateBiome(prefab));
        }

        public void AddBiomeAtTop(GameObject prefab)
        {
            if (!prefab)
            {
                Debug.Log("No prefab");
                return;
            }
            if (biomes == null) biomes = new List<Biome>();
            biomes.Insert(0, CreateBiome(prefab));
        }

        private Biome CreateBiome(GameObject prefab)
        {
            if (!prefab)
            {
                Debug.Log("No prefab");
                return null;
            }
            GameObject b = Instantiate(prefab);
            b.transform.localScale = transform.lossyScale;
            b.transform.SetParent(transform);
            b.transform.localPosition = Vector3.zero;
            Image i = b.GetComponent<Image>();
            if (i) i.SetNativeSize();
            return b.GetComponent<Biome>();
        }

        /// <summary>
        /// Remove from hierarchy biomes without references in biomes list 
        /// </summary>
        public void CleanExcessBiomes()
        {
            Biome[] b = GetComponentsInChildren<Biome>();
            bool dflag = false;
            for (int i = 0; i < b.Length; i++)
            {
                Biome bi = b[i];
                if (biomes == null || biomes.IndexOf(bi) == -1)
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

        /// <summary>
        /// Remove empty biomes from list
        /// </summary>
        public void CleanLostBiomes()
        {
            if (biomes == null || biomes.Count == 0) return;
            int length = biomes.Count;
            for (int i = 0; i < length; i++)
            {
                if (i >= biomes.Count) break;
                if (!biomes[i])
                {
                    biomes.Remove(biomes[i]);
                }
            }
        }

        /// <summary>
        /// Remove Biome from List and Destroy
        /// </summary>
        /// <param name="index"></param>
        public void RemoveBiome(int index)
        {
            if (biomes != null && index < biomes.Count && index >= 0)
            {
                Biome b = biomes[index];
                if (b)
                {
                    biomes.Remove(b);
                    DestroyImmediate(b.gameObject);
                }
            }
        }

        /// <summary>
        /// Remove Biome from List
        /// </summary>
        /// <param name="index"></param>
        public void RemoveBiome(Biome biome, bool destroy)
        {
            if (biomes != null && biomes.Contains(biome))
            {
                biomes.Remove(biome);
            }
            if (destroy) DestroyImmediate(biome.gameObject);
        }

        /// <summary>
        /// Remove All Biomes from List and Destroy
        /// </summary>
        /// <param name="index"></param>
        public void Clean()
        {
            if (biomes == null || biomes.Count == 0) return;
            int length = biomes.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                if (i < biomes.Count)
                {
                    Biome b = biomes[i];
                    if (b)
                    {
                        biomes.Remove(b);
                        DestroyImmediate(b.gameObject);
                    }
                }
            }

        }

        /// <summary>
        /// Rebuld the same Map
        /// </summary>
        /// <param name="index"></param>
        public void ReBuild()
        {
            int length = biomes.Count;

            if (biomes == null || length == 0) return;
            int lengthp = BackgroundPrefabs.Count;
            if (BackgroundPrefabs == null || lengthp == 0) return;

            //save  biomes prefabs
            GameObject[] bPrefs = new GameObject[length];
            for (int i = 0; i < length; i++)
            {
                for (int pi = 0; pi < length; pi++)
                {

                    if (biomes[i].name.Contains(BackgroundPrefabs[pi].name))
                    {
                        bPrefs[i] = BackgroundPrefabs[pi];
                        break;
                    }
                }
            }

            Clean();

            foreach (var item in bPrefs)
            {
                AddBiome(item);
            }

            ReArrangeBiomes();
        }

        /// <summary>
        /// Rearrange Biomes positions and hierarchy order
        /// </summary>
        public void ReArrangeBiomes()
        {
            if (biomes == null || biomes.Count == 0) return;
            int length = biomes.Count;

            // check layouts and set new hierarchy order
            VerticalLayoutGroup vLG = GetComponent<VerticalLayoutGroup>();
            HorizontalLayoutGroup hLG = GetComponent<HorizontalLayoutGroup>();
            if ((vLG && vLG.enabled) || (hLG && hLG.enabled))
            {
                for (int i = 0; i < length; i++)
                {
                    Biome b = biomes[i];
                    b.transform.SetSiblingIndex(i);
                }
                return;
            }

            // if no layouts set transform positions manually  and set new hierarchy order
            biomes[0].transform.localPosition = new Vector3(0, 0, 0);
            biomes[0].transform.SetSiblingIndex(0);
            if (length > 1)
            {
                for (int i = 1; i < length; i++)
                {
                    Biome b = biomes[i];
                    Biome bPrev = biomes[i - 1];
                    switch (mapType)
                    {
                        case MapType.Vertical:
                            float dy = i * bPrev.BiomeSize.y / 2 + i * b.BiomeSize.y / 2;
                            b.transform.localPosition = new Vector3(0, dy, 0);
                            break;
                        case MapType.Horizontal:
                            float dx = i * bPrev.BiomeSize.x / 2 + i * b.BiomeSize.x / 2;
                            b.transform.localPosition = new Vector3(dx, 0, 0);
                            break;
                    }
                    b.transform.SetSiblingIndex(i);
                }
            }
        }

        /// <summary>
        /// Change biome type at index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bType"></param>
        public void ChangeBiome(int index, GameObject prefab)
        {
            Biome b = biomes[index];

            Biome newBiome = CreateBiome(prefab);
            if (newBiome)
            {
                biomes.Remove(b);
                DestroyImmediate(b.gameObject);
                biomes.Insert(index, newBiome);
            }
        }

        public void OnManuallyHierChanged()
        {
            List<Biome> bList = new List<Biome>(GetComponentsInChildren<Biome>());
            bList.Sort((b, c) =>
            {
                int bi = b.transform.GetSiblingIndex();
                int ci = c.transform.GetSiblingIndex();
                if (ci > bi) return -1;
                if (ci < bi) return 1;
                return 0;
            });
            biomes = bList;//  Debug.Log("blist count: " + bList.Count);
        }

        public void AddComponents(bool addLayoutGroup, bool addContSizeFitter, MapType mType)
        {
            VerticalLayoutGroup[] vLGs = GetComponents<VerticalLayoutGroup>();
            HorizontalLayoutGroup[] hLGs = GetComponents<HorizontalLayoutGroup>();
            ContentSizeFitter[] cSFs = GetComponents<ContentSizeFitter>();

            VerticalLayoutGroup vLG = null;
            HorizontalLayoutGroup hLG = null;

            if (mType == MapType.Vertical)
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



            // Remove or add ContentSizeFitter
            ContentSizeFitter cSF = null;
            if (cSFs != null && cSFs.Length > 0)
            {
                for (int i = (addContSizeFitter) ? 1 : 0; i < cSFs.Length; i++)
                {
                    DestroyImmediate(cSFs[i]);
                }
                cSF = cSFs[0];

            }
            else if (cSFs == null || cSFs.Length == 0)
            {
                if (addContSizeFitter)
                {
                    cSF = gameObject.AddComponent<ContentSizeFitter>();
                }
            }
#if UNITY_EDITOR
            if (addContSizeFitter && cSF != null)
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
#endif
        }
    }
}
/*
 *     public void OnManuallyHierChanged_old()
    {
        Debug.Log(name + " OnManuallyHierChanged()");



        if (biomes.Count < 2) return;
        List<Biome> bList = new List<Biome>();

        for (int i = 0; i < biomes.Count; i++)
        {
            Biome b = biomes[i];
            int si = b.transform.GetSiblingIndex();
            bList.Add(b);
        }
        //  bList.ForEach((b) => { Debug.Log(b.transform.GetSiblingIndex()); });
        bList.Sort((b, c) => {
            int bi = b.transform.GetSiblingIndex();
            int ci = c.transform.GetSiblingIndex();
            if (ci > bi) return -1;
            if (ci < bi) return 1;
            return 0;
        });
        int it = 0;
        bList.ForEach((b) => { biomes[it] = b; it++; });

    }
 */
