using System;
using UnityEngine;

namespace Audio.AudioData
{
    [CreateAssetMenu(fileName = "NewAudioCue", menuName = "Audio/Audio Cue")]
    public class AudioCueSO : ScriptableObject
    {
        public bool looping = false;
        [SerializeField] 
        private AudioClipsGroup[] _audioClipsGroups = default;

        public AudioClip[] GetClips()
        {
            int numberOfClips = _audioClipsGroups.Length;
            AudioClip[] resultingClips = new AudioClip[numberOfClips];
            for (int i = 0; i < numberOfClips; i++)
            {
                resultingClips[i] = _audioClipsGroups[i].GetNextClip();
            }

            return resultingClips;
        }
    }

    /// <summary>
    /// Represents a group of AudioClips that can be treated as one, and provides automatic randomisation or sequencing based on the <c>SequenceMode</c> value.
    /// </summary>
    [Serializable]
    public class AudioClipsGroup
    {
        public SequenceMode SequenceMode = SequenceMode.RandomNoImmediateRepeat;
        public AudioClip[] AudioClips;

        private int _nextClipToPlay = -1;
        private int _lastClipPlayed = -1;

        /// <summary>
        /// Chooses the next clip in the sequence, either following the order or randomly
        /// </summary>
        /// <returns>A reference to an AudioClip</returns>
        public AudioClip GetNextClip()
        {
            //Fast out if there is only one clip to play
            if (AudioClips.Length == 1)
            {
                return AudioClips[0];
            }

            if (_nextClipToPlay == -1)
            {
                //index needs to be initialised: 0 if Sequential, random if otherwise
                _nextClipToPlay = (SequenceMode == SequenceMode.Sequential)
                    ? 0
                    : UnityEngine.Random.Range(0, AudioClips.Length);
            }
            else
            {
                //Select the next clip index based on the appropriate SequenceMode
                switch (SequenceMode)
                {
                    case SequenceMode.Random:
                        _nextClipToPlay = UnityEngine.Random.Range(0, AudioClips.Length);
                        break;
                    case SequenceMode.RandomNoImmediateRepeat:
                        do
                        {
                            _nextClipToPlay = UnityEngine.Random.Range(0, AudioClips.Length);
                        } while (_nextClipToPlay == _lastClipPlayed);
                        break;
                    case SequenceMode.Sequential:
                        _nextClipToPlay = (int)Mathf.Repeat(++_nextClipToPlay, AudioClips.Length);
                        break;
                }
            }

            _lastClipPlayed = _nextClipToPlay;
            return AudioClips[_nextClipToPlay];
        }
    }
    
    public enum SequenceMode
    {
        Random,
        RandomNoImmediateRepeat,
        Sequential
    }
    
}