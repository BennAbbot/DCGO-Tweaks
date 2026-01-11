using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DCGO_Tweaks
{
    internal static class Utils
    {
        public static void MoveUIItem(GameObjectHandle ui_object, Vector2 pos)
        {
            RectTransform rect_trans = ui_object?.GetComponent<RectTransform>();
            if (rect_trans != null)
            {
                rect_trans.set_anchoredPosition_Injected(ref pos);
            }
        }

        public static void OffsetUIItem(GameObjectHandle ui_object, Vector2 offset)
        {
            RectTransform rect_trans = ui_object?.GetComponent<RectTransform>();
            if (rect_trans != null)
            {
                rect_trans.get_anchoredPosition_Injected(out Vector2 pos);
                pos += offset;
                rect_trans.set_anchoredPosition_Injected(ref pos);
            }
        }

        public static void ScaleUIItem(GameObjectHandle ui_object, float scale)
        {
            ScaleUIItem(ui_object, new Vector3(scale, scale, 1.0f));
        }

        public static void ScaleUIItem(GameObjectHandle ui_object, Vector3 scale)
        {
            RectTransform rect_trans = ui_object?.GetComponent<RectTransform>();
            if (rect_trans != null)
            {
                rect_trans.set_localScale_Injected(ref scale);
            }
        }
    }
}
