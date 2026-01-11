using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    internal static class DecksChanges
    {
        public static void Apply(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            MoveDecks(object_collection);
            MoveEggDecks(object_collection);

            object_collection.You.EggDeckFrame.SetActive(settings.EggDeckFrameEnabled());
            object_collection.Opponent.EggDeckFrame.SetActive(settings.EggDeckFrameEnabled());

            object_collection.You.BreedingAreaFrame.SetActive(settings.BreedingAreaFrameEnabled());
            object_collection.Opponent.BreedingAreaFrame.SetActive(settings.BreedingAreaFrameEnabled());

            ChangeDeckColour(object_collection.You?.EggDeck, settings.EggDeckOutlineColour(), settings.EggDeckOutlineScale());
            ChangeDeckColour(object_collection.Opponent?.EggDeck, settings.EggDeckOutlineColour(), settings.EggDeckOutlineScale());
            ChangeYouEggDeckTopOutline(object_collection.You.HatchImage, settings.EggDeckSelectedOutlineColour(), 3.0f * settings.EggDeckSelectedOutlineScale());

            ChangeDeckColour(object_collection.You?.Deck, settings.DeckOutlineColour(), settings.DeckOutlineScale());
            ChangeDeckColour(object_collection.Opponent?.Deck, settings.DeckOutlineColour(), settings.DeckOutlineScale());
        }

        static void ChangeYouEggDeckTopOutline(GameObjectHandle hatch_iamge, Color color, float outline_size)
        {
            Outline outline = hatch_iamge?.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = color;
                outline.effectDistance = new Vector2(outline_size, outline_size);
            }
        }

        static void MoveDecks(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            {
                Vector2 deck_offset = settings.YourDeckOffset();
                Vector2 pos = new Vector2(837.6f, -196.0f) + deck_offset;

                RectTransform deck_transform = object_collection.You?.Deck?.GetComponent<RectTransform>();
                if (deck_transform != null)
                {
                    deck_transform.set_anchoredPosition_Injected(ref pos);
                }
            }

            {
                Vector2 deck_offset = settings.OpponentDeckOffset();
                Vector2 pos = new Vector2(-816f, 212.0f) + deck_offset;

                GameObjectHandle Deck = object_collection.Opponent?.Deck;
                RectTransform deck_transform = Deck?.GetComponent<RectTransform>();

                if (deck_transform != null)
                {
                    deck_transform.set_anchoredPosition_Injected(ref pos);
                }

                GameObject deck_cards = Deck?.Child("DeckCards");
                RectTransform deck_cards_trans = deck_cards != null ? deck_cards.GetComponent<RectTransform>() : null;

                if (deck_cards_trans != null)
                {
                    // Dumb hack because mulligan flips the deck
                    for (int i = 0; i < deck_cards_trans.childCount; i++)
                    {
                        RectTransform child = deck_cards_trans.GetChild(i).GetComponent<RectTransform>();

                        if (child != null)
                        {
                            child.get_anchoredPosition_Injected(out Vector2 deck_pos);
                            deck_pos.y *= -1.0f;
                            child.set_anchoredPosition_Injected(ref deck_pos);
                        }
                    }
                }
            }
        }

        static void MoveEggDecks(BattleSceneCollection object_collection)
        {
            Settings settings = Settings.Instance;

            {
                RectTransform deck_transform = object_collection.You?.EggDeck?.GetComponent<RectTransform>();

                Vector2 deck_offset = settings.YourEggDeckOffset();

                if (deck_transform != null)
                {
                    deck_transform.get_anchoredPosition_Injected(out Vector2 pos);
                    pos += deck_offset;

                    deck_transform.set_anchoredPosition_Injected(ref pos);
                }
            }

            {
                GameObjectHandle EggDeck = object_collection.Opponent?.EggDeck;
                RectTransform deck_transform = EggDeck?.GetComponent<RectTransform>();

                if (deck_transform != null)
                {
                    Vector2 deck_offset = settings.OpponentEggDeckOffset();

                    deck_transform.get_anchoredPosition_Injected(out Vector2 pos);
                    pos += deck_offset;

                    deck_transform.set_anchoredPosition_Injected(ref pos);
                }

                GameObject egg_deck_cards = EggDeck?.Child("DeckCards");
                RectTransform egg_deck_cards_trans = egg_deck_cards != null ? egg_deck_cards.GetComponent<RectTransform>() : null;

                if (egg_deck_cards_trans != null)
                {
                    Quaternion rot = Quaternion.EulerAngles(Mathf.Deg2Rad * 5.0f, 0.0f, 0.0f);
                    egg_deck_cards_trans.set_localRotation_Injected(ref rot);
                }

            }
        }

        static void ChangeDeckColour(GameObjectHandle Deck, Color colour, float size)
        {
            {
                GameObject deck = Deck?.Child("DeckCards");
                RectTransform deck_cards_trans = deck != null ? deck.GetComponent<RectTransform>() : null;

                if (deck_cards_trans != null)
                {
                    for (int i = 0; i < deck_cards_trans.childCount; i++)
                    {
                        Outline outline = deck_cards_trans.GetChild(i).GetComponent<Outline>();

                        if (outline != null)
                        {
                            outline.effectColor = colour;
                            outline.effectDistance = new Vector2(size, size);
                        }
                    }
                }
            }
        }
    }
}
