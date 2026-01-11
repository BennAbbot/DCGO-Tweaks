using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    [HarmonyPatch(typeof(FieldPermanentCard), "Awake")]
    public static class FieldPermanentCardAddComp
    {
        private static void Postfix(FieldPermanentCard __instance)
        {
            __instance.gameObject.AddComponent<FeildPermanentCardModded>();
        }
    }

    [RegisterTypeInIl2Cpp]
    class FeildPermanentCardModded : MonoBehaviour
    {
        public FeildPermanentCardModded(IntPtr ptr) : base(ptr) { }

        GameObjectHandle _root_object;

        public void Start()
        {
            _root_object = new GameObjectHandle("Parent", gameObject);

            ApplyOutlineAndShadowChanges();
        }

        void ApplyOutlineAndShadowChanges()
        {
            Settings settings = Settings.Instance;

            GameObject outline_obj = _root_object.Child("カード画像");
            Outline outline_comp = outline_obj != null ? outline_obj.GetComponent<Outline>() : null;

            if (outline_comp)
            {
                outline_comp.effectColor = settings.FeildPermanentOutlineColour();
                outline_comp.effectDistance = new Vector2(1.0f, 1.0f) * settings.FeildPermanentOutlineScale();

                if (Settings.Instance.FeildPermanentShadow())
                {
                    Shadow Shadow = outline_obj.AddComponent<Shadow>();
                    Shadow.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
                    Shadow.effectDistance = new Vector2(-7.0f, -7.0f);
                }
            }

            GameObject highlight_outline_obj = _root_object.Child("Outline_Select");
            Image highlight_image = highlight_outline_obj != null ? highlight_outline_obj.GetComponent<Image>() : null;
            if (highlight_image)
            {
                Vector3 OutlineSize = new Vector3(1.15f, 1.1f, 1.0f) * settings.FeildPermanentHighlightOutlineScale();
                highlight_outline_obj.transform.set_localScale_Injected(ref OutlineSize);

                if (settings.FeildPermanentHighlightOutlineGlow())
                {
                    highlight_image.sprite = AssetManager.Instance.GetSceneSprite("カード角丸マスク_glow");
                }
            }
        }

        void OnDestroy()
        {
            
        }
    }
}
