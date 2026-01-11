
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    internal static class BackgroundChanges
    {
        public static void Apply(BattleSceneCollection collection)
        {
            if (!Settings.Instance.IsBackgroundsEnabled())
            {
                return;
            }

            Image background_sprite = collection.Background?.GetComponent<Image>();

            if (background_sprite != null)
            {
                background_sprite.sprite = AssetManager.Instance.RandomBackground();
            }

            DisableUnusedObjects(collection);
        }

        static void DisableUnusedObjects(BattleSceneCollection object_collection)
        {

            object_collection.DigitalLines.SetActive(false);

            object_collection.You?.PlayMat.SetActive(false);
            object_collection.Opponent?.PlayMat.SetActive(false);

            object_collection.YourPlayMatFrame?.ForEachChild((child_Object) =>
            {
                Image img = child_Object.GetComponent<Image>();
                if (img != null)
                {
                    img.color = Color.clear;

                    if (child_Object.name == "プレイマット枠_Selected")
                    {
                        img.enabled = false;
                    }
                }
            });         
        }
    }
}
