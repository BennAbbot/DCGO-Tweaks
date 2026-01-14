using DCGO_Tweaks.ModdedComponents;
using MelonLoader;

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
            AssetManager.Instance.FindAnimatedImages();
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
            MainThreadDispatcher.CreateDispatcher();
            AssetManager.Instance.LoadSceneSprites();

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