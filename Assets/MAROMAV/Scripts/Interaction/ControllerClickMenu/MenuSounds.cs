
using UnityEngine;

namespace MAROMAV.CoworkSpace
{
    public class MenuSounds : MonoBehaviour
    {

        // private GvrAudioSource audioSelect;
        // private GvrAudioSource audioHover;
        // private GvrAudioSource audioBack;
        public AudioClip audioSelect;
        public AudioClip audioHover;
        public AudioClip audioBack;
        public ClickMenuRoot menuRoot;

        private AudioSource audioSource;

        void Awake()
        {
            menuRoot.OnItemSelected += OnItemSelected;
            menuRoot.OnItemHovered += OnItemHovered;
            menuRoot.OnMenuOpened += OnMenuOpened;

            audioSource = GetComponent<AudioSource>();
        }

        void OnDestroy()
        {
            if (menuRoot)
            {
                menuRoot.OnItemSelected -= OnItemSelected;
                menuRoot.OnItemHovered -= OnItemHovered;
                menuRoot.OnMenuOpened -= OnMenuOpened;
            }
        }

        private void OnItemSelected(ClickMenuItem item)
        {
            if (item == null || item.id == -1)
            {
                audioSource.PlayOneShot(audioBack);
            }
            else
            {
                audioSource.PlayOneShot(audioSelect);
            }
        }

        private void OnItemHovered(ClickMenuItem item)
        {
            audioSource.PlayOneShot(audioHover);
        }

        private void OnMenuOpened()
        {
            audioSource.PlayOneShot(audioSelect);
        }
    }
}
