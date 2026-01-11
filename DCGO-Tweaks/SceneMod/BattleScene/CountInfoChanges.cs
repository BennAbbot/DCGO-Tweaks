using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    internal static class CountInfoChanges
    {
        public static void Apply(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            UpdateTextStyle(object_collection.You.DeckCount, 5.0f);
            UpdateTextStyle(object_collection.You.EggDeckCount, 2.0f);
            UpdateTextStyle(object_collection.You.TrashCount, 2.0f);

            UpdateTextStyle(object_collection.Opponent.DeckCount, 5.0f);
            UpdateTextStyle(object_collection.Opponent.EggDeckCount, 2.0f);
            UpdateTextStyle(object_collection.Opponent.TrashCount, 2.0f);

            Utils.MoveUIItem(object_collection.You.HandCount, new Vector2(910.0f, 20.0f) + settings.YourDeckCountOffset());
            Utils.MoveUIItem(object_collection.Opponent.HandCount, new Vector2(720.0f, -45.0f) + settings.OpponenttDeckCountOffset());
            
            if (!settings.ShowHandCount())
            {
                SetActive(object_collection.You.HandCount, false);
                SetActive(object_collection.Opponent.HandCount, false);

                {
                    GameObject new_hand_text_count = GameObject.Instantiate<GameObject>(object_collection.Opponent.TrashCount?.GameObject);
                    new_hand_text_count.transform.SetParent(object_collection.You.HandRoot?.GetComponent<Transform>(), false);
                    new_hand_text_count.transform.localScale = Vector3.one * 0.4f;
                    object_collection.You.NewHandCount = new GameObjectHandle(new_hand_text_count);
                    Utils.MoveUIItem(object_collection.You.NewHandCount, new Vector2(0.0f, 185.0f) + settings.YourHandOffset() + settings.YourDeckCountOffset());
                }

                {
                    GameObject new_hand_text_count = GameObject.Instantiate<GameObject>(object_collection.Opponent.TrashCount?.GameObject);
                    new_hand_text_count.transform.SetParent(object_collection.Opponent.HandRoot?.GetComponent<Transform>(), false);
                    new_hand_text_count.transform.localScale = Vector3.one * 0.4f;
                    object_collection.Opponent.NewHandCount = new GameObjectHandle(new_hand_text_count);
                    Utils.MoveUIItem(object_collection.Opponent.NewHandCount, new Vector2(0.0f, -100.0f) + settings.OpponentHandOffset() + settings.OpponenttDeckCountOffset());
                }
            }

            SetAllActive(object_collection, false);
        }

        public static void Update(BattleSceneCollection object_collection)
        {
            SetAllActive(object_collection, Input.GetKey(Settings.Instance.ShowInfoKey()) || Input.GetMouseButton(2));

            UpdateHandText(object_collection.Opponent.NewHandCount, object_collection.Opponent.HandObject);
            UpdateHandText(object_collection.You.NewHandCount, object_collection.You.HandObject);
        }

        static void UpdateHandText(GameObjectHandle text_object, GameObjectHandle hand_root)
        {
            Text text = text_object?.GetComponent<Text>();
            Transform count_trans = hand_root?.GameObject ? hand_root.GameObject.transform : null;
            if (text != null && count_trans != null)
            {
                text.text = count_trans.GetChildCount().ToString();
            }
        }

        static void SetAllActive(BattleSceneCollection object_collection, bool active)
        {
            Settings settings = Settings.Instance;

            SetActive(object_collection.You.DeckCount, active || settings.ShowDeckCount());
            SetActive(object_collection.You.EggDeckCount, active || settings.ShowEggDeckCount());
            SetActive(object_collection.You.TrashCount, active || settings.ShowTrashCount());
            SetActive(object_collection.You.NewHandCount, active);

            SetActive(object_collection.Opponent.DeckCount, active || settings.ShowDeckCount());
            SetActive(object_collection.Opponent.EggDeckCount, active || settings.ShowEggDeckCount());
            SetActive(object_collection.Opponent.TrashCount, active || settings.ShowTrashCount());
            SetActive(object_collection.Opponent.NewHandCount, active);
        }

        public static void SetActive(GameObjectHandle text_object, bool active)
        {
            if (text_object?.GameObject != null)
            {
                text_object.GameObject.SetActive(active);
            }
        }

        public static void UpdateTextStyle(GameObjectHandle text_object, float outline_width)
        {
            Text text = text_object?.GetComponent<Text>();
            if (text != null)
            {
                text.color = Color.white;
            }

            Outline outline = text_object?.GetComponent<Outline>();
            if (outline)
            {
                outline.effectColor = Color.black;
                outline.effectDistance = new Vector2(outline_width, outline_width);
            }
            
        }
    }
}
