using System;
using Audio.ScriptableObjects;
using Levels.ScriptableObjects;
using Misc.Singelton;
using UnityEngine;

namespace SaveSystem
{
    public class SaveSystem : MonoBehaviorSingleton<SaveSystem>
    {
        [Header("Scriptable Objects that hold the persistent Data")]
        [SerializeField] 
        private PersistentAudioDataSO _audioData;
      
        
        public PersistentAudioDataSO AudioData
        {
            get => _audioData;
        }


        protected override void Awake()
        {
            base.Awake();
            if(_audioData == null)
                Debug.LogError("Persistent Audio Data is Null");
          
        }

        private void Start()
        {
            
        }
        
      
    }
    
}