using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
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
        GameObjectHandle _dp_ui;
        GameObjectHandle _source_count_ui;
        GameObjectHandle _level_ui;
        GameObjectHandle _linked_ui;
        TextMeshProUGUI _old_level_text;
        Text _new_level_text;
        bool _dp_last_active_state = false;

        FieldPermanentCard _feild_permanent_card = null;

        CardSource _last_top_card = null;

        RawImage _animated_image_ui = null;
        AnimatedImage _current_animated_image = null;

        public void Start()
        {
            _root_object = new GameObjectHandle("Parent", gameObject);
            _source_count_ui = new GameObjectHandle("EvoRoot", _root_object);
            _level_ui = new GameObjectHandle("Level", _root_object);
            _dp_ui = new GameObjectHandle("DP", _root_object);
            _linked_ui = new GameObjectHandle("LinkedRoot", _root_object);

            if (_dp_ui.GameObject != null)
            {
                _dp_last_active_state = _dp_ui.GameObject.activeSelf;
            }

            if (Settings.Instance.DCGOTweaksPermanentInfoUIStyle())
            {
                ApplyLinkStyle();
                ApplyLevelStyle(_dp_last_active_state);
                ApplyDPStyle();
                ApplySourceCountStyle(_dp_last_active_state);
                ApplyTappedStyle();
            }

            _feild_permanent_card = GetComponent<FieldPermanentCard>();

            ApplyOutlineAndShadowChanges();
        }

        void ApplyLinkStyle()
        {
            GameObject link_icon = _linked_ui.Child("Link Icon");

            if (_linked_ui.GameObject == null || link_icon == null)
            {
                return;
            }

            _linked_ui.GameObject.transform.localScale = Vector3.one;

            Vector3 local_Pos_Offset = new Vector3(60f, -58f, 0.0f);

            if (!Settings.Instance.RotateInfoUIWithPermanent())
            {
                _linked_ui.GameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                _linked_ui.GameObject.transform.localPosition = local_Pos_Offset;
                local_Pos_Offset = Vector3.zero;
            }

            link_icon.transform.localPosition = local_Pos_Offset;
            link_icon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 32.0f);
            link_icon.transform.localScale = new Vector3(0.5f, 0.5f, 0);

            Image image = link_icon.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = AssetManager.Instance.LinkIcon;
            }
        }

        void ApplyLevelStyle(bool dp_offset)
        {
            if (_level_ui.GameObject == null)
            {
                return;
            }

            Vector3 local_Pos_Offset = new Vector3(-40f, -35f, 0.0f);

            if (!dp_offset)
            {
                local_Pos_Offset += new Vector3(40f, -14.5f, 0f);
            }

            if (!Settings.Instance.RotateInfoUIWithPermanent())
            {
                _level_ui.GameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                _level_ui.GameObject.transform.localPosition = local_Pos_Offset;
                local_Pos_Offset = Vector3.zero;
            }

            _level_ui.GameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            _level_ui.GameObject.transform.SetSiblingIndex(12);

            GameObject background = _level_ui.Child("Background");

            if (background != null)
            {
                RectTransform rect = background.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.sizeDelta = new Vector2(44.75f, 21.5f);
                    rect.localScale = new Vector3(1.1f, 0.85f, 1f);
                    rect.transform.localPosition = local_Pos_Offset;
                }

                Image image = background.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = AssetManager.Instance.UnitFrameStats;
                    image.color = Color.white;
                }

                Shadow shadow = background.AddComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectDistance = new Vector2(-2, -2);
                    shadow.effectColor = new Color(0, 0, 0, 0.25f);
                }
            }

            GameObject level_text = _level_ui.Child("LevelText");
            if (level_text != null)
            {
                _old_level_text = level_text.GetComponent<TextMeshProUGUI>();
                _old_level_text.enabled = false;
            }

            GameObject new_level_text = _level_ui.Child("LevelTitleText");
            if (new_level_text != null)
            {
                _new_level_text = new_level_text.GetComponent<Text>();

                new_level_text.transform.localScale = new Vector3(0.05f, 0.05f, 0.0f);
                new_level_text.transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f) + local_Pos_Offset;
                Outline outline = new_level_text.AddComponent<Outline>();
            }
        }

        void ApplySourceCountStyle(bool dp_offset)
        {
            Vector3 local_Pos_Offset = new Vector3(40f, -35f, 0.0f);

            if (!dp_offset)
            {
                local_Pos_Offset += new Vector3(-40f, -14.5f, 0f);
            }

            if (!Settings.Instance.RotateInfoUIWithPermanent())
            {
                Utils.MoveUIItem(_source_count_ui, Vector2.zero);
            }
            else
            {
                Utils.MoveUIItem(_source_count_ui, local_Pos_Offset);
                local_Pos_Offset = Vector3.zero;
            }

            Utils.ScaleUIItem(_source_count_ui, 1.0f);

            _source_count_ui.ForEachChild((child) =>
            {
                if (child.name == "Background (2)" || child.name == "EvoRootCountBackground")
                {
                    child.SetActive(false);
                }
                else if (child.name == "Background (1)")
                {
                    RectTransform rect = child.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.sizeDelta = new Vector2(44.75f, 21.5f);
                        rect.localScale = new Vector3(1.1f, 0.85f, 1f);
                        rect.localPosition = local_Pos_Offset;
                    }

                    Image image = child.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = AssetManager.Instance.UnitFrameStats;
                        image.color = Color.white;
                    }

                    Shadow shadow = child.AddComponent<Shadow>();
                    if (shadow != null)
                    {
                        shadow.effectDistance = new Vector2(-2, -2);
                        shadow.effectColor = new Color(0, 0, 0, 0.25f);
                    }
                }
                else if (child.name == "EvoRootCountText")
                {
                    child.transform.localScale = new Vector3(0.05f, 0.05f, 0.0f);
                    child.transform.localPosition = new Vector3(0.0f, 1.5f, 0.0f) + local_Pos_Offset;
                    Text Text = child.GetComponent<Text>();
                    if (Text != null)
                    {
                        Text.fontStyle = FontStyle.Bold;
                    }
                }
            });
        }

        void ApplyTappedStyle()
        {
            if (!Settings.Instance.RotateInfoUIWithPermanent())
            {
                GameObject tapped_image = _root_object.Child("TapObject.Image");
                tapped_image.transform.localPosition = Vector3.zero;
            }
           
        }

        void ApplyDPStyle()
        {
            if (_dp_ui.GameObject == null)
            {
                return;
            }

            Vector3 local_Pos_Offset = new Vector3(0, -48.0f, 0);

            if (!Settings.Instance.RotateInfoUIWithPermanent())
            {
                _dp_ui.GameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                _dp_ui.GameObject.transform.localPosition = local_Pos_Offset;
                local_Pos_Offset = new Vector3(0.0f, -10.0f, 0.0f);
            }

            _dp_ui.ForEachChild((GameObject child) =>
            {
                if (child.name == "Background_black")
                {
                    child.SetActive(false);
                }
                else if (child.name == "DPText")
                {
                    child.transform.localPosition = new Vector3(0, -0.5f, 0) + local_Pos_Offset;
                    child.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
                    Outline outline = child.AddComponent<Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(8, -8);

                    Text text = child.GetComponent<Text>();
                    if (text != null)
                    {
                        text.fontStyle = FontStyle.Bold;
                    }
                }
                else
                {
                    child.transform.localPosition = local_Pos_Offset;
                    child.transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);

                    RectTransform child_rect = child.GetComponent<RectTransform>();
                    if (child_rect != null)
                    {
                        child_rect.sizeDelta = new Vector2(92, 31);
                    }

                    Image image = child.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = AssetManager.Instance.UnitFrame;
                    }

                    for (int i = 0; i < child.transform.childCount; i++)
                    {
                        GameObject mask = child.transform.GetChild(i).gameObject;

                        Image mask_image = mask.GetComponent<Image>();
                        if (mask_image != null)
                        {
                            mask_image.sprite = AssetManager.Instance.UnitFrame;
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

        void UpdateUIRotation()
        {
            if (Settings.Instance.RotateInfoUIWithPermanent() || !Settings.Instance.DCGOTweaksPermanentInfoUIStyle())
            {
                return;
            }

            if (_dp_ui.GameObject == null || _root_object.GameObject == null || _source_count_ui.GameObject == null || _level_ui.GameObject == null || _linked_ui.GameObject == null)
            {
                return;
            }

            Vector3 euler = _root_object.GameObject.transform.parent.localEulerAngles;
            euler.z *= -1;
            _dp_ui.GameObject.transform.localEulerAngles = euler;
            _source_count_ui.GameObject.transform.localEulerAngles = euler;
            _level_ui.GameObject.transform.localEulerAngles = euler;
            _linked_ui.GameObject.transform.localEulerAngles = euler;
        }

        void UpdateUIPosition()
        {
            if (!Settings.Instance.DCGOTweaksPermanentInfoUIStyle())
            {
                return;
            }

            if (_dp_ui.GameObject == null)
            {
                return;
            }

            if (_dp_ui.GameObject.activeSelf != _dp_last_active_state)
            {
                ApplyLevelStyle(_dp_ui.GameObject.activeSelf);
                ApplySourceCountStyle(_dp_ui.GameObject.activeSelf);

                _dp_last_active_state = _dp_ui.GameObject.activeSelf;
            }
        }

        void ApplyOutlineAndShadowChanges()
        {
            Settings settings = Settings.Instance;

            GameObject outline_obj = _root_object.Child("カード画像");
            Outline outline_comp = outline_obj != null ? outline_obj.GetComponent<Outline>() : null;

            _animated_image_ui = Utils.CreateRawImageChild(outline_obj?.GetComponent<RectTransform>(), AssetManager.Instance.CardMask);

            _last_top_card = _feild_permanent_card.ThisPermanent?.TopCard;
            UpdateAnimatedImage();

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

        void LateUpdate()
        {
            UpdateUIPosition();
            UpdateUIRotation();

            if (_new_level_text != null && _old_level_text != null)
            {
                _new_level_text.text = "Lv." + _old_level_text.text;
            }

            CardSource top_card = _feild_permanent_card.ThisPermanent?.TopCard;
            if (top_card != _last_top_card)
            {
                _last_top_card = top_card;

                UpdateAnimatedImage();
            }
        }

        public void OnDestroy()
        {
            if (_current_animated_image != null && _animated_image_ui != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
            }
        }

        void UpdateAnimatedImage()
        {
             CardSource top_card = _feild_permanent_card.ThisPermanent?.TopCard;

            if (_current_animated_image != null)
            {
                _current_animated_image.UnsubscribeRawImage(_animated_image_ui);
                _current_animated_image = null;
            }

            if (top_card != null)
            {
                string card_name = AssetManager.Instance.GetEntityFromCardIndex(top_card).CardSpriteName;
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
