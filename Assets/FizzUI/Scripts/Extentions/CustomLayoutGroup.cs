using UnityEngine.UI;
using UnityEngine;

namespace Fizz.UI.Extentions
{
    public class CustomLayoutGroup : LayoutGroup
    {
        [SerializeField] protected float m_Spacing = 0;
        public float spacing { get { return m_Spacing; } set { SetProperty (ref m_Spacing, value); } }

        [SerializeField] protected bool m_ChildForceExpandWidth = true;
        public bool childForceExpandWidth { get { return m_ChildForceExpandWidth; } set { SetProperty (ref m_ChildForceExpandWidth, value); } }

        [SerializeField] protected bool m_ChildForceExpandHeight = true;
        public bool childForceExpandHeight { get { return m_ChildForceExpandHeight; } set { SetProperty (ref m_ChildForceExpandHeight, value); } }

        public override void CalculateLayoutInputHorizontal ()
        {
            base.CalculateLayoutInputHorizontal ();
            CalcAlongAxis (0, true);
        }

        public override void CalculateLayoutInputVertical ()
        {
            CalcAlongAxis (1, true);
        }

        public override void SetLayoutHorizontal ()
        {
            SetChildrenAlongAxis (0, true);
        }

        public override void SetLayoutVertical ()
        {
            SetChildrenAlongAxis (1, true);
        }

        protected void CalcAlongAxis (int axis, bool isVertical)
        {
            float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);

            float totalMin = combinedPadding;
            float totalPreferred = combinedPadding;
            float totalFlexible = 0;

            bool alongOtherAxis = (isVertical ^ (axis == 1));
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float min = LayoutUtility.GetMinSize (child, axis);
                float preferred = LayoutUtility.GetPreferredSize (child, axis);
                float flexible = LayoutUtility.GetFlexibleSize (child, axis);
                if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                    flexible = Mathf.Max (flexible, 1);

                if (alongOtherAxis)
                {
                    totalMin = Mathf.Max (min + combinedPadding, totalMin);
                    totalPreferred = Mathf.Max (preferred + combinedPadding, totalPreferred);
                    totalFlexible = Mathf.Max (flexible, totalFlexible);
                }
                else
                {
                    totalMin += min + spacing;
                    totalPreferred += preferred + spacing;

                    // Increment flexible size with element's flexible size.
                    totalFlexible += flexible;
                }
            }

            if (!alongOtherAxis && rectChildren.Count > 0)
            {
                totalMin -= spacing;
                totalPreferred -= spacing;
            }
            totalPreferred = Mathf.Max (totalMin, totalPreferred);
            SetLayoutInputForAxis (totalMin, totalPreferred, totalFlexible, axis);
        }

        protected void SetChildrenAlongAxis (int axis, bool isVertical)
        {
            float size = rectTransform.rect.size[axis];

            bool alongOtherAxis = (isVertical ^ (axis == 1));
            if (alongOtherAxis)
            {
                float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];
                    float min = LayoutUtility.GetMinSize (child, axis);
                    float preferred = LayoutUtility.GetPreferredSize (child, axis);
                    float flexible = LayoutUtility.GetFlexibleSize (child, axis);
                    if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                        flexible = Mathf.Max (flexible, 1);

                    float requiredSpace = Mathf.Clamp (innerSize, min, flexible > 0 ? size : preferred);
                    TextAnchor anchor = childAlignment;
                    if (child.GetComponent<CustomLayoutGroupElement> () != null)
                    {
                        anchor = child.GetComponent<CustomLayoutGroupElement> ().anchor;
                    }
                    float startOffset = GetStartOffsetttt (axis, requiredSpace, anchor);
                    SetChildAlongAxis (child, axis, startOffset, requiredSpace);
                }
            }
            else
            {
                float pos = (axis == 0 ? padding.left : padding.top);
                if (GetTotalFlexibleSize (axis) == 0 && GetTotalPreferredSize (axis) < size)
                    pos = GetStartOffset (axis, GetTotalPreferredSize (axis) - (axis == 0 ? padding.horizontal : padding.vertical));

                float minMaxLerp = 0;
                if (GetTotalMinSize (axis) != GetTotalPreferredSize (axis))
                    minMaxLerp = Mathf.Clamp01 ((size - GetTotalMinSize (axis)) / (GetTotalPreferredSize (axis) - GetTotalMinSize (axis)));

                float itemFlexibleMultiplier = 0;
                if (size > GetTotalPreferredSize (axis))
                {
                    if (GetTotalFlexibleSize (axis) > 0)
                        itemFlexibleMultiplier = (size - GetTotalPreferredSize (axis)) / GetTotalFlexibleSize (axis);
                }

                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];
                    float min = LayoutUtility.GetMinSize (child, axis);
                    float preferred = LayoutUtility.GetPreferredSize (child, axis);
                    float flexible = LayoutUtility.GetFlexibleSize (child, axis);
                    if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                        flexible = Mathf.Max (flexible, 1);

                    float childSize = Mathf.Lerp (min, preferred, minMaxLerp);
                    childSize += flexible * itemFlexibleMultiplier;
                    SetChildAlongAxis (child, axis, pos, childSize);
                    pos += childSize + spacing;
                }
            }
        }

        protected float GetStartOffsetttt (int axis, float requiredSpaceWithoutPadding, TextAnchor tAnchor)
        {
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
            float availableSpace = rectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            float alignmentOnAxis = 0;
            if (axis == 0)
                alignmentOnAxis = ((int)tAnchor % 3) * 0.5f;
            else
                alignmentOnAxis = ((int)tAnchor / 3) * 0.5f;
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }
    }
}