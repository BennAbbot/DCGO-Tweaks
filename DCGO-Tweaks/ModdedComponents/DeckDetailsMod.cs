using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(DetailCard_DeckEditor), "SetUpDetailCard")]
    public static class DetailCard_DeckEditorSetUpDetailCardPatch
    {
        private static void Postfix(PermanentDetail __instance, CEntity_Base cEntity_Base)
        {
            DeckDetailsMod modded_comp = __instance.gameObject.GetComponent<DeckDetailsMod>();
            if (modded_comp == null)
            {
                modded_comp = __instance.gameObject.AddComponent<DeckDetailsMod>();
            }

            modded_comp.SetCard(cEntity_Base);
        }
    }

    [RegisterTypeInIl2Cpp]
    class DeckDetailsMod : MonoBehaviour
    {
        public DeckDetailsMod(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _image_object;

        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;

        public void Awake()
        {
           _image_object = new GameObjectHandle("DetailCard", gameObject);
           _animated_image_ui = Utils.CreateRawImageChild(_image_object?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);
        }

        public void SetCard(CEntity_Base cEntity_Base)
        {
            if (!Settings.Instance.AnimatedDeckEditorDetails())
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
