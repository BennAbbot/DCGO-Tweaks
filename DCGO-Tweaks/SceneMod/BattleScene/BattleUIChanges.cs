using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    internal static class BattleUIChanges
    {
        public static void Apply(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            Utils.MoveUIItem(object_collection.LogButton, new Vector2(-838.5f, 498.3f) + settings.LogButtonOffset());
            Utils.ScaleUIItem(object_collection.LogButton, 0.45f * settings.LogButtonScale());

            Utils.MoveUIItem(object_collection.LogButtonMask, new Vector2(525.0f, -402.0f) + settings.LogButtonOffset());
            Utils.ScaleUIItem(object_collection.LogButtonMask, 0.45f * settings.LogButtonScale());

            Utils.MoveUIItem(object_collection.OptionButton, new Vector2(-917.8f, 498.3f) + settings.OptionButtonOffset());
            Utils.ScaleUIItem(object_collection.OptionButton, 0.45f * settings.OptionButtonScale());

            Utils.MoveUIItem(object_collection.OptionButtonMask, new Vector2(447.0f, -402.0f) + settings.OptionButtonOffset());
            Utils.ScaleUIItem(object_collection.OptionButtonMask, 0.45f * settings.OptionButtonScale());

            Utils.OffsetUIItem(object_collection.NextPhaseButton, settings.NextPhaseOffset());
            Utils.ScaleUIItem(object_collection.NextPhaseButton, 0.8f * settings.NextPhaseScale());

            Utils.OffsetUIItem(object_collection.ShowPhase, settings.ShowPhaseOffset());
            Utils.ScaleUIItem(object_collection.ShowPhase, 0.77f * settings.ShowPhaseScale());

            Utils.MoveUIItem(object_collection.ShowPhaseMask, new Vector2(-834.5f, 56.0f) + settings.ShowPhaseOffset());
            Utils.ScaleUIItem(object_collection.ShowPhaseMask, new Vector3(0.75f * settings.ShowPhaseScale(), 0.65f * settings.ShowPhaseScale(), 1.0f));

            Utils.MoveUIItem(object_collection.RevealCardsRoot, new Vector2(-62.5f, -65.5f));
            Utils.ScaleUIItem(object_collection.RevealCardsRoot, 0.65f);

            Utils.OffsetUIItem(object_collection.RevealCardsTop, settings.RevealedCardsTopOffset());
            Utils.ScaleUIItem(object_collection.RevealCardsTop, settings.RevealedCardsTopScale());

            Utils.MoveUIItem(object_collection.RevealCardsBottom, new Vector2(642.0f, 145.0f) + settings.RevealedCardsBottomOffset());
            Utils.ScaleUIItem(object_collection.RevealCardsBottom, settings.RevealedCardsBottomScale());

            Utils.MoveUIItem(object_collection.EffectDesciption, new Vector2(-750.0f, 380.0f) + settings.EffectDescriptionOffset());
            Utils.ScaleUIItem(object_collection.EffectDesciption, 0.5f * settings.EffectDescriptionScale());

            Utils.MoveUIItem(object_collection.SyncText, new Vector2(-760.0f, -501.0f) + settings.SyncingTextOffset());
            Utils.ScaleUIItem(object_collection.SyncText, 0.1f * settings.SyncingTextScale());

            ChangePlayerName(object_collection.You.PlayerName, new Vector2(-862.0f, -390.0f) + settings.YourNameOffset(), 0.55f * settings.YourNameScale());
            ChangePlayerName(object_collection.Opponent.PlayerName, new Vector2(862.0f, 300.0f) + settings.OpponentNameOffset(), 0.55f * settings.OpponentNameScale());

            ChangeSecurity(object_collection.You.SecurityUI, settings.YourSecurityOffset(), settings.YourSecurityScale(), settings.YourSecurityFlipX(), settings.YourSecurityFlipY());
            ChangeSecurity(object_collection.Opponent.SecurityUI, settings.OpponentSecurityOffset(), settings.OpponentSecurityScale(), settings.OpponentSecurityFlipX(), settings.OpponentSecurityFlipY());
        }

        public static void Update(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            Utils.OffsetUIItem(object_collection.RevealCardsTop, settings.RevealedCardsTopOffset());
            Utils.MoveUIItem(object_collection.RevealCardsBottom, new Vector2(642.0f, 145.0f) + settings.RevealedCardsBottomOffset());

            UpdateBreedingCardMask(object_collection.CardMask);
        }

        static void UpdateBreedingCardMask(GameObjectHandle card_mask)
        {
            //Rotate the Select Card Mask When Breeding is the target
            RectTransform rect_trans = card_mask?.GetComponent<RectTransform>();
            if (rect_trans)
            {
                rect_trans.get_anchoredPosition_Injected(out Vector2 Pos);

                if (Vector2.Distance(Pos, new Vector2(-657.0f, -275.11f)) < 1.0f || Vector2.Distance(Pos, new Vector2(-519.2f, -270.7606f)) < 1.0f)
                {
                    Quaternion rot = Quaternion.EulerAngles(Mathf.Deg2Rad * 5.0f, 0.0f, 0.0f);
                    rect_trans.set_localRotation_Injected(ref rot);
                }
            }
                
        }

        static void ChangePlayerName(GameObjectHandle player_name, Vector2 pos, float scale)
        {
            Utils.MoveUIItem(player_name, pos);
            Utils.ScaleUIItem(player_name, scale);

            GameObject background_obj = player_name.Child("Background");
            if (background_obj != null)
            {
                background_obj.GetComponent<Outline>().enabled = false;
                background_obj.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
            }
        }

        static void ChangeSecurity(GameObjectHandle Security, Vector2 offset, float scale, bool flip_x, bool flip_y)
        {
            RectTransform rect_trans = Security?.GetComponent<RectTransform>();
            if (rect_trans != null)
            {
                rect_trans.get_anchoredPosition_Injected(out Vector2 pos);
                pos.x *= flip_x ? -1 : 1;
                pos.y *= flip_y ? -1 : 1;
                pos += offset;
                rect_trans.set_anchoredPosition_Injected(ref pos);

                Vector3 scale_vector = new Vector3(scale, scale, 1.0f);
                rect_trans.set_localScale_Injected(ref scale_vector);
            }
        }
    }
}
