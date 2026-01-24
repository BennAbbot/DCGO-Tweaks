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
        GameObjectHandle _cost_ui;
        GameObjectHandle _evo_cost_ui;
        GameObjectHandle _dp_ui;

        HandCard _hand_card;

        CardSource _last_card_source = null;


        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;
        Image _normal_image = null;

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
            _hand_card = GetComponent<HandCard>();

            _root_object = new GameObjectHandle("Parent", gameObject);
            _image_object = new GameObjectHandle("CardImage", _root_object);
            _highlight_object = new GameObjectHandle("Outline_Select", _root_object);

            if (Settings.Instance.DCGOTweaksHandInfoUIStyle())
            {
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
            }

            ApplyOutlineChanges();
        }

        public void ApplyCostStyle()
        {
            _hand_card.WhiteCircle = AssetManager.Instance.CostCircle;

            _cost_ui = new GameObjectHandle("Cost", _root_object);
            _evo_cost_ui = new GameObjectHandle("EvoCost", _root_object);

            if (_cost_ui.GameObject == null || _evo_cost_ui.GameObject == null)
            {
                return;
            }

            Utils.MoveUIItem(_cost_ui, new Vector2(-35.35f, 55.6f));
            Utils.ScaleUIItem(_cost_ui, 0.75f);

            GameObject background = _cost_ui.Child("background");
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

            GameObject cost_mask = _cost_ui.Child("color mask");

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

            Utils.MoveUIItem(_evo_cost_ui, new Vector2(-38.5f, 12.6f));
            Utils.ScaleUIItem(_evo_cost_ui, 0.8f);

            int evo_cost_index = 0;
            for (int i = _evo_cost_ui.GameObject.transform.GetChildCount() -1; i >= 0; i--)
            {
                Transform evo_cost = _evo_cost_ui.GameObject.transform.GetChild(i);

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
            _dp_ui = new GameObjectHandle("DP", _root_object);

            if (_dp_ui.GameObject == null)
            {
                return;
            }

            _dp_ui.GameObject.transform.localPosition = Vector3.zero;

            _dp_ui.ForEachChild((GameObject child) =>
            {
                if (child.name == "Background_black")
                {
                    child.SetActive(false);
                }
                else if (child.name == "DPText")
                {
                    child.transform.localPosition = new Vector3(0, -48.5f, 0);
                    child.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
                    Outline outline = child.AddComponent<Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(8, -8);
                }
                else
                {
                    Sprite sprite = AssetManager.Instance.UnitFrame;

                    if (child.name == "DPBackground0_mask")
                    {
                        sprite = AssetManager.Instance.UnitFrameMaskRight;
                    }
                    else if (child.name == "DPBackground1_mask")
                    {
                        sprite = AssetManager.Instance.UnitFrameMaskLeft;
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

        public void OnDestroy()
        {
            if (_current_animated_image != null && _animated_image_ui != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
            }
        }
        public void ApplyOutlineChanges()
        {
            Settings settings = Settings.Instance;

            Transform highlight_transform = _highlight_object?.GameObject != null ? _highlight_object.GameObject.transform : null;
            if (highlight_transform != null)
            {
                Vector3 highlight_size = new Vector3(1.025f, 1.005f, 1.0f) * settings.HandCardHighlightOutlineScale();
                highlight_transform.set_localScale_Injected(ref highlight_size);
            }

            if (_image_object.GameObject != null)
            {
                _normal_image = _image_object.GetComponent<Image>();

                Outline outline = _image_object.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.effectDistance = new Vector2(1.0f, 1.0f) * settings.HandCardOutlineScale();
                    outline.effectColor = settings.HandCardOutlineColour();
                }

                _animated_image_ui = Utils.CreateRawImageChild(_image_object?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);

                UpdateAnimatedImage();
            }

            _last_is_flipped = IsFlipped();
            _last_card_source = _hand_card.cardSource;
        }

        bool IsFlipped()
        {
            return _hand_card.CardImage == ContinuousController.instance.ReverseCard;
        }

        void Update()
        {
            _normal_image.enabled = !_animated_image_ui.enabled;

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
            if (_current_animated_image != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
                _current_animated_image = null;
            }

            if (!IsFlipped() && _hand_card.cardSource != null )
            {
                string card_name = AssetManager.Instance.GetEntityFromCardIndex(_hand_card.cardSource).CardSpriteName;
                _current_animated_image = AssetManager.Instance.GetAnimatedImage(card_name);

                if (_current_animated_image != null)
                {
                    _current_animated_image.SubscribeRawImage(_animated_image_ui);
                }
                else
                {
                    _current_animated_image = null;
                }
            }
        }
    }
}
