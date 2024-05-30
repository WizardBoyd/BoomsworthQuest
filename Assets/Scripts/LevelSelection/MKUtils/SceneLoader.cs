using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mkey
{
    public class SceneLoader : MonoBehaviour
    {

        public Slider loadingBar;
        public GameObject LoadGroup;

        public Action LoadingCallBack;

        public static SceneLoader Instance;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); }
            else
            {
                Instance = this;
            }
        }

        public void LoadScene(int scene)
        {
            StartCoroutine(AsyncLoad(scene));
        }

        public void LoadScene(int scene, Action LoadingCallBack)
        {
            this.LoadingCallBack = LoadingCallBack;
            StartCoroutine(AsyncLoad(scene));
        }

        public void LoadScene(string sceneName)
        {
            int scene = SceneManager.GetSceneByName(sceneName).buildIndex;
            StartCoroutine(AsyncLoad(scene));
        }

        IEnumerator AsyncLoad(int scene)
        {
            if (LoadGroup)
            {
                LoadGroup.SetActive(true);
            }
            yield return new WaitForSeconds(0.2f);

            AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                if (loadingBar) loadingBar.value = ao.progress; //  float progress = Mathf.Clamp01(ao.progress / 0.9f);

                if (ao.progress >= 0.9f && !ao.allowSceneActivation)
                {
                    ao.allowSceneActivation = true;
                }
                yield return new WaitForSeconds(0.1f);
            }

            if (LoadGroup) LoadGroup.SetActive(false);
            if (LoadingCallBack != null) LoadingCallBack();
        }
    }
}