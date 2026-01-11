
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    internal static class MemoryChanges
    {
        public static void Apply(BattleSceneCollection object_collection)
        {
            FixMemoryMask(object_collection.MemoryMask);

            if (Settings.Instance.IsBackgroundsEnabled())
            {
                TransparentMemory(object_collection.Memory);
            }
        }

        static void TransparentMemory(GameObjectHandle memory)
        {
            if (memory.GameObject == null)
            {
                return;
            }

            memory.Child("line").SetActive(false);
            memory.Child("line (1)").SetActive(false);

            memory.ForEachChild("Grid (1)", (child_object) =>
            {
                GameObject image_1_object = GameObjectHandle.FindObject("Image (1)", child_object);
                Image image_1 = image_1_object?.GetComponent<Image>();

                if (image_1 != null)
                {
                    Color color = image_1.color;
                    color.a = 1.0f;
                    image_1.color = color;

                    Il2CppArrayBase<Shadow> shadows = image_1_object.GetComponents<Shadow>();

                    foreach (Shadow child in shadows)
                    {
                        child.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.4f);
                    }
                }

                GameObject image_2_object = GameObjectHandle.FindObject("Image", child_object);

                Image image_2 = image_2_object?.GetComponent<Image>();
                if (image_2 != null)
                {
                    Color color = image_2.color;
                    color.a = 1.0f;
                    image_2.color = color;
                }

                Shadow shadow_2 = image_2_object?.GetComponent<Shadow>();
                if (shadow_2 != null)
                {
                    shadow_2.effectColor = new Color(0.0f, 0.0f, 0.0f, 0.4f);
                }
            });
        }

        static void FixMemoryMask(GameObjectHandle memory_mask)
        {
            RectTransform memory_mask_transform = memory_mask?.GetComponent<RectTransform>();

            if (memory_mask_transform != null)
            {
                Vector2 location = new Vector2(0.0f, 55.0f);
                memory_mask_transform.set_anchoredPosition_Injected(ref location);
                Vector3 scale = new Vector3(1.0f, 0.90f, 1.0f);
                memory_mask_transform.set_localScale_Injected(ref scale);
            }

            GridLayoutGroup memory_mask_grid_layout = memory_mask.GetComponent<GridLayoutGroup>();

            if (memory_mask_grid_layout)
            {
                memory_mask_grid_layout.cellSize = new Vector2(59.7f, 67.5f);
            }

            memory_mask.ForEachChild((GameObject) =>
            {
                Image image = GameObject.GetComponent<Image>();
                if (image)
                {
                    image.sprite = AssetManager.Instance.GetSceneSprite("hexagon_orange");
                }
            });
        }
    }
}
