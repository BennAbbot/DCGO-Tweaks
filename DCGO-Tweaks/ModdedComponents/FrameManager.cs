using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{
    using static Il2CppSystem.Uri;
    using FrameComponentList = Il2CppSystem.Collections.Generic.List<FrameComponent>;

    [HarmonyPatch(typeof(FieldCardFrame), nameof(FieldCardFrame.GetLocalCanvasPosition))]

    public static class FieldCardFrameGetLocalCanvasPositionPathc
    {
        private static void Prefix(FieldCardFrame __instance, ref Vector3 __result)
        {
            FrameManager frame_manager = __instance.player.GetComponent<FrameManager>();
            if (frame_manager != null)
            {
                frame_manager.UpdateAllRows();
            }

            __result = __instance.Frame.transform.localPosition + __instance.Frame.transform.parent.localPosition + __instance.Frame.transform.parent.parent.localPosition;
        }
    }

    [RegisterTypeInIl2Cpp]
    class FrameManager : MonoBehaviour
    {
        public FrameManager(IntPtr ptr) : base(ptr) { }

        Player _player;

        FrameComponentList _digimon_row = new FrameComponentList();
        FrameComponentList _tamer_row = new FrameComponentList();

        FrameComponent _breeding_frame;

        int _age_counter = 0;

        float _max_permanent_spacing = 160.0f;

        float _min_permanent_spacing = 160.0f;

        int _sort_dir = 1;

        public static void InitForPlayer(Player player, int sort_dir)
        {
            Settings settings = Settings.Instance;

            if (!settings.CollapseEmptySpace())
            {
                return;
            }

            FrameManager frame_manager = player.gameObject.AddComponent<FrameManager>();
            frame_manager._player = player;

            frame_manager._sort_dir = sort_dir;

            frame_manager._max_permanent_spacing = settings.FeildPermanentMaxSpacing();

            int index = 0;
            foreach (var card_frame in frame_manager._player.fieldCardFrames)
            {
                RectTransform rect_transform = card_frame.Frame.transform.parent.GetComponent<RectTransform>();
                FrameComponent frame_comp = card_frame.Frame.transform.parent.gameObject.AddComponent<FrameComponent>();
                frame_comp.CardFrame = card_frame;

                if (card_frame.isBreedingAreaFrame())
                {
                    frame_manager._breeding_frame = frame_comp;
                    continue;
                }

                if (index < 9)
                {
                    frame_manager._digimon_row.Add(frame_comp);
                }
                else
                {
                    frame_manager._tamer_row.Add(frame_comp);
                }

                index++;
            }

        }

        public void Update()
        {
            UpdateAllRows();
        }

        public void UpdateAllRows()
        {
            UpdateRow(_digimon_row);
            UpdateRow(_tamer_row);
        }

        public void UpdateRow(FrameComponentList frame_row)
        {
            FrameComponentList frames_with_new_permanents = new FrameComponentList();
            FrameComponentList frames_with_permanents = new FrameComponentList();

            foreach (var frame_comp in frame_row)
            {
                Permanent permanent = frame_comp.CardFrame.GetFramePermanent();

                if (permanent != null)
                {
                    frames_with_permanents.Add(frame_comp);
                }

                if (frame_comp.IsPermanentDirty())
                {
                    if (permanent == null)
                    {
                        frame_comp.PermanentAge = 0;
                    }
                    else 
                    {
                        frame_comp.PermanentAge = ++_age_counter * (permanent.IsOption ? -1 : 1);
                    }

                    frames_with_new_permanents.Add(frame_comp);
                }
            }

            if (frames_with_new_permanents.Count > 0)
            {
                float total_space = _min_permanent_spacing * frame_row.Count;
                float spacing = Math.Min(_max_permanent_spacing, total_space / frames_with_permanents.Count);

                float offset = 0.0f;

                if (frame_row == _tamer_row)
                {
                    if (frames_with_permanents.Count == 6)
                    {
                        spacing = _min_permanent_spacing;
                        offset = _min_permanent_spacing / 2.0f;
                    }
                    else if (frames_with_permanents.Count > 6)
                    {
                        spacing = _min_permanent_spacing;
                        offset = _min_permanent_spacing;
                    }
                }

                // Annoying, cant use .Sort so do it myself
                for (int i = 0; i < frames_with_permanents.Count - 1; i++)
                {
                    for (int j = i + 1; j < frames_with_permanents.Count; j++)
                    {
                        if (frames_with_permanents[i].PermanentAge * _sort_dir < _sort_dir * frames_with_permanents[j].PermanentAge)
                        {
                            FrameComponent temp = frames_with_permanents[i];
                            frames_with_permanents[i] = frames_with_permanents[j];
                            frames_with_permanents[j] = temp;
                        }
                    }
                }

                SpaceRow(spacing, frames_with_permanents, frames_with_new_permanents, offset);
            }

            void SpaceRow(float spacing, FrameComponentList frames_with_permanents, FrameComponentList frames_with_new_permanents, float centre_offset)
            {
                int index = 0;
                foreach (var frame_comp in frames_with_permanents)
                {
                    bool is_new = frames_with_new_permanents.Contains(frame_comp);
                    if (frame_comp.CardFrame.GetFramePermanent() != null)
                    {
                        frame_comp.MovePosition((index * spacing) - ((frames_with_permanents.Count - 1) * spacing * 0.5f) + centre_offset, is_new ? 0.0f : 0.3f, !is_new);
                        index++;
                    }
                    else if (is_new)
                    {
                        frame_comp.ResetPosition();
                    }
                }
            }
        }
    }
}
