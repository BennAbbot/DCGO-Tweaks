using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(PermanentDetail), "OpenUnitDetail")]
    public static class PermanentDetailOpenUnitDetailPatch
    {
        private static void Postfix(PermanentDetail __instance, Permanent permanent)
        {
            PermanentDetailsMod modded_comp = __instance.gameObject.GetComponent<PermanentDetailsMod>();
            if (modded_comp == null)
            {
                modded_comp = __instance.gameObject.AddComponent<PermanentDetailsMod>();
            }

            if (permanent != null)
            {
                modded_comp.SetCardSource(permanent.TopCard);
            }
           
        }
    }

    [RegisterTypeInIl2Cpp]
    class PermanentDetailsMod : MonoBehaviour
    {
        public PermanentDetailsMod(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _image_object;

        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;

        public void Awake()
        {
            _image_object = new GameObjectHandle("PokemonDetailPanel.CardImage", gameObject);
            _animated_image_ui = Utils.CreateRawImageChild(_image_object?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);
        }

        public void SetCardSource(CardSource top_card)
        {
            if (!Settings.Instance.AnimatePermanentDetails())
            {
                return;
            }

            if (top_card == null)
            {
                return;
            }

            if (_current_animated_image != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
                _current_animated_image = null;
            }

            CEntity_Base Entity = AssetManager.Instance.GetEntityFromSource(top_card);
            _current_animated_image = null;

            if (Entity)
            {
                _current_animated_image = AssetManager.Instance.GetAnimatedImage(Entity.CardSpriteName);
            }

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
