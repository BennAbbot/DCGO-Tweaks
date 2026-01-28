using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(CardPrefab_CreateDeck), "SetUpCardPrefab_CreateDeck")]
    public static class SetUpCardPrefab_CreateDeckPatch
    {
        private static void Postfix(CardPrefab_CreateDeck __instance, CEntity_Base _cEntity_Base)
        {
            DeckCardMod modded_comp = __instance.gameObject.GetComponent<DeckCardMod>();
            if (modded_comp == null)
            {
                modded_comp = __instance.gameObject.AddComponent<DeckCardMod>();
            }

            modded_comp.SetCard(_cEntity_Base);
        }
    }

    [RegisterTypeInIl2Cpp]
    class DeckCardMod : MonoBehaviour
    {
        public DeckCardMod(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _image_object;

        CardPrefab_CreateDeck _deck_card;

        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;

        public void Awake()
        {
            _deck_card = GetComponent<CardPrefab_CreateDeck>();

            _image_object = new GameObjectHandle("Parent.CardImage.Card", gameObject);
            _animated_image_ui = Utils.CreateRawImageChild(_image_object?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);
        }

        public void Start()
        {
            SetCard(_deck_card.cEntity_Base);
        }

        public void SetCard(CEntity_Base cEntity_Base)
        {
            if (!Settings.Instance.AnimatedDeckEditorCards())
            {
                return;
            }

            if (_current_animated_image != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
                _current_animated_image = null;
            }

            string card_name = cEntity_Base.CardSpriteName;
            _current_animated_image = AssetManager.Instance.GetAnimatedImage(card_name);
            if (_current_animated_image != null && _animated_image_ui != null)
            {
                _current_animated_image.SubscribeRawImage(_animated_image_ui);
            }
        }

        public void OnDestroy()
        {
            if (_current_animated_image != null && _animated_image_ui != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
            }           
        }
    }
}
