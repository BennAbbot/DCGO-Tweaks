using HarmonyLib;
using Il2Cpp;
using Il2CppDCGO.CardEntities;
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
        GameObjectHandle _cost;
        GameObjectHandle _evo_cost;
        GameObjectHandle _dp;

        HandCard _hand_card;

        static int s_age_counter = 0;

        public int Age { get; private set; }

        public bool IsDragging { get; set; }
        public void Awake()
        {
            Age = s_age_counter++;
        }

        public void Start()
        {
            _hand_card = GetComponent<HandCard>();

            _root_object = new GameObjectHandle("Parent", gameObject);
            _image_object = new GameObjectHandle("CardImage", _root_object);
            _highlight_object = new GameObjectHandle("Outline_Select", _root_object);

            GameObject level = _root_object.Child("Level");

            if (level != null)
            {
                for (int i = 0; i < level.transform.GetChildCount(); i++)
                {
                    level.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            ApplyCostStyle();
            ApplyDPStyle();
            ApplyOutlineChanges();
        }

        public void ApplyCostStyle()
        {
            _hand_card.WhiteCircle = AssetManager.Instance.CostCircle;

            _cost = new GameObjectHandle("Cost", _root_object);
            _evo_cost = new GameObjectHandle("EvoCost", _root_object);

            if (_cost.GameObject == null || _evo_cost.GameObject == null)
            {
                return;
            }

            Utils.MoveUIItem(_cost, new Vector2(-35.35f, 55.6f));
            Utils.ScaleUIItem(_cost, 0.75f);

            GameObject background = _cost.Child("background");
            Image background_image = background.GetComponent<Image>();
            Outline background_outline = background.GetComponent<Outline>();
            Shadow background_shadow = background.AddComponent<Shadow>();

            if (background_image != null)
            {
                background_image.sprite = AssetManager.Instance.CostCircle;
                background_image.color = Color.white;
            }

            if (background_outline != null)
            {
                background_outline.effectColor = new Color(1f, 1f, 1f, 0.5f);
                background_outline.effectDistance = new Vector2(0.5f, 0.5f);
            }

            if (background_shadow != null)
            {
                background_shadow.effectColor = new Color(0f, 0f, 0f, 0.25f);
                background_shadow.effectDistance = new Vector2(0, -2);
            }

            GameObject cost_mask = _cost.Child("color mask");

            cost_mask.transform.localScale = new Vector3(0.85f, 0.85f, 1.0f);
            cost_mask.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);

            Image cost_mask_image = cost_mask.GetComponent<Image>();
            if (cost_mask_image != null)
            {
                cost_mask_image.sprite = AssetManager.Instance.CostCircleRotated;
            }

            for (int i = 0; i < cost_mask.transform.GetChildCount(); i++)
            {
                GameObject child = cost_mask.transform.GetChild(i).gameObject;
                Image child_image = child.GetComponent<Image>();
                if (child_image != null)
                {
                    child_image.sprite = AssetManager.Instance.CostCircleRotated;
                }
            }

            Utils.MoveUIItem(_evo_cost, new Vector2(-38.5f, 12.6f));
            Utils.ScaleUIItem(_evo_cost, 0.8f);

            int evo_cost_index = 0;
            for (int i = _evo_cost.GameObject.transform.GetChildCount() -1; i >= 0; i--)
            {
                Transform evo_cost = _evo_cost.GameObject.transform.GetChild(i);

                float hieght_offset = (evo_cost_index % 4) * 23;
                evo_cost_index++;

                Vector3 pos = evo_cost.localPosition;
                pos.y = 23.0f - hieght_offset;
                evo_cost.localPosition = pos;

                for (int j = 0; j < evo_cost.GetChildCount(); j++)
                {
                    GameObject child = evo_cost.GetChild(j).gameObject;
                    
                    if (child.name == "outline (1)")
                    {
                        child.SetActive(false);
                    }
                    else if (child.name == "outline")
                    {
                        child.transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);

                        Outline outline = child.AddComponent<Outline>();
                        Shadow shadow = child.AddComponent<Shadow>();
                        Image image = child.GetComponent<Image>();
                        if (image != null)
                        {
                            image.sprite = AssetManager.Instance.CostCircle;
                        }
                        if (outline != null)
                        {
                            outline.effectColor = new Color(1f, 1f, 1f, 0.5f);
                            outline.effectDistance = new Vector2(0.5f, 0.5f);
                        }

                        if (shadow != null)
                        {
                            shadow.effectColor = new Color(0f, 0f, 0f, 0.25f);
                            shadow.effectDistance = new Vector2(0, -2);
                        }
                    }
                }
            }
        }

        void ApplyDPStyle()
        {
            _dp = new GameObjectHandle("DP", _root_object);

            if (_dp.GameObject == null)
            {
                return;
            }

            _dp.GameObject.transform.localPosition = Vector3.zero;

            _dp.ForEachChild((GameObject child) =>
            {
                if (child.name == "Background_black")
                {
                    child.SetActive(false);
                }
                else if (child.name == "DPText")
                {
                    child.transform.localPosition = new Vector3(0, -49.0f, 0);
                    child.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
                    Outline outline = child.AddComponent<Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(8, -8);
                }
                else
                {
                    Sprite sprite = AssetManager.Instance.DPHolder;

                    if (child.name == "DPBackground0_mask")
                    {
                        sprite = AssetManager.Instance.DPHolderMaskLeft;
                    }
                    else if (child.name == "DPBackground1_mask")
                    {
                        sprite = AssetManager.Instance.DPHolderMaskRight;
                    }

                    child.transform.localPosition = new Vector3(0, -48.0f, 0);
                    child.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);

                    RectTransform child_rect = child.GetComponent<RectTransform>();
                    if (child_rect != null)
                    {
                        child_rect.sizeDelta = new Vector2(92, 31);
                    }

                    Image image = child.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = sprite;
                    }

                    for (int i = 0; i < child.transform.childCount; i++)
                    {
                        GameObject mask = child.transform.GetChild(i).gameObject;

                        Image mask_image = mask.GetComponent<Image>();
                        if (mask_image != null)
                        {
                            mask_image.sprite = sprite;
                        }

                        RectTransform mask_rect = mask.GetComponent<RectTransform>();
                        if (mask_rect != null)
                        {
                            mask_rect.sizeDelta = new Vector2(92, 31);
                        }
                    }
                }
            });
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
