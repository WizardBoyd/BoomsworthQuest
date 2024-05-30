using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Mkey
{
    public class MapController : MonoBehaviour
    {
        private List<LevelButton> mapLevelButtons;
        public List<LevelButton> MapLevelButtons
        {
            get { return mapLevelButtons; }
            set { mapLevelButtons = value; }
        }
        public static MapController Instance;
        private LevelButton activeButton;
        public LevelButton ActiveButton
        {
            get { return activeButton; }
            set { activeButton = value; }
        }
        [HideInInspector]
        public Canvas parentCanvas;
        private MapMaker mapMaker;
        private ScrollRect sRect;
        private RectTransform content;
        private int biomesCount = 6;

        public static int currentLevel = 1; // set from this script by clicking on button. Use this variable to load appropriate level.
        public static int topPassedLevel = 30; // set from game MapController.topPassedLevel = 2; 

        [Header("If true, then the map will scroll to the Active Level Button", order = 1)]
        public bool scrollToActiveButton = true;

        private SoundMaster MSound => SoundMaster.Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            Debug.Log("Map controller started");
            if (mapMaker == null) mapMaker = GetComponent<MapMaker>();

            if (mapMaker == null)
            {
                Debug.LogError("No <MapMaker> component. Add <MapMaker.>");
                return;
            }

            if (mapMaker.biomes == null)
            {
                Debug.LogError("No Maps. Add Biomes to MapMaker.");
                return;
            }

            content = GetComponent<RectTransform>();
            if (!content)
            {
                Debug.LogError("No RectTransform component. Use RectTransform for MapMaker.");
                return;
            }

            List<Biome> bList = new List<Biome>(mapMaker.biomes);
            bList.RemoveAll((b) => { return b == null; });

            if (mapMaker.mapType == MapType.Vertical) bList.Reverse();
            MapLevelButtons = new List<LevelButton>();
            foreach (var b in bList)
            {
                MapLevelButtons.AddRange(b.levelButtons);
            }

            topPassedLevel = Mathf.Clamp(topPassedLevel, 0, MapLevelButtons.Count - 1);
            for (int i = 0; i < MapLevelButtons.Count; i++)
            {
                int scene = i + 1;
                MapLevelButtons[i].button.onClick.AddListener(() =>
                {
                    currentLevel = scene;
                    if (MSound) MSound.SoundPlayClick(0, null);
                    Debug.Log("load scene : " + scene);

                // uncomment for load game scene 
                /* 
               if(SceneLoader.Instance) SceneLoader.Instance.LoadScene(scene, () => { });
                 */
                // 

            });

                SetButtonActive(scene, (currentLevel == scene || scene == topPassedLevel + 1), (topPassedLevel >= scene));
                MapLevelButtons[i].numberText.text = (scene).ToString();
            }
            parentCanvas = GetComponentInParent<Canvas>();
            sRect = GetComponentInParent<ScrollRect>();
            if (scrollToActiveButton) StartCoroutine(SetMapPositionToAciveButton());
        }

 IEnumerator SetMapPositionToAciveButton()
        {
            yield return new WaitForSeconds(0.1f);
            if (sRect)
            {
                int bCount = mapMaker.biomes.Count;
                if (mapMaker.mapType == MapType.Vertical)
                {
                    float contentSizeY = content.sizeDelta.y / (bCount) * (bCount - 1.0f);
                    float relPos = content.InverseTransformPoint(ActiveButton.transform.position).y; // Debug.Log("contentY : " + contentSizeY +  " ;relpos : " + relPos + " : " + relPos / contentSizeY);
                    float vpos = (-contentSizeY / (bCount * 2.0f) + relPos) / contentSizeY; // 
                  //  sRect.verticalNormalizedPosition = Mathf.Clamp01(vpos); // Debug.Log("vpos : " + Mathf.Clamp01(vpos));

                    SimpleTween.Cancel(gameObject, false);
                    float start = sRect.verticalNormalizedPosition;

                    SimpleTween.Value(gameObject, start, vpos, 0.25f).SetOnUpdate((float f) => { sRect.verticalNormalizedPosition = Mathf.Clamp01(f); });

                }
                else
                {
                    float contentSizeX = content.sizeDelta.x / (bCount) * (bCount - 1.0f);
                    float relPos = content.InverseTransformPoint(ActiveButton.transform.position).x;
                    float hpos = (-contentSizeX / (bCount * 2.0f) + relPos) / contentSizeX; // 
                    //sRect.horizontalNormalizedPosition = Mathf.Clamp01(hpos);

                    SimpleTween.Cancel(gameObject, false);
                    float start = sRect.horizontalNormalizedPosition;

                    SimpleTween.Value(gameObject, start, hpos, 0.25f).SetOnUpdate((float f) => { sRect.horizontalNormalizedPosition = Mathf.Clamp01(f); });
                }
            }
            else
            {
                Debug.Log("no scrolling rect");
            }
        }

        private void SetButtonActive(int sceneNumber, bool active, bool isPassed)
        {
            string saveKey = sceneNumber.ToString() + "_stars_";
            int activeStarsCount = (PlayerPrefs.HasKey(saveKey)) ? PlayerPrefs.GetInt(saveKey) : 0;
            MapLevelButtons[sceneNumber - 1].SetActive(active, activeStarsCount, isPassed);
        }

        public void SetControlActivity(bool activity)
        {
            for (int i = 0; i < MapLevelButtons.Count; i++)
            {
                if (!activity) MapLevelButtons[i].button.interactable = activity;
                else
                {
                    MapLevelButtons[i].button.interactable = MapLevelButtons[i].Interactable;
                }
            }
        }

        void Update_rem()
        {
            Debug.Log(content.sizeDelta.y + " : " + content.InverseTransformPoint(ActiveButton.transform.position).y);
            Debug.Log("sRect.verticalNormalizedPosition: " + sRect.verticalNormalizedPosition);
            Debug.Log("sRect.verticalNormalizedPosition: " + sRect.horizontalNormalizedPosition);
        }
    }
}