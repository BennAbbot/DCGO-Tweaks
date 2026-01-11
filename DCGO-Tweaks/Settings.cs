using MelonLoader;
using UnityEngine;

namespace DCGO_Tweaks
{
    internal class Settings
    {
        public static Settings Instance { get; private set; }

        const string ConfigPath = "UserData/DCGOTweaks.cfg";

        private Settings() 
        {
        }

        public static bool Init()
        {
            Instance = new Settings();
            Instance.InitBackgroundSettings();
            Instance.InitDeckSettings();
            Instance.InitEggDeckSettings();
            Instance.InitTrashSettings();
            Instance.InitBattleUISettings();
            Instance.InitFeildPermanentSettings();
            Instance.InitHandCardSettings();
            Instance.InitHandSettings();
            Instance.InitCountInfoSettings();

            return true;
        }

        #region Background Settings

        private MelonPreferences_Category _background_category;
        private MelonPreferences_Entry<bool> _backgrounds_enabled;
        private MelonPreferences_Entry<int>  _background_none_repeating_list_size;
        void InitBackgroundSettings()
        {
            _background_category = MelonPreferences.CreateCategory("Backgrounds");
            _backgrounds_enabled = _background_category.CreateEntry("Enabled", true);
            _background_none_repeating_list_size = _background_category.CreateEntry("NoneRepeatingListSize", 3, null, "The number of games before a background has a chance to appear again");
            _background_category.SetFilePath(ConfigPath);
        }

        public bool IsBackgroundsEnabled() => _backgrounds_enabled.Value;
        public int BackgroundNoneRepeatingListSize() => _background_none_repeating_list_size.Value;

        #endregion

        #region Deck Settings
        private MelonPreferences_Category _deck_category;
        private MelonPreferences_Entry<Vector2> _your_deck_offset;
        private MelonPreferences_Entry<Vector2> _opponent_deck_offset;
        private MelonPreferences_Entry<Color> _deck_outline_colour;
        private MelonPreferences_Entry<float> _deck_outline_scale;

        void InitDeckSettings()
        {
            _deck_category = MelonPreferences.CreateCategory("Deck");

            _your_deck_offset = _deck_category.CreateEntry("Your_Offset", Vector2.zero);
            _opponent_deck_offset = _deck_category.CreateEntry("Opponent_Offset", Vector2.zero);

            _deck_outline_colour = _deck_category.CreateEntry("Outline_Color", Color.black);
            _deck_outline_scale = _deck_category.CreateEntry("Outline_Scale", 1.0f);

            _deck_category.SetFilePath(ConfigPath);
        }

        public Vector2 YourDeckOffset() => _your_deck_offset.Value;
        public Vector2 OpponentDeckOffset() => _opponent_deck_offset.Value;
        public Color DeckOutlineColour() => _deck_outline_colour.Value;
        public float DeckOutlineScale() => _deck_outline_scale.Value;
        #endregion

        #region Egg Deck Settings
        private MelonPreferences_Category _egg_deck_category;
        private MelonPreferences_Entry<Vector2> _your_egg_deck_offset;
        private MelonPreferences_Entry<Vector2> _opponent_egg_deck_offset;
        private MelonPreferences_Entry<Color> _egg_deck_outline_colour;
        private MelonPreferences_Entry<float> _egg_deck_outline_scale;
        private MelonPreferences_Entry<Color> _egg_deck_selected_outline_color;
        private MelonPreferences_Entry<float> _egg_deck_selected_outline_scale;

        private MelonPreferences_Entry<bool> _egg_deck_frame_enabled;
        private MelonPreferences_Entry<bool> _breeding_area_frame_enabled;

        void InitEggDeckSettings()
        {
            _egg_deck_category = MelonPreferences.CreateCategory("Egg_Deck");

            _your_egg_deck_offset = _egg_deck_category.CreateEntry("Your_Offset", Vector2.zero);
            _opponent_egg_deck_offset = _egg_deck_category.CreateEntry("Opponent_Offset", Vector2.zero);

            _egg_deck_outline_colour = _egg_deck_category.CreateEntry("Outline_Color", Color.white);
            _egg_deck_outline_scale = _egg_deck_category.CreateEntry("Outline_Scale", 1.0f);

            _egg_deck_selected_outline_color = _egg_deck_category.CreateEntry("Selected_Outline_Color", new Color(0.9623f, 0.2678f, 0.2944f, 1.0f));
            _egg_deck_selected_outline_scale = _egg_deck_category.CreateEntry("Selected_Outline_Scale", 1.0f);

            _egg_deck_frame_enabled = _egg_deck_category.CreateEntry("Frame_Enabled", false);
            _breeding_area_frame_enabled = _egg_deck_category.CreateEntry("Breeding_Area_Frame_Enabled", false);
            _egg_deck_category.SetFilePath(ConfigPath);
        }

        public Vector2 YourEggDeckOffset() => _your_egg_deck_offset.Value;
        public Vector2 OpponentEggDeckOffset() => _opponent_egg_deck_offset.Value;
        public bool EggDeckFrameEnabled() => _egg_deck_frame_enabled.Value;
        public bool BreedingAreaFrameEnabled() => _breeding_area_frame_enabled.Value;

        public Color EggDeckOutlineColour() => _egg_deck_outline_colour.Value;
        public float EggDeckOutlineScale() => _egg_deck_outline_scale.Value;

        public Color EggDeckSelectedOutlineColour() => _egg_deck_selected_outline_color.Value;
        public float EggDeckSelectedOutlineScale() => _egg_deck_selected_outline_scale.Value;
        #endregion

        #region Trash Settings
        private MelonPreferences_Category _trash_category;
        private MelonPreferences_Entry<Vector2> _your_trash_offset;
        private MelonPreferences_Entry<Vector2> _opponent_trash_offset;
        private MelonPreferences_Entry<bool> _trash_frame_enabled;

        void InitTrashSettings()
        {
            _trash_category = MelonPreferences.CreateCategory("Trash");

            _your_trash_offset = _trash_category.CreateEntry("Your_Offset", Vector2.zero);
            _opponent_trash_offset = _trash_category.CreateEntry("Opponent_Offset", Vector2.zero);
            _trash_frame_enabled = _trash_category.CreateEntry("Frame_Enabled", false);

            _trash_category.SetFilePath(ConfigPath);
        }

        public Vector2 YourTrashOffet() => _your_trash_offset.Value;
        public Vector2 OpponentTrashOffet() => _opponent_trash_offset.Value;
        public bool TrashFrameEnabled() => _trash_frame_enabled.Value;
        #endregion

        #region Battle UI Settings
        private MelonPreferences_Category _battle_ui_category;
        private MelonPreferences_Entry<Vector2> _log_button_offset;
        private MelonPreferences_Entry<float> _log_button_scale;

        private MelonPreferences_Entry<Vector2> _option_button_offset;
        private MelonPreferences_Entry<float> _option_button_scale;

        private MelonPreferences_Entry<Vector2> _show_phase_offset;
        private MelonPreferences_Entry<float> _show_phase_scale;

        private MelonPreferences_Entry<Vector2> _next_phase_offset;
        private MelonPreferences_Entry<float> _next_phase_scale;

        private MelonPreferences_Entry<Vector2> _revealed_cards_top_offset;
        private MelonPreferences_Entry<float> _revealed_cards_top_scale;

        private MelonPreferences_Entry<Vector2> _revealed_cards_bottom_offset;
        private MelonPreferences_Entry<float> _revealed_cards_bottom_scale;

        private MelonPreferences_Entry<Vector2> _effect_description_offset;
        private MelonPreferences_Entry<float> _effect_description_scale;

        private MelonPreferences_Entry<Vector2> _syncing_text_offset;
        private MelonPreferences_Entry<float> _syncing_text_scale;

        private MelonPreferences_Entry<Vector2> _your_name_offset;
        private MelonPreferences_Entry<float> _your_name_scale;

        private MelonPreferences_Entry<Vector2> _opponent_name_offset;
        private MelonPreferences_Entry<float> _opponent_name_scale;

        private MelonPreferences_Entry<Vector2> _your_security_offset;
        private MelonPreferences_Entry<float> _your_security_scale;
        private MelonPreferences_Entry<bool> _your_security_flip_x;
        private MelonPreferences_Entry<bool> _your_security_flip_y;

        private MelonPreferences_Entry<Vector2> _opponent_security_offset;
        private MelonPreferences_Entry<float> _opponent_security_scale;
        private MelonPreferences_Entry<bool> _opponent_security_flip_x;
        private MelonPreferences_Entry<bool> _opponent_security_flip_y;

        void InitBattleUISettings()
        {
            _battle_ui_category = MelonPreferences.CreateCategory("Battle_UI");

            _log_button_offset = _battle_ui_category.CreateEntry("Log_Button_Offset", Vector2.zero);
            _log_button_scale = _battle_ui_category.CreateEntry("Log_Button_Scale", 1.0f);

            _option_button_offset = _battle_ui_category.CreateEntry("Option_Button_Offset", Vector2.zero);
            _option_button_scale = _battle_ui_category.CreateEntry("Option_Button_Scale", 1.0f);

            _next_phase_offset = _battle_ui_category.CreateEntry("Next_Phase_Button_Offset", Vector2.zero);
            _next_phase_scale = _battle_ui_category.CreateEntry("Next_Phase_Scale", 1.0f);

            _show_phase_offset = _battle_ui_category.CreateEntry("Show_Phase_Offset", Vector2.zero);
            _show_phase_scale = _battle_ui_category.CreateEntry("Show_Phase_Scale", 1.0f);

            _revealed_cards_top_offset = _battle_ui_category.CreateEntry("Revealed_Cards_Top_Offset", Vector2.zero);
            _revealed_cards_top_scale = _battle_ui_category.CreateEntry("Revealed_Cards_Top_Scale", 1.0f);

            _revealed_cards_bottom_offset = _battle_ui_category.CreateEntry("Revealed_Cards_Bottom_Offset", Vector2.zero);
            _revealed_cards_bottom_scale = _battle_ui_category.CreateEntry("Revealed_Cards_Bottom_Scale", 1.0f);

            _effect_description_offset = _battle_ui_category.CreateEntry("Effect_Description_Offset", Vector2.zero);
            _effect_description_scale = _battle_ui_category.CreateEntry("Effect_Description_Scale", 1.0f);

            _syncing_text_offset = _battle_ui_category.CreateEntry("Syncing_Text_Offset", Vector2.zero);
            _syncing_text_scale = _battle_ui_category.CreateEntry("Syncing_Text_Scale", 1.0f);

            _your_name_offset = _battle_ui_category.CreateEntry("Your_Name_Offset", Vector2.zero);
            _your_name_scale = _battle_ui_category.CreateEntry("Your_Name_Scale", 1.0f);

            _opponent_name_offset = _battle_ui_category.CreateEntry("Opponent_Name_Offset", Vector2.zero);
            _opponent_name_scale = _battle_ui_category.CreateEntry("Opponent_Name_Scale", 1.0f);

            _your_security_offset = _battle_ui_category.CreateEntry("Your_Security_Offset", Vector2.zero);
            _your_security_scale = _battle_ui_category.CreateEntry("Your_Security_Scale", 1.0f);
            _your_security_flip_x = _battle_ui_category.CreateEntry("Your_Security_Flip_X", true);
            _your_security_flip_y = _battle_ui_category.CreateEntry("Your_Security_Flip_Y", false);

            _opponent_security_offset = _battle_ui_category.CreateEntry("Opponent_Security_Offset", Vector2.zero);
            _opponent_security_scale = _battle_ui_category.CreateEntry("Opponent_Security_Scale", 1.0f);
            _opponent_security_flip_x = _battle_ui_category.CreateEntry("Opponent_Security_Flip_X", false);
            _opponent_security_flip_y = _battle_ui_category.CreateEntry("Opponent_Security_Flip_Y", false);

            _battle_ui_category.SetFilePath(ConfigPath);
        }

        public Vector2 LogButtonOffset() => _log_button_offset.Value;
        public float LogButtonScale() => _log_button_scale.Value;

        public Vector2 OptionButtonOffset() => _option_button_offset.Value;
        public float OptionButtonScale() => _option_button_scale.Value;

        public Vector2 ShowPhaseOffset() => _show_phase_offset.Value;
        public float ShowPhaseScale() => _show_phase_scale.Value;

        public Vector2 NextPhaseOffset() => _next_phase_offset.Value;
        public float NextPhaseScale() => _next_phase_scale.Value;

        public Vector2 RevealedCardsTopOffset() => _revealed_cards_top_offset.Value;
        public float RevealedCardsTopScale() => _revealed_cards_top_scale.Value;

        public Vector2 RevealedCardsBottomOffset() => _revealed_cards_bottom_offset.Value;
        public float RevealedCardsBottomScale() => _revealed_cards_bottom_scale.Value;

        public Vector2 EffectDescriptionOffset() => _effect_description_offset.Value;
        public float EffectDescriptionScale() => _effect_description_scale.Value;

        public Vector2 SyncingTextOffset() => _syncing_text_offset.Value;
        public float SyncingTextScale() => _syncing_text_scale.Value;

        public Vector2 YourNameOffset() => _your_name_offset.Value;
        public float YourNameScale() => _your_name_scale.Value;

        public Vector2 OpponentNameOffset() => _opponent_name_offset.Value;
        public float OpponentNameScale() => _opponent_name_scale.Value;

        public Vector2 YourSecurityOffset() => _your_security_offset.Value;
        public float YourSecurityScale() => _your_security_scale.Value;

        public bool YourSecurityFlipX() => _your_security_flip_x.Value;

        public bool YourSecurityFlipY() => _your_security_flip_y.Value;

        public Vector2 OpponentSecurityOffset() => _opponent_security_offset.Value;
        public float OpponentSecurityScale() => _opponent_security_scale.Value;

        public bool OpponentSecurityFlipX() => _opponent_security_flip_x.Value;

        public bool OpponentSecurityFlipY() => _opponent_security_flip_y.Value;
        #endregion

        #region Feild Permanent Settings
        private MelonPreferences_Category _feild_permanent_settings;
        private MelonPreferences_Entry<Color> _feild_permanent_outline_colour;
        private MelonPreferences_Entry<float> _feild_permanent_outline_scale;
        private MelonPreferences_Entry<float> _feild_permanent_highlight_outline_scale;
        private MelonPreferences_Entry<bool> _feild_permanent_highlight_outline_glow;
        private MelonPreferences_Entry<bool> _feild_permanent_shadow;


        void InitFeildPermanentSettings()
        {
            _feild_permanent_settings = MelonPreferences.CreateCategory("Feild_Permanent");

            _feild_permanent_outline_colour = _feild_permanent_settings.CreateEntry("Outline_Color", Color.black);
            _feild_permanent_outline_scale = _trash_category.CreateEntry("Outline_Scale", 1.0f);
            _feild_permanent_highlight_outline_scale = _trash_category.CreateEntry("Highlight_Outline_Scale", 1.0f);
            _feild_permanent_highlight_outline_glow = _feild_permanent_settings.CreateEntry("Highlight_Outline_Glow", true);
            _feild_permanent_shadow = _trash_category.CreateEntry("Shadow", true);

            _feild_permanent_settings.SetFilePath(ConfigPath);
        }

        public Color FeildPermanentOutlineColour() => _feild_permanent_outline_colour.Value;
        public float FeildPermanentOutlineScale() => _feild_permanent_outline_scale.Value;
        public float FeildPermanentHighlightOutlineScale() => _feild_permanent_highlight_outline_scale.Value;
        public bool FeildPermanentHighlightOutlineGlow() => _feild_permanent_highlight_outline_glow.Value;
        public bool FeildPermanentShadow() => _feild_permanent_shadow.Value;
        #endregion

        #region Hand Settings
        private MelonPreferences_Category _hand_settings;

        private MelonPreferences_Entry<Vector2> _your_hand_offset;
        private MelonPreferences_Entry<float> _your_hand_max_spacing;
        private MelonPreferences_Entry<float> _your_hand_scale;
        private MelonPreferences_Entry<float> _your_hand_total_width;
        private MelonPreferences_Entry<float> _your_hand_curve_scale;

        private MelonPreferences_Entry<Vector2> _opponent_hand_offset;
        private MelonPreferences_Entry<float> _opponent_hand_max_spacing;
        private MelonPreferences_Entry<float> _opponent_hand_scale;
        private MelonPreferences_Entry<float> _opponent_hand_total_width;
        private MelonPreferences_Entry<float> _opponent_hand_curve_height;

        void InitHandSettings()
        {
            _hand_settings = MelonPreferences.CreateCategory("Hand");

            _your_hand_offset = _hand_settings.CreateEntry("Your_Hand_Offset", Vector2.zero);
            _your_hand_max_spacing = _hand_settings.CreateEntry("Your_Hand_Max_Spacing", 90.0f);
            _your_hand_scale = _hand_settings.CreateEntry("Your_Hand_Scale", 1.0f);
            _your_hand_total_width = _hand_settings.CreateEntry("Your_Hand_Total_Width", 540.0f);
            _your_hand_curve_scale = _hand_settings.CreateEntry("Your_Hand_Curve_Scale", 1.0f);

            _opponent_hand_offset = _hand_settings.CreateEntry("Opponent_Hand_Offset", Vector2.zero);
            _opponent_hand_max_spacing = _hand_settings.CreateEntry("Opponent_Hand_Max_Spacing", 90.0f);
            _opponent_hand_scale = _hand_settings.CreateEntry("Opponent_Hand_Scale", 1.0f);
            _opponent_hand_total_width = _hand_settings.CreateEntry("Opponent_Hand_Total_Width", 450.0f);
            _opponent_hand_curve_height = _hand_settings.CreateEntry("Opponent_Hand_Curve_Scale", 1.0f);

            _hand_settings.SetFilePath(ConfigPath);
        }

        public Vector2 YourHandOffset() => _your_hand_offset.Value;
        public float YourHandMaxSpacing() => _your_hand_max_spacing.Value;
        public float YourHandScale() => _your_hand_scale.Value;

        public float YourHandTotalWidth() => _your_hand_total_width.Value;

        public float YourHandCurveScale() => _your_hand_curve_scale.Value;

        public Vector2 OpponentHandOffset() => _opponent_hand_offset.Value;
        public float OpponentHandMaxSpacing() => _opponent_hand_max_spacing.Value;
        public float OpponentHandScale() => _opponent_hand_scale.Value;
        public float OpponentHandTotalWidth() => _opponent_hand_total_width.Value;
        public float OpponentHandCurveScale() => _opponent_hand_curve_height.Value;

        #endregion

        #region Hand Card Settings
        private MelonPreferences_Category _hand_card_settings;

        private MelonPreferences_Entry<Color> _hand_card_outline_colour;
        private MelonPreferences_Entry<float> _hand_card_outline_scale;
        private MelonPreferences_Entry<float> _hand_card_highlight_outline_scale;
        void InitHandCardSettings()
        {
            _hand_card_settings = MelonPreferences.CreateCategory("Hand_Card");

            _hand_card_outline_colour = _hand_card_settings.CreateEntry("Outline_Color", Color.black);
            _hand_card_outline_scale = _hand_card_settings.CreateEntry("Outline_Scale", 0.0f);
            _hand_card_highlight_outline_scale = _hand_card_settings.CreateEntry("Highlight_Outline_Scale", 1.0f);

            _hand_card_settings.SetFilePath(ConfigPath);
        }

        public Color HandCardOutlineColour() => _hand_card_outline_colour.Value;
        public float HandCardOutlineScale() => _hand_card_outline_scale.Value;
        public float HandCardHighlightOutlineScale() => _hand_card_highlight_outline_scale.Value;

        #endregion

        #region Count Info Settings
        private MelonPreferences_Category _count_info_settings;

        private MelonPreferences_Entry<KeyCode> _count_info_show_info_key;
        private MelonPreferences_Entry<bool> _count_info_show_deck_count;
        private MelonPreferences_Entry<Vector2> _your_deck_count_offset;
        private MelonPreferences_Entry<Vector2> _opponent_deck_count_offset;
        private MelonPreferences_Entry<bool> _count_info_show_egg_deck_count;
        private MelonPreferences_Entry<bool> _count_info_show_trash_count;
        private MelonPreferences_Entry<bool> _count_info_show_hand_count;

        void InitCountInfoSettings()
        {
            _count_info_settings = MelonPreferences.CreateCategory("Count_Info");

            _count_info_show_info_key = _count_info_settings.CreateEntry("Show_Info_Key", KeyCode.LeftAlt);
            _count_info_show_deck_count = _count_info_settings.CreateEntry("Show_Deck_Count", false);
            _your_deck_count_offset = _count_info_settings.CreateEntry("Deck_Count_Offset", Vector2.zero);
            _opponent_deck_count_offset = _count_info_settings.CreateEntry("Opponent_Offset", Vector2.zero);
            _count_info_show_egg_deck_count = _count_info_settings.CreateEntry("Show_Egg_Deck_Count", false);
            _count_info_show_trash_count = _count_info_settings.CreateEntry("Show_Trash_Count", false);
            _count_info_show_hand_count = _count_info_settings.CreateEntry("Show_Hand_Count", false);

            _count_info_settings.SetFilePath(ConfigPath);
        }

        public KeyCode ShowInfoKey() => _count_info_show_info_key.Value;
        public bool ShowHandCount() => _count_info_show_hand_count.Value;
        public bool ShowTrashCount() => _count_info_show_trash_count.Value;
        public bool ShowEggDeckCount() => _count_info_show_egg_deck_count.Value;
        public bool ShowDeckCount() => _count_info_show_deck_count.Value;

        public Vector2 YourDeckCountOffset() => _your_deck_count_offset.Value;
        public Vector2 OpponenttDeckCountOffset() => _opponent_deck_count_offset.Value;

        #endregion
    }
}
