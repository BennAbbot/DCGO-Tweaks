using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(DCGO_Tweaks.Core), "DCGO Tweaks", "1.0.0", "Lv.B", null)]
[assembly: MelonGame("DCGO", "DCGO")]

namespace DCGO_Tweaks
{
    public class Core : MelonMod
    {
        ISceneMod _current_scene_mod = null;

        public override void OnInitializeMelon()
        {
            Settings.Init();
        }

        public override void OnLateInitializeMelon()
        {
            AssetManager.Instance.FindAnimatedImages();
            AssetManager.Instance.LoadUIAssets();
            AssetManager.Instance.InitColours();

            if (Settings.Instance.ShowFps())
            {
                GameObject fpsObject = new GameObject("FPSCounter");
                fpsObject.AddComponent<FPSCounter>();
            }
        }

        public override void OnUpdate()
        {
            if (_current_scene_mod != null)
            {
                _current_scene_mod.SceneUpdate();
            }
        }

        public override void OnSceneWasLoaded(int build_index, string scene_name)
        {
            AssetManager asset_manager = AssetManager.Instance;

            asset_manager.LoadSceneSprites();
            asset_manager.CleanUpAnimatedImages(true);

            switch (scene_name)
            {
                case "BattleScene":
                    _current_scene_mod = new BattleSceneMod();
                    break;
            }

            if (_current_scene_mod != null)
            {
                _current_scene_mod.OnSceneLoaded();
            }
        }

        public override void OnSceneWasUnloaded(int build_index, string scene_name)
        {
            _current_scene_mod = null;
        }

        public override void OnSceneWasInitialized(int build_index, string scene_name)
        {
            if (_current_scene_mod != null)
            {
                _current_scene_mod.OnSceneWasInitialized();
            }
        }
    }
}