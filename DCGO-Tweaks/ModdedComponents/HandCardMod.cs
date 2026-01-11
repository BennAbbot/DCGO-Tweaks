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

        static int s_age_counter = 0;

        public int Age { get; private set; }

        public bool IsDragging { get; set; }
        public void Awake()
        {
            Age = s_age_counter++;
        }

        public void Start()
        {
            _root_object = new GameObjectHandle("Parent", gameObject);
            _image_object = new GameObjectHandle("CardImage", _root_object);
            _highlight_object = new GameObjectHandle("Outline_Select", _root_object);

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
        }
    }
}
