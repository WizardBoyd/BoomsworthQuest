using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagment;
using AssetManagment.ConcreteReferences;
using Misc.Singelton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Levels
{
    [RequireComponent(typeof(RectTransform))]
    public class MapController : MonoBehaviorSingleton<MapController>
    {
        [SerializeField] 
        private List<AssetReferenceGameObject> LevelStack;

        [SerializeField] 
        private MapType MapType = MapType.Horizontal;

        private ContentSizeFitter _contentSizeFitter;
        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        private List<LevelZone> _levelZones;

        
        
        private IEnumerator Start()
        {
            if (!PreBuildCheck())
                yield return null;
            var Zonehandle = AssetManager.Instance.LoadAssetsByLabelAsync("Zone");
            var LevelDescriptionHandle = AssetManager.Instance.LoadAssetsByLabelAsync("LevelSelection");
            yield return Task.WhenAll(Zonehandle.Task, LevelDescriptionHandle.Task);
            AddLayoutComponents(true,true);
            
            foreach (AssetReferenceGameObject ZoneReference in LevelStack)
            {
               AddZone(ZoneReference);
            }
        }

        private void AddLayoutComponents(bool addLayoutGroup, bool addContentSizeFitter)
        {
            if (MapType == MapType.Horizontal && addLayoutGroup)
            {
                _layoutGroup = gameObject.GetComponent<HorizontalLayoutGroup>();
                if (_layoutGroup == null)
                    _layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else if (MapType == MapType.Vertical && addLayoutGroup)
            {
                _layoutGroup = gameObject.GetComponent<VerticalLayoutGroup>();
                if (_layoutGroup == null)
                    _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            }

            if (addContentSizeFitter && _contentSizeFitter == null)
                _contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();

            if (addContentSizeFitter && _contentSizeFitter != null)
            {
                _contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            if (addLayoutGroup)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();
                switch (MapType)
                {
                    case MapType.Vertical:
                        _layoutGroup.childForceExpandHeight = false;
                        _layoutGroup.childForceExpandWidth = false;
                        rectTransform.anchorMin = new Vector2( 0.5f,0);
                        rectTransform.anchorMax = new Vector2(0.5f, 0);
                        rectTransform.pivot = new Vector2(0, 0);
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                        break;
                    case MapType.Horizontal:
                        _layoutGroup.childForceExpandHeight = false;
                        _layoutGroup.childForceExpandWidth = false;
                        rectTransform.anchorMin = new Vector2( 0,0.5f);
                        rectTransform.anchorMax = new Vector2(0, 0.5f);
                        rectTransform.pivot = new Vector2(0, 0.5f);
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool PreBuildCheck()
        {
            if (LevelStack == null || LevelStack.Count <= 0)
            {
                Debug.LogError("Level Stack is null or empty");
                return false;
            }

            return true;
        }

        private void AddZone(AssetReferenceGameObject zoneReference)
        {
            
            if (_levelZones == null)
                _levelZones = new List<LevelZone>();
            if (AssetManager.Instance.TryGetComponentSync(zoneReference, out LevelZone OutZone))
            {
                _levelZones.Add(CreateZone(OutZone));
            }
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
    }
}