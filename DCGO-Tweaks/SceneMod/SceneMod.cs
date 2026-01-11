namespace DCGO_Tweaks
{
    internal interface ISceneMod
    {
        public abstract void OnSceneLoaded();
        public abstract void OnSceneWasInitialized();
        public abstract void SceneUpdate();
    }
}
