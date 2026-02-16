using Il2Cpp;
using Il2CppDG.Tweening;
using MelonLoader;
using UnityEngine;
namespace DCGO_Tweaks
{
    [RegisterTypeInIl2Cpp]
    class FrameComponent : MonoBehaviour
    {
        public FrameComponent(IntPtr ptr) : base(ptr) { }

        public RectTransform RectTransform { get; private set; }

        Vector2 _inital_pos = Vector2.zero;

        public FieldCardFrame CardFrame { get; set; }

        Permanent _last_permanent = null;
        bool _is_default_state = true;

        public int PermanentAge { get; set; } = 0;

        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            RectTransform.get_anchoredPosition_Injected(out _inital_pos);
        }

        public void ResetPosition()
        {
            MovePosition(_inital_pos.x, 0.0f , true);
            _is_default_state = true;
        }

        public void MovePosition(float x_pos, float time, bool move_permanent)
        {
            if (move_permanent && !CardFrame.IsEmptyFrame())
            {
                RectTransform permanent_transfrom = CardFrame.GetFramePermanent().ShowingPermanentCard.gameObject.GetComponent<RectTransform>();
                if (permanent_transfrom)
                {
                    permanent_transfrom.DOKill(true);

                    permanent_transfrom.get_anchoredPosition_Injected(out Vector2 permanent_pos);
                    permanent_pos.x = x_pos;
                    if (time > 0.0f)
                    {
                        permanent_transfrom.DOAnchorPos(permanent_pos, time).SetEase(Ease.OutQuint);
                    }
                    else
                    {
                        permanent_transfrom.set_anchoredPosition_Injected(ref permanent_pos);
                    }
                   
                }
            }

            _is_default_state = false;
            RectTransform.get_anchoredPosition_Injected(out Vector2 pos);
            pos.x = x_pos;

            RectTransform.set_anchoredPosition_Injected(ref pos);
        }

        public bool IsPermanentDirty()
        {
            if (_last_permanent != CardFrame.GetFramePermanent())
            {
                _last_permanent = CardFrame.GetFramePermanent();
                return true;
            }

            return false;
        }

        public Vector3 GetArrowPos()
        {
            Vector3 pos = CardFrame.Frame.transform.localPosition + CardFrame.Frame.transform.parent.localPosition + CardFrame.Frame.transform.parent.parent.localPosition;
            if (CardFrame.GetFramePermanent() != null)
            {
                pos += CardFrame.framePermanent.TopCard.Owner.playerUIObjectParent.localPosition;
            }
            return pos;
        }
    }
}
