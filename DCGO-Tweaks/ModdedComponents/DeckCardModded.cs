using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(CardPrefab_CreateDeck), "Start")]
    public static class CardPrefab_CreateDeckComp
    {
        private static void Postfix(CardPrefab_CreateDeck __instance)
        {
            __instance.gameObject.AddComponent<DeckCardModded>();
        }
    }

    [RegisterTypeInIl2Cpp]
    class DeckCardModded : MonoBehaviour
    {
        public DeckCardModded(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _image_object;



        CardPrefab_CreateDeck _deck_card;

        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;

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
            _deck_card = GetComponent<CardPrefab_CreateDeck>();

            _image_object = new GameObjectHandle("Parent.CardImage.Card", gameObject);

            _current_animated_image = AssetManager.Instance.GetAnimatedImage(_deck_card.cEntity_Base.CardSpriteName);
            if (_current_animated_image != null)
            {
                _animated_image_ui = Utils.CreateRawImageChild(_image_object?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);
                if (_animated_image_ui != null)
                {
                    _current_animated_image.SubscribeRawImage(_animated_image_ui);
                }
               
            }
        }

        public void OnDestroy()
        {
            if (_current_animated_image != null && _animated_image_ui != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
            }           
        }

        bool IsFlipped()
        {
            return _deck_card.CardImage == ContinuousController.instance.ReverseCard;
        }
    }
}
