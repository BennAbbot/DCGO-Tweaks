using Il2Cpp;
using Il2CppMHLab.Patch.Core.IO;
using UnityEngine;
using static Il2CppSystem.Runtime.Remoting.RemotingServices;

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

            AssetManager asset_manager = AssetManager.Instance;

            foreach (var deck in ContinuousController.instance.DeckDatas)
            {
                foreach (var card in deck.DeckCards())
                {
                    asset_manager.PreLoadImage(card.CardSpriteName);
                }

                foreach (var card in deck.DigitamaDeckCards())
                {
                    asset_manager.PreLoadImage(card.CardSpriteName);
                }
            }
        } 
        public void OnSceneWasInitialized()
        {
            BackgroundChanges.Apply(_scene_object_collection);
            CountInfoChanges.Apply(_scene_object_collection);

            FrameManager.InitForPlayer(GManager.instance.You, sort_dir: 1);
            FrameManager.InitForPlayer(GManager.instance.Opponent, sort_dir: - 1);
        }

        public void SceneUpdate()
        {
            BattleUIChanges.Update(_scene_object_collection);
            CountInfoChanges.Update(_scene_object_collection);
        }
    }
}
