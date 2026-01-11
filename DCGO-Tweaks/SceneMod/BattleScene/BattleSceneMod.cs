using UnityEngine;

namespace DCGO_Tweaks
{
    internal class BattleSceneMod : ISceneMod
    {
        BattleSceneCollection _scene_object_collection;

        public void OnSceneLoaded()
        {
            _scene_object_collection = new BattleSceneCollection();

            BattleUIChanges.Apply(_scene_object_collection);
            DecksChanges.Apply(_scene_object_collection);
            TrashChanges.Apply(_scene_object_collection);
            MemoryChanges.Apply(_scene_object_collection);
            HandChanges.Apply(_scene_object_collection);
            
        }
        public void OnSceneWasInitialized()
        {
            BackgroundChanges.Apply(_scene_object_collection);
            CountInfoChanges.Apply(_scene_object_collection);
        }

        public void SceneUpdate()
        {
            BattleUIChanges.Update(_scene_object_collection);
            CountInfoChanges.Update(_scene_object_collection);
        }
    }
}
