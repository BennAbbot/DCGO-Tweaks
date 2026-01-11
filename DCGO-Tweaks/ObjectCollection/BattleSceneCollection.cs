
namespace DCGO_Tweaks
{
    internal class BattleSceneCollection
    {
        public GameObjectHandle BoardUI { get; private set; }
        public GameObjectHandle Background { get; private set; }
        public GameObjectHandle DigitalLines { get; private set; }
        public GameObjectHandle Memory { get; private set; }
        public GameObjectHandle UIMask { get; private set; }
        public GameObjectHandle MemoryMask { get; private set; }
        public GameObjectHandle YourPlayMatFrame { get; private set; }

        public GameObjectHandle LogButton { get; private set; }
        public GameObjectHandle LogButtonMask { get; private set; }
        public GameObjectHandle OptionButton { get; private set; }
        public GameObjectHandle OptionButtonMask { get; private set; }

        public GameObjectHandle NextPhaseButton { get; private set; }

        public GameObjectHandle ShowPhase { get; private set; }
        public GameObjectHandle ShowPhaseMask { get; private set; }

        public GameObjectHandle RevealCardsRoot { get; private set; }
        public GameObjectHandle RevealCardsTop { get; private set; }
        public GameObjectHandle RevealCardsBottom { get; private set; }

        public GameObjectHandle EffectDesciption { get; private set; }
        public GameObjectHandle CardMask { get; private set; }

        public GameObjectHandle SyncText { get; private set; }
        public PlayerCollection You { get; private set; }
        public PlayerCollection Opponent { get; private set; }

        public BattleSceneCollection()
        {
            BoardUI = new GameObjectHandle("Board UI");
            UIMask = new GameObjectHandle("HideCannotSelectObject.Mask", BoardUI);

            Background = new GameObjectHandle("Background Effects.BackGround");
            DigitalLines = new GameObjectHandle("Particle_digitline");

            Memory = new GameObjectHandle("Memory");

            MemoryMask = new GameObjectHandle("Unmask_Memorys", UIMask);

            YourPlayMatFrame = new GameObjectHandle("YourObject.YourPlayMatFrame", BoardUI);

            You = new PlayerCollection(PlayerCollection.PlayerType.You);
            Opponent = new PlayerCollection(PlayerCollection.PlayerType.Opponent);

            LogButton = new GameObjectHandle("LogButton");
            LogButtonMask = new GameObjectHandle("Unmask_LogButton", UIMask);

            OptionButton = new GameObjectHandle("OptionButton");
            OptionButtonMask = new GameObjectHandle("Unmask_OptionButton", UIMask);

            NextPhaseButton = new GameObjectHandle("Next Phase");

            ShowPhase = new GameObjectHandle("ShowPhase");

            ShowPhaseMask = new GameObjectHandle("Unmask_ShowPhase", UIMask);

            EffectDesciption = new GameObjectHandle(" effect text display parent");

            SyncText = new GameObjectHandle("SyncText", BoardUI);

            RevealCardsRoot = new GameObjectHandle("ShowCardParent");
            RevealCardsTop = new GameObjectHandle("カード公開パネル1", RevealCardsRoot);
            RevealCardsBottom = new GameObjectHandle("カード公開パネル2", RevealCardsRoot);

            CardMask = new GameObjectHandle("Unmask_Cards.Unmask_Card", UIMask);
        }
    }
}
