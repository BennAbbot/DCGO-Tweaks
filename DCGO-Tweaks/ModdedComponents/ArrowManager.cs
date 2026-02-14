using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace DCGO_Tweaks
{
    [HarmonyPatch(typeof(TargetArrow), nameof(TargetArrow.OnTargetArrow))]

    public static class TargetArrowSetTargetArrowPatch
    {
        private static void Postfix(TargetArrow __instance, Vector3 InitialPosition, Vector3 targetPosition, FieldPermanentCard StartFieldUnitCard, FieldPermanentCard EndFieldUnitCard)
        {
            ArrowManager arrow_manager = GManager.instance.You.GetComponent<ArrowManager>();

            if (arrow_manager != null)
            {
                arrow_manager.ArrowCreated(__instance, InitialPosition, targetPosition);
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    class ArrowManager : MonoBehaviour
    {
        private struct ManagedTargetArrow
        {
            public TargetArrow ArrowObj;
            public Vector3 InitalSourcePos;
            public Vector3 InitalTargetPos;
            public FrameComponent SourceFrame;
            public FrameComponent TargetFrame;

            public Vector3 _last_source_pos;
            public Vector3 _last_target_pos;

            public ManagedTargetArrow(TargetArrow arrow_obj, Vector3 inital_source_pos, Vector3 inital_target_pos, FrameComponent source_frame, FrameComponent target_frame)
            {
                ArrowObj = arrow_obj;
                InitalSourcePos = inital_source_pos;
                InitalTargetPos = inital_target_pos;
                SourceFrame = source_frame;
                TargetFrame = target_frame;
                _last_source_pos = inital_source_pos;
                _last_target_pos = inital_target_pos;
            }

            public void UpdatePosition()
            {
                Vector3 source_pos = SourceFrame != null ? SourceFrame.GetArrowPos() : InitalSourcePos;
                Vector3 target_pos = TargetFrame != null ? TargetFrame.GetArrowPos() : InitalTargetPos;

                if (_last_source_pos != source_pos || _last_target_pos != target_pos)
                {
                    ArrowObj.SetTargetArrow(source_pos, target_pos);
                }                
            }

        }

        public ArrowManager(IntPtr ptr) : base(ptr) { }

        FrameManager _you_frame_manager;
        FrameManager _opponent_frame_manager;

        List<ManagedTargetArrow> _managed_target_arrows = new List<ManagedTargetArrow>();

        public static void AddToManager(GManager manager)
        {
            ArrowManager instance = manager.You.gameObject.AddComponent<ArrowManager>();

            instance._you_frame_manager = manager.You.GetComponent<FrameManager>();
            instance._opponent_frame_manager = manager.Opponent.GetComponent<FrameManager>();
        }

        public void ArrowCreated(TargetArrow arrow_obj, Vector3 source_pos, Vector3 target_pos)
        {
            _managed_target_arrows.Add(new ManagedTargetArrow(arrow_obj, source_pos, target_pos, GetRelatedFrame(source_pos), GetRelatedFrame(target_pos)));
        }

        public void Update()
        {
            for (int i = _managed_target_arrows.Count - 1; i >= 0; i--)
            {
                ManagedTargetArrow target_arrow = _managed_target_arrows[i];
                if (target_arrow.ArrowObj == null)
                {
                    _managed_target_arrows.RemoveAt(i);
                    break;
                }

                target_arrow.UpdatePosition();
            }
        }

        FrameComponent GetRelatedFrame(Vector3 pos)
        {
            foreach (FrameComponent frame in _you_frame_manager.Frames)
            {
                if (frame.CardFrame.framePermanent != null && frame.GetArrowPos() == pos)
                {
                    return frame;
                }
            }

            foreach (FrameComponent frame in _opponent_frame_manager.Frames)
            {
                if (frame.CardFrame.framePermanent != null && frame.GetArrowPos() == pos)
                {
                    return frame;
                }
            }

            return null;
        }
    }
}
