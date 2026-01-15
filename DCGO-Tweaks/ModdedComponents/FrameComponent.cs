using Il2Cpp;
using Il2CppDG.Tweening;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public int PermanentAge { get; set; } = 0;

        List<RectTransform> _arrow_rects = new List<RectTransform>();
        Dictionary<RectTransform, Vector3> _arrow_target_map = new Dictionary<RectTransform, Vector3>();

        public void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            RectTransform.get_anchoredPosition_Injected(out _inital_pos);
        }

        public void ResetPosition()
        {
            MovePosition(_inital_pos.x, 0.0f , true);
        }

        public void Update()
        {
            // Whould Love to move this from update but dont know how to add OnUpdate to DOAnchorPos
            UpdateArrows();
        }

        public void MovePosition(float x_pos, float time, bool move_permanent)
        {
            if (move_permanent && !CardFrame.IsEmptyFrame())
            {
                RectTransform permanent_transfrom = CardFrame.GetFramePermanent().ShowingPermanentCard.gameObject.GetComponent<RectTransform>();
                if (permanent_transfrom)
                {
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

            RectTransform.get_anchoredPosition_Injected(out Vector2 pos);
            pos.x = x_pos;

            if (time > 0.0f)
            {
                RectTransform.DOAnchorPos(pos, time).SetEase(Ease.OutQuint);
            }
            else
            {
                RectTransform.set_anchoredPosition_Injected(ref pos);
            }

            UpdateArrows();
        }

        public void AddArrowRect(RectTransform arrow_rect)
        {
            if (!_arrow_rects.Contains(arrow_rect))
            {
                _arrow_rects.Add(arrow_rect);
            }
            
        }

        void UpdateArrows()
        {
            if (CardFrame.GetFramePermanent() != null)
            {
                for (int i = _arrow_rects.Count - 1; i >= 0; i--)
                {
                    RectTransform arrow_rect = _arrow_rects[i];
                    if (arrow_rect == null || !arrow_rect.gameObject.activeSelf)
                    {
                        _arrow_rects.RemoveAt(i);
                        _arrow_target_map.Remove(arrow_rect);
                        continue;
                    }
                    TargetArrow target_arrow = arrow_rect.GetComponent<TargetArrow>();
                    RectTransform root_rect = target_arrow.RootRect;

                    Vector3 arrow_pos = GetArrowPos();
                    if (arrow_rect.localPosition != arrow_pos)
                    {
                        if (!_arrow_target_map.ContainsKey(arrow_rect))
                        {
                            float angle = root_rect.localRotation.eulerAngles.z * Mathf.Deg2Rad;
                            float length = root_rect.sizeDelta.y;

                            Vector3 new_target_pos = arrow_rect.localPosition + new Vector3(
                                -Mathf.Sin(angle) * length,
                                Mathf.Cos(angle) * length,
                                0
                            );
                            _arrow_target_map.Add(arrow_rect, new_target_pos);
                        }

                        Vector3 target_pos = _arrow_target_map[arrow_rect];

                        arrow_rect.localPosition = arrow_pos;

                        Vector3 direction = target_pos - arrow_pos;
                        float new_angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
                        root_rect.localRotation = Quaternion.Euler(0, 0, new_angle);

                        root_rect.sizeDelta = new Vector2(root_rect.sizeDelta.x,  direction.magnitude);
                        target_arrow.TipRect.localPosition = new Vector3(0, root_rect.sizeDelta.y - 13, 0);
                    }
                }
            }
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
