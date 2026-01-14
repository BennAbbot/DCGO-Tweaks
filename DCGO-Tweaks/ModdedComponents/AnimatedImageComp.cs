using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using static MelonLoader.MelonLogger;

namespace DCGO_Tweaks
{
    [RegisterTypeInIl2Cpp]
    internal class AnimatedImageComp : MonoBehaviour
    {
        public AnimatedImageComp(IntPtr ptr) : base(ptr) { }

        AnimatedImageData _animated_image = null;
        public AnimatedImageData AnimatedImage
        {
            get
            {
                return _animated_image;
            }

            set
            {
                _animated_image = value;
                enabled = _animated_image != null;
            }
        }

        Image _ui_image;

        int _current_frame = 0;
        float _current_frame_time = 0.0f;
        Sprite _last_frame_sprite = null;

        void Start()
        {
            _ui_image = GetComponent<Image>();

            if (AnimatedImage != null)
            {
                _ui_image.sprite = AnimatedImage.GetFrameAt(0);
            }
        }

        void LateUpdate()
        {
            if (AnimatedImage != null)
            {
                _current_frame_time += Time.deltaTime;

                int next_frame_index = AnimatedImage.GetFrameIndexFrom(_current_frame_time, _current_frame);
                if (next_frame_index != _current_frame || _ui_image.sprite != _last_frame_sprite)
                {
                    _current_frame_time = 0.0f;
                    _current_frame = next_frame_index;
                    _last_frame_sprite = AnimatedImage.GetFrameAt(_current_frame);
                    _ui_image.sprite = _last_frame_sprite;
                }
            }
        }

    }
}
