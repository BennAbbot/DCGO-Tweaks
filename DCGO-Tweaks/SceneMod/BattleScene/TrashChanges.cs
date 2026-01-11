using UnityEngine;

namespace DCGO_Tweaks
{
    internal static class TrashChanges
    {
        public static void Apply(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            Utils.MoveUIItem(object_collection.You?.Trash, new Vector2(837.6f, -366.0f) + settings.YourTrashOffet());
            Utils.MoveUIItem(object_collection.Opponent?.Trash, new Vector2(-801.0f, 374.0f) + settings.OpponentTrashOffet());

            object_collection.You.TrashFrame.SetActive(settings.TrashFrameEnabled());
            object_collection.Opponent.TrashFrame.SetActive(settings.TrashFrameEnabled());
        }
    }
}
