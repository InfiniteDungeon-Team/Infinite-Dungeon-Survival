using UnityEngine;
using UnityEngine.UI;

namespace MyGame.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        [Header("Âm thanh khi click")]
        public AudioClip clickSound;
        [Range(0f, 1f)]
        public float volume = 1f;

        private AudioSource audioSource;

        void Awake()
        {
            // Tìm hoặc tạo AudioSource riêng cho button này
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }

            // Thêm sự kiện phát âm thanh khi click button
            GetComponent<Button>().onClick.AddListener(PlayClickSound);
        }

        void PlayClickSound()
        {
            if (clickSound != null)
                audioSource.PlayOneShot(clickSound, volume);
        }
    }
}
