using UnityEngine.UI;
using UnityEngine;

/*
    22.04.2019
  - add click sound
 */
namespace Mkey
{
    public class ButtonClickSound : MonoBehaviour
    {
        [Tooltip("Set your clip, or default clip will be played.")]
        [SerializeField]
        private AudioClip clickSound;
        private Button b;

        void Start()
        {
            b = GetComponent<Button>();
            if (b)
            {
                b.onClick.RemoveListener(ClickSound);
                b.onClick.AddListener(ClickSound);
            }
        }

        public void ClickSound()
        {
            if (!clickSound) SoundMaster.Instance.SoundPlayClick(0, null);
            else {SoundMaster.Instance.PlayClip(0, clickSound); }
        }
    }
}
