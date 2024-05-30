using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
/*
   021218 
   add stopallclip

    160119
    set clips as private
    add StopAllClip(bool stopBackgroundMusik)

    240119
    add   public void SetNewBackGroundClipAndPlay(AudioClip newBackGroundClip)
    fix  void PlayBkgMusik(bool play)
            // set clip if failed
            if (aSbkg && !aSbkg.clip && bkgMusic) aSbkg.clip = bkgMusic;

    020619
        add SetSound (bool on)
        add SetMusic (bool on)
        add SetFeatMusic (bool on)
        add SetVolume(float volume)

    100219
        replace OLD
              if (aSclick && aC)
                {
                    aSclick.clip = aC;
                    aSclick.Play();
                }
                while (aSclick.isPlaying)
                    yield return wff;
       new
        if (aSclick && aC)
                {
                    aSclick.clip = aC;
                    aSclick.Play();
                    while (aSclick.isPlaying)
                        yield return wff;
                }
           remove
            GetComponet<AudioSource>

    21.02.19
        base class for game soun masters
    25.06.2019
     change  
      public void StopAllClip(bool stopMusic)
        {
            if (musicSource && stopMusic) StopMusic();

            AudioSource[] aSs = GetComponentsInChildren<AudioSource>();
            if (aSs != null )
            {
                foreach (var item in aSs)
                {
                    if (item && (item != musicSource)) item.Stop();
                }
            }
        }
        
        private void ApplyVolume()
        {
            if (musicSource)
            {
                musicSource.volume = Volume * musicVolumeMult;
            }

            AudioSource[] aSs = GetComponentsInChildren<AudioSource>();
            if (aSs != null)
            {
                foreach (var item in aSs)
                {
                    if (item && (item != musicSource)) item.volume = Volume;
                }
            }
        }

        27.06.2019  public void PlayCurrentMusic()
        28.06.2019   - make accessible       
        [SerializeField]
        private int audioSoucesMaxCount = 10;
        
        31.03.2020
            - public Action <float> ChangeVolumeEvent;
        
        16.07.2020 
            -fix editor play mode

             
*/
namespace Mkey
{
    public class SoundMaster : MonoBehaviour
    {
        #region basic clips
        [Space(8, order = 0)]
        [Header("Basic audio clips", order = 1)]
        [SerializeField]
        private AudioClip music;
        [SerializeField]
        private AudioClip buttonClick;
        [SerializeField]
        private AudioClip openWindow;
        [SerializeField]
        private AudioClip closeWindow;
        #endregion basic clips
        [SerializeField]
        private uint audioSoucesMaxCount = 10;
        #region save keys
        [SerializeField]
        private bool saveSettings = true;
        private string saveNameSound = "mk_soundon";
        private string saveNameMusic = "mk_musicon";
        private string saveNameVolume = "mk_volume";
        #endregion save keys

        #region private
        private AudioSource musicSource; // for background musik
        private AudioClip currentMusic;
        private WaitForEndOfFrame wff; // new WaitForEndOfFrame()
        private WaitForSeconds wfs0_1; // new WaitForSeconds(0.1f);
        private List<AudioSource> tempAudioSources; // not loop
        private int musicVolumeTween = -1;
        #endregion private

        #region properties
        public static SoundMaster Instance { get; private set; }

        public bool SoundOn
        {
            get; private set;
        }

        public bool MusicOn
        {
            get; private set;
        }

        public float Volume
        {
            get; private set;
        }
        #endregion properties

        #region events
        public Action<float> ChangeVolumeEvent;
        #endregion events

        [SerializeField]
        private float musicVolumeMult = 0.1f;

        #region regular
        protected virtual void Awake()
        {
            Debug.Log("sound base awake");
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            wff = new WaitForEndOfFrame();
            wfs0_1 = new WaitForSeconds(0.1f);
        }

        protected virtual void Start()
        {
            SoundOn = true;
            MusicOn = true;
            Volume = 1;
            if (saveSettings)
            {
                if (!PlayerPrefs.HasKey(saveNameSound))
                {
                    PlayerPrefs.SetInt(saveNameSound, (SoundOn) ? 1 : 0);
                }
                SoundOn = (PlayerPrefs.GetInt(saveNameSound) > 0) ? true : false;

                if (!PlayerPrefs.HasKey(saveNameMusic))
                {
                    PlayerPrefs.SetInt(saveNameMusic, (MusicOn) ? 1 : 0);
                }
                MusicOn = (PlayerPrefs.GetInt(saveNameMusic) > 0) ? true : false;

                if (!PlayerPrefs.HasKey(saveNameVolume))
                {
                    PlayerPrefs.SetFloat(saveNameVolume, 1.0f);
                }
                Volume = PlayerPrefs.GetFloat(saveNameVolume);
                ChangeVolumeEvent?.Invoke(Volume);
            }

            musicSource = CreateAudioSourceAtPos(transform.position);
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.name = "music";

            tempAudioSources = new List<AudioSource>();
            currentMusic = music;
            ApplyVolume();
            PlayCurrentMusic();
        }

        protected virtual void OnDestroy()
        {
            SimpleTween.Cancel(musicVolumeTween, false);
        }

        protected virtual void OnValidate()
        {
            musicVolumeMult = Mathf.Clamp01(musicVolumeMult);
        }
        #endregion regular

        #region sound settings countrol
        public virtual void SetSound(bool on)
        {
            SoundOn = on;
            if (saveSettings) PlayerPrefs.SetInt(saveNameSound, (SoundOn) ? 1 : 0);
        }

        public virtual void SetMusic(bool on)
        {
            MusicOn = on;
            if (saveSettings) PlayerPrefs.SetInt(saveNameMusic, (MusicOn) ? 1 : 0);
            PlayCurrentMusic();
        }

        public void SetVolume(float volume)
        {
            Volume = Mathf.Clamp(volume, 0, 1);
            if (saveSettings) PlayerPrefs.SetFloat(saveNameVolume, Volume);
            ApplyVolume();
            ChangeVolumeEvent?.Invoke(Volume);
        }
        #endregion sound settings countrol

        #region play basic clips
        public void SoundPlayClipAtPos(float playDelay, AudioClip aC, Vector3 pos, float volumeMultiplier, Action callBack)
        {
            StartCoroutine(PlayClipAtPoint(playDelay, aC, pos, volumeMultiplier, callBack));
        }

        public void SoundPlayClick(float playDelay, Action callBack)
        {
            PlayClip(playDelay, buttonClick, callBack);
        }

        public void SoundPlayOpenWindow(float playDelay, Action callBack)
        {
            PlayClip(playDelay, openWindow, callBack);
        }

        public void SoundPlayCloseWindow(float playDelay, Action callBack)
        {
            PlayClip(playDelay, closeWindow, callBack);
        }
        #endregion play basic clips

        #region play clips
        /// <summary>
        /// Play clip at audiosource point
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="clip"></param>
        /// <param name="completeCallBack"></param>
        public void PlayClip(float delay, AudioClip clip)
        {
            StartCoroutine(PlayClipAtPoint(delay, clip, transform.position, 1, null));
        }

        /// <summary>
        /// Play clip at audiosource point
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="clip"></param>
        /// <param name="completeCallBack"></param>
        public void PlayClip(float delay, AudioClip clip, Action completeCallBack)
        {
            StartCoroutine(PlayClipAtPoint(delay, clip, transform.position, 1, completeCallBack));
        }

        /// <summary>
        /// Play clip at position
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="clip"></param>
        /// <param name="completeCallBack"></param>
        public void PlayClip(float delay, AudioClip clip, Vector3 position, Action completeCallBack)
        {
            StartCoroutine(PlayClipAtPoint(delay, clip, position, 1, completeCallBack));
        }

        protected IEnumerator PlayClipAtPoint(float playDelay, AudioClip aC, Vector3 pos, float volumeMultiplier, Action completeCallBack)
        {
            if (SoundOn && tempAudioSources.Count < audioSoucesMaxCount)
            {
                AudioSource aSt = CreateAudioSourceAtPos(pos);
                tempAudioSources.Add(aSt);
                aSt.volume = Volume * volumeMultiplier;

                float delay = 0f;
                while (delay < playDelay)
                {
                    delay += Time.deltaTime;
                    yield return wff;
                }

                if (aC)
                {
                    aSt.clip = aC;
                    aSt.Play();
                }

                while (aSt && aSt.isPlaying)
                    yield return wff;

                tempAudioSources.Remove(aSt);
                if (aSt) Destroy(aSt.gameObject);
                completeCallBack?.Invoke();
            }
        }
        
        /// <summary>
        /// Set new music and play
        /// </summary>
        /// <param name="newMusic"></param>
        public void SetMusicAndPlay(AudioClip newMusic)
        {
            if (!newMusic) return;
            currentMusic = newMusic;
            PlayCurrentMusic();
        }

        /// <summary>
        /// Play current music clip
        /// </summary>
        public void PlayCurrentMusic()
        {
            if (!musicSource || !currentMusic) return;
            SimpleTween.Cancel(musicVolumeTween, true);
            float volume = Volume * musicVolumeMult;
            if (MusicOn)
            {
                if ((currentMusic == musicSource.clip) && musicSource.isPlaying) // check volume
                {
                    float vol = musicSource.volume;
                    if (vol != volume) musicVolumeTween = SimpleTween.Value(gameObject, vol, volume, 1f).SetOnUpdate((float val) => { musicSource.volume = val; }).
                         AddCompleteCallBack(() => { musicSource.volume = volume; }).ID;
                }

                if ((currentMusic != musicSource.clip) && musicSource.isPlaying)
                {
                    musicSource.Stop();
                    musicSource.clip = currentMusic;
                    musicSource.Play();
                    float vol = musicSource.volume;
                    if (vol != volume) musicVolumeTween = SimpleTween.Value(gameObject, vol, volume, 1f).SetOnUpdate((float val) => { musicSource.volume = val; }).
                         AddCompleteCallBack(() => { musicSource.volume = volume; }).ID;
                }

                if (!musicSource.isPlaying)
                {
                    musicSource.clip = currentMusic;
                    musicSource.volume = 0;
                    musicSource.Play();
                    musicVolumeTween = SimpleTween.Value(gameObject, 0.0f, volume, 1f).SetOnUpdate((float val) => { musicSource.volume = val; }).
                           AddCompleteCallBack(() => { musicSource.volume = volume; }).ID;
                }
            }
            else
            {
                StopMusic();
            }
        }
        #endregion play clips

        #region stop clips
        /// <summary>
        /// Stop all clips with or without backround music
        /// </summary>
        /// <param name="stopMusic"></param>
        public void StopAllClip(bool stopMusic)
        {
            if (musicSource && stopMusic) StopMusic();

            AudioSource[] aSs = GetComponentsInChildren<AudioSource>();
            if (aSs != null )
            {
                foreach (var item in aSs)
                {
                    if (item && (item != musicSource)) item.Stop();
                }
            }
        }

        /// <summary>
        /// Stop music audiosource
        /// </summary>
        public void StopMusic()
        {
            SimpleTween.Cancel(musicVolumeTween, true);
            if (musicSource && musicSource.isPlaying)
            {
                musicVolumeTween = SimpleTween.Value(gameObject, Volume* musicVolumeMult, 0.0f, 1f).SetOnUpdate((float val) => { musicSource.volume = val; }).
                      AddCompleteCallBack(() => { musicSource.Stop(); musicSource.volume = 0; }).ID;
            }
        }
        #endregion stop clips

        #region private
        private void ApplyVolume()
        {
            if (musicSource)
            {
                musicSource.volume = Volume * musicVolumeMult;
            }

            AudioSource[] aSs = GetComponentsInChildren<AudioSource>();
            if (aSs != null)
            {
                foreach (var item in aSs)
                {
                    if (item && (item != musicSource)) item.volume = Volume;
                }
            }
        }

        private AudioSource CreateAudioSourceAtPos(Vector3 pos)
        {
            GameObject aS = new GameObject();
            aS.transform.position = pos;
            aS.transform.parent = transform;
            return aS.AddComponent<AudioSource>();
        }
        #endregion private
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SoundMaster))]
    public class SoundMasterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
                SoundMaster script = (SoundMaster)target;
                if (script)
                {
                    GUILayout.Label("Testing, music - " + script.MusicOn + ", sound - "+ script.SoundOn);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Play click"))
                    {
                        script.SoundPlayClick(0,null);
                    }
                    if (GUILayout.Button(!script.MusicOn?  "MusicOn" : "MusicOff"))
                    {
                        script.SetMusic(!script.MusicOn);
                    }
                    if (GUILayout.Button(!script.SoundOn ? "SoundOn" : "SoundOff"))
                    {
                        script.SetSound(!script.SoundOn);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("Goto play mode for test sounds");
            }
        }
    }
#endif
}