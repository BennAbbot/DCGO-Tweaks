using Il2CppTMPro;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace DCGO_Tweaks
{
    internal class PlayerCollection
    {
        public enum PlayerType
        {
            You,
            Opponent
        }

        public GameObjectHandle Deck { get; private set; }
        public GameObjectHandle EggDeck { get; private set; }
        public GameObjectHandle Trash { get; private set; }
        public GameObjectHandle EggDeckFrame { get; private set; }
        public GameObjectHandle BreedingAreaFrame { get; private set; }
        public GameObjectHandle TrashFrame { get; private set; }

        public GameObjectHandle PlayMat { get; private set; }

        public GameObjectHandle SecurityUI { get; private set; }
        public GameObjectHandle PlayerName { get; private set; }
        public GameObjectHandle HandUI { get; private set; }

        public GameObjectHandle HatchImage { get; private set; }

        public GameObjectHandle HandRoot { get; private set; }
        public GameObjectHandle HandObject { get; private set; }

        public GameObjectHandle HandCount { get; private set; }

        public GameObjectHandle NewHandCount { get; set; }
        public GameObjectHandle DeckCount { get; private set; }
        public GameObjectHandle TrashCount { get; private set; }
        public GameObjectHandle EggDeckCount { get; private set; }

        public PlayerCollection(PlayerType player_type)
        {
            string item_prefix = "";
            string ui_prefix = "";
            string trash_count_name = "";

            switch (player_type)
            {
                case PlayerType.You:
                    item_prefix = "Your";
                    ui_prefix = "You";
                    trash_count_name = "TrashCountText";
                    break;
                case PlayerType.Opponent:
                    item_prefix = "Opponent";
                    ui_prefix = "Opponent";
                    trash_count_name = "TrashCountText (1)";
                    break;
                default:
                    break;
            }

            Deck = new GameObjectHandle(item_prefix + "MainDeck");

            EggDeck = new GameObjectHandle(item_prefix + "DigitamaDeck");
            EggDeckFrame = new GameObjectHandle(item_prefix + "DigitamaDeckFrame");

            BreedingAreaFrame = new GameObjectHandle(item_prefix + "BreedingAreaFrame");

            Trash = new GameObjectHandle(item_prefix + "Trash");
            TrashFrame = new GameObjectHandle(item_prefix + "TrashFrame");

            PlayMat = new GameObjectHandle(ui_prefix + ".PlayMat");

            SecurityUI = new GameObjectHandle(item_prefix + "Security");
            PlayerName = new GameObjectHandle(item_prefix + "PlayerName");
            HandUI = new GameObjectHandle(item_prefix + "Hand");

            HatchImage = new GameObjectHandle("DeckCards.HatchImage", EggDeck);

            HandRoot = new GameObjectHandle(item_prefix + "Hand");
            HandObject = new GameObjectHandle(item_prefix + "HandTransform");

            HandCount = new GameObjectHandle(item_prefix + "HandCount");

            DeckCount = new GameObjectHandle(item_prefix + "MainDeck.DeckCountText");

 
            TrashCount = new GameObjectHandle(item_prefix + "Trash." + trash_count_name);
            EggDeckCount = new GameObjectHandle(item_prefix + "DigitamaDeck.DigitamaDeckCountText");
        }
    }
}
