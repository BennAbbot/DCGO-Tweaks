using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(HandCard), "Awake")]
    public static class HandCardAddComp
    {
        private static void Postfix(HandCard __instance)
        {
            __instance.gameObject.AddComponent<HandCardModded>();
        }
    }

    [RegisterTypeInIl2Cpp]
    class HandCardModded : MonoBehaviour
    {
        public HandCardModded(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _root_object;
        GameObjectHandle _image_object;
        GameObjectHandle _highlight_object;

        HandCard _hand_card = null;

        CardSource _last_card_source = null;

        AnimatedImageComp _animated_image_comp = null;

        static int s_age_counter = 0;

        public int Age { get; private set; }

        public bool IsDragging { get; set; }
        bool _last_is_flipped = false;

        public void Awake()
        {
            Age = s_age_counter++;
        }

        public void Start()
        {
            _root_object = new GameObjectHandle("Parent", gameObject);
            _image_object = new GameObjectHandle("CardImage", _root_object);
            _highlight_object = new GameObjectHandle("Outline_Select", _root_object);

            _hand_card = GetComponent<HandCard>();

            ApplyOutlineChanges();
        }

        public void ApplyOutlineChanges()
        {
            Settings settings = Settings.Instance;

            Outline outline = _image_object?.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectDistance = new Vector2(1.0f, 1.0f) * settings.HandCardOutlineScale();
                outline.effectColor = settings.HandCardOutlineColour();
            }

            Transform highlight_transform = _highlight_object?.GameObject != null ? _highlight_object.GameObject.transform : null;
            if (highlight_transform != null)
            {
                Vector3 highlight_size = new Vector3(1.025f, 1.005f, 1.0f) * settings.HandCardHighlightOutlineScale();
                highlight_transform.set_localScale_Injected(ref highlight_size);
            }

            _animated_image_comp = outline.gameObject.AddComponent<AnimatedImageComp>();

            UpdateAnimatedImage();

            _last_is_flipped = IsFlipped();
            _last_card_source = _hand_card.cardSource;
        }

        bool IsFlipped()
        {
            return _hand_card.CardImage == ContinuousController.instance.ReverseCard;
        }

        void LateUpdate()
        {
            if (_last_is_flipped != IsFlipped())
            {
                _last_is_flipped = IsFlipped();

                UpdateAnimatedImage();
            }

            if (_last_card_source != _hand_card.cardSource)
            {
                _last_card_source = _hand_card.cardSource;
                UpdateAnimatedImage();
            }
        }

        void UpdateAnimatedImage()
        {
            if (!IsFlipped() && _hand_card.cardSource != null )
            {
                string card_name = AssetManager.Instance.GetEntityFromCardIndex(_hand_card.cardSource).CardSpriteName;
                _animated_image_comp.AnimatedImage = AssetManager.Instance.GetAnimatedImage(card_name);
            }
            else
            {
                _animated_image_comp.AnimatedImage = null;
            }
        }


    }
}
