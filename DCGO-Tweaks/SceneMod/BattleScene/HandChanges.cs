using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [HarmonyPatch(typeof(LayoutGroup), "OnEnable")]
    public static class UpdateOnLayoutGroupEnable
    {

        private static void Postfix(LayoutGroup __instance)
        {
            HandChanges.OnGridLayoutEnabled(__instance);
        }
    }

    [HarmonyPatch(typeof(Draggable_HandCard), "OnPointerEnter")]
    public static class UpdateOnHandCardOnPointerEnter
    {
        private static void Postfix(Draggable_HandCard __instance)
        {
            HandChanges.UpdateHandPositions(you: true);
        }
    }

    [HarmonyPatch(typeof(Draggable_HandCard), "OnPointerExit")]
    public static class UpdateOnHandCardOnPointerExit
    {
        private static void Postfix(Draggable_HandCard __instance)
        {
            HandChanges.UpdateHandPositions(you: true);
        }
    }

    [HarmonyPatch(typeof(Draggable_HandCard), "OnEndDrag")]
    public static class UpdateOnHandCardOnEndDrag
    {
        private static void Postfix(Draggable_HandCard __instance)
        {
            HandCardModded mod_comp = __instance.GetComponent<HandCardModded>();

            if (mod_comp != null)
            {
                mod_comp.IsDragging = false;
            }

            HandChanges.UpdateHandPositions(you: true);
        }
    }

    [HarmonyPatch(typeof(Draggable_HandCard), "OnDrag")]
    public static class UpdateOnHandCardOnDrag
    {
        private static void Postfix(Draggable_HandCard __instance)
        {
            HandChanges.UpdateHandPositions(you: true);
        }
    }

    [HarmonyPatch(typeof(Draggable_HandCard), "OnBeginDrag")]
    public static class UpdateOnHandCardOnBeginDrag
    {
        private static void Postfix(Draggable_HandCard __instance)
        {
            HandCardModded mod_comp = __instance.GetComponent<HandCardModded>();

            if (mod_comp != null)
            {
                mod_comp.IsDragging = true;
            }

            HandChanges.UpdateHandPositions(you: true);
        }
    }

    [HarmonyPatch(typeof(CardObjectController), "AlignHand", new Type[] { typeof(Player) })]
    public static class UpdateOnHAlignHand
    {
        private static void Postfix(CardObjectController __instance, Player player)
        {
            HandChanges.UpdateHandPositions(player);
        }
    }

    internal static class HandChanges
    {
        static GameObjectHandle _opponents_hand;
        static GameObjectHandle _your_hand;

        const float _your_hand_curve_height = 30.0f;
        const float _opponent_hand_curve_height = -15.0f;
        // I like this slop value, max angle of 10.9 degrees if arc_height is 30
        const float _curve_slop_value = (11.0f / 30.0f);

        public static void Apply(BattleSceneCollection object_collection)
        {
            _opponents_hand = object_collection.Opponent.HandObject;
            _your_hand = object_collection.You.HandObject;

            _opponents_hand.GetComponent<GridLayoutGroup>().enabled = false;
            _your_hand.GetComponent<GridLayoutGroup>().enabled = false;

            Settings settings = Settings.Instance;


            Utils.MoveUIItem(object_collection.You.HandRoot, new Vector2(6.72f, -590.0f));
            Utils.MoveUIItem(object_collection.Opponent.HandRoot, new Vector2(0.0f, 524.80f));

            Utils.OffsetUIItem(object_collection.You.HandObject, settings.YourHandOffset());
            Utils.OffsetUIItem(object_collection.Opponent.HandObject, settings.OpponentHandOffset());

            Utils.ScaleUIItem(object_collection.You.HandObject, 2.0f * settings.YourHandScale());
            Utils.ScaleUIItem(object_collection.Opponent.HandObject, settings.OpponentHandScale());
        }

        static void UpdateHandCardsPosition(GameObjectHandle hand_object, float max_spacing, float hand_width, float curve_height)
        {
            List<HandCardModded> cards = new List<HandCardModded>();

            RectTransform active_card = null;

            hand_object.ForEachChild(card =>
            {
                HandCardModded hand_card_modded = card.GetComponent<HandCardModded>();
                HandCard hand_card = card.GetComponent<HandCard>();

                if (hand_card_modded != null && hand_card != null)
                {
                    RectTransform card_rect = hand_card_modded.GetComponent<RectTransform>();

                    if (card_rect == null || card_rect.localScale.magnitude <= 0.0f)
                    {
                        return;
                    }

                    if (hand_card_modded.IsDragging || hand_card.IsExecuting)
                    {
                        active_card = card_rect;
                    }

                    cards.Add(hand_card_modded);                    
                }
            });

            RectTransform hand_rect = hand_object?.GetComponent<RectTransform>();

            cards.Sort((x, y) => x.Age.CompareTo(y.Age));

            int count = cards.Count;
            float total_width = hand_width;
            float card_spacing = count > 1 ? Math.Min(max_spacing, total_width / (count-1)) : 0.0f;
            float arc_width = card_spacing * (count -1);

            for (int i = 0; i < count; i++)
            {
                RectTransform rt = cards[i].GetComponent<RectTransform>();

                if (rt == active_card)
                {
                    Quaternion no_rot = Quaternion.identity;
                    Vector3 no_scale = Vector3.one;
                    rt.set_localScale_Injected(ref no_scale);
                    rt.set_localRotation_Injected(ref no_rot);
                    continue;
                }

                float x = (-arc_width / 2.0f) + (i * card_spacing);
                float normalized_x = x / (total_width/2.0f);
                float y = curve_height * (1f - normalized_x * normalized_x);

                Vector2 pos = new Vector2(x, y - curve_height);
                float angle = normalized_x * _curve_slop_value * curve_height;
                Quaternion rot = Quaternion.Euler(0, 0, -angle);

                Vector3 scale = Vector3.one;

                Draggable_HandCard draggable_hand_card = rt.GetComponent<Draggable_HandCard>();
                if (draggable_hand_card != null && draggable_hand_card.isExpand)
                {
                    pos.y += 50.0f;
                    scale *= 1.2f;
                }

                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.set_localScale_Injected(ref scale);
                rt.set_localRotation_Injected(ref rot);
                rt.set_anchoredPosition_Injected(ref pos);
            }
        }

        public static void UpdateHandPositions(bool you = false, bool opponent = false)
        {
            Settings settings = Settings.Instance;

            if (you)
            {
                UpdateHandCardsPosition(_your_hand, settings.YourHandMaxSpacing(), settings.YourHandTotalWidth(), _your_hand_curve_height * settings.YourHandCurveScale());
            }

            if (opponent)
            {
               UpdateHandCardsPosition(_opponents_hand, settings.OpponentHandMaxSpacing(), settings.OpponentHandTotalWidth(), _opponent_hand_curve_height * settings.OpponentHandCurveScale());
            }
        }

        public static void UpdateHandPositions(Player player)
        {
            UpdateHandPositions(player.HandTransform == _your_hand?.GetComponent<Transform>(), player.HandTransform == _opponents_hand?.GetComponent<Transform>());
        }

        public static void OnGridLayoutEnabled(LayoutGroup layout)
        {
            if (_opponents_hand?.GameObject == layout.gameObject)
            {
                _opponents_hand.GetComponent<GridLayoutGroup>().enabled = false;
                UpdateHandPositions(opponent: true);
            }
            else if (_your_hand?.GameObject == layout.gameObject)
            {
                _your_hand.GetComponent<GridLayoutGroup>().enabled = false;
                UpdateHandPositions(you: true);
            }
        }
    }
}
